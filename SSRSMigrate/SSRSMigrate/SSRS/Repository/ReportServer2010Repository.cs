using System;
using System.Collections.Generic;
using System.Linq;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Utility;
using System.Xml;
using SSRSMigrate.DataMapper;
using System.Web.Services.Protocols;
using SSRSMigrate.SSRS.Errors;
using Ninject;
using Ninject.Extensions.Logging;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Item.Proxy;
using SSRSMigrate.SSRS.Attributes;
using SSRSMigrate.Extensions;
using System.Text;

namespace SSRSMigrate.SSRS.Repository
{
    /// <summary>
    /// Implements methods for communciating with the ReportingService2010 interface
    /// </summary>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/reportservice2010.reportingservice2010?view=sqlserver-2016"/>
    [DefaultEndpointAttribute("reportservice2010.asmx")]
    public class ReportServer2010Repository : IReportServerRepository
    {
        private readonly ReportingService2010 mReportingService;
        private readonly ReportingService2010DataMapper mDataMapper;
        private ILogger mLogger = null;
        private string mRootPath = null;

        private const string ReportTypeName = "Report";
        private const string DatasetTypeName = "DataSet";
        private const string FolderTypeName = "Folder";
        private const string DataSourceTypeName = "DataSource";
        

        /// <summary>
        /// List of properties which can migrate between servers
        /// </summary>
        private readonly string[] PropertiesToMigrate = new string[]
        {
            "Description", "Hidden"
        };



        // Used to help with generic creation
        public ReportServer2010Repository() 
            : this("/", new ReportingService2010(), new ReportingService2010DataMapper())
        {

        }
        

        public ReportServer2010Repository(string rootPath,
            ReportingService2010 reportingService) 
            : this(rootPath, reportingService, new ReportingService2010DataMapper())
        {

        }

        public ReportServer2010Repository(string rootPath,
            ReportingService2010 reportingService,
            ReportingService2010DataMapper dataMapper)
        {
            if (string.IsNullOrEmpty(rootPath))
                throw new ArgumentException(nameof(rootPath));

            if (reportingService == null)
                throw new ArgumentNullException(nameof(reportingService));

            if (dataMapper == null)
                throw new ArgumentNullException(nameof(dataMapper));

            this.mRootPath = rootPath;
            this.mReportingService = reportingService;
            this.mDataMapper = dataMapper;
        }

        ~ReportServer2010Repository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.mReportingService != null)
                {
                    this.mReportingService.Dispose();
                    //this.mReportingService = null;
                }
            }
        }

        #region Properties

        [Inject]
        /// <inheritdoc/>
        public ILogger Logger
        {
            get { return this.mLogger; }
            set { this.mLogger = value; }
        }

        /// <inheritdoc/>
        public string ServerAddress
        {
            get { return this.mReportingService.Url; }
        }

        /// <inheritdoc/>
        public string RootPath
        {
            get { return this.mRootPath; }
            set { this.mRootPath = value; }
        }

        /// <inheritdoc/>
        public SoapHttpClientProtocol SoapClient
        {
            get => this.mReportingService;
        }


        #endregion

        #region Folder Methods

        /// <inheritdoc/>
        public FolderItem GetFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolder - path = {0}", path);

            string folderName = path;

            if (folderName != "/")
                folderName = path.Substring(path.LastIndexOf('/') + 1);

            this.mLogger.Debug("GetFolder - folderName = {0}", folderName);

            CatalogItem item = this.GetItem(
                folderName,
                path,
                FolderTypeName);

            if (item != null)
            {
                this.mLogger.Debug("GetFolder - Found item = {0}", item.Path);

                return this.PopulateFolderItem(item);
            }

            this.mLogger.Debug("GetFolder - No item found.");

            return null;
        }

        /// <inheritdoc/>
        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolders - path = {0}", path);

            List<FolderItem> folderItems = new List<FolderItem>();
            List<CatalogItem> items = this.GetItems(path, FolderTypeName);

            if (items != null && items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    this.mLogger.Debug("GetFolders - Found Item = {0}", item.Path);
                    folderItems.Add(this.PopulateFolderItem(item));
                }

                return folderItems;
            }

            this.mLogger.Debug("GetFolders - No item found.");

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<FolderItem> GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFoldersList - path = {0}", path);

            var items = this.GetItemsList<FolderItem>(path, FolderTypeName, item =>
            {
                this.mLogger.Debug("GetFoldersList - Found item = {0}", item.Path);
                return this.PopulateFolderItem(item);
            });

            if (items != null && items.Any())
                foreach (FolderItem item in items)
                    yield return item;
        }

        /// <inheritdoc/>
        public string CreateFolder(FolderItem folder)
        {
            this.mLogger.Debug("CreateFolder - name = {0}; parentPath = {1}", folder.Name, folder.ParentPath);

            try
            {
                var catalogItem = this.mReportingService.CreateFolder(folder.Name, folder.ParentPath, GetMigrationProperties(folder));
                SetPermissions(catalogItem.Path, folder);
            }
            catch (SoapException er)
            {
                this.mLogger.Error(er, "CreateFolder - Error creating folder '{0}' in '{1}'", folder.Name, folder.ParentPath);
                return er.Message;
            }

            return null;
        }

        /// <inheritdoc/>
        public string CreateFolder(string name, string parentPath)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            if (string.IsNullOrEmpty(parentPath))
                throw new ArgumentException("parentPath");

            this.mLogger.Debug("CreateFolder - name = {0}; parentPath = {1}", name, parentPath);

            try
            {
                // Go through every folder in the parentPath and create it if it does not exist
                string[] folders = parentPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                string parent = "/";

                foreach (string folder in folders)
                {
                    string sFolder;

                    if (parent != "/" && parent.EndsWith("/"))
                        parent = parent.Substring(0, parent.LastIndexOf("/"));

                    if (parent == "/")
                        sFolder = parent + folder;
                    else
                        sFolder = parent + "/" + folder;

                    if (!this.ItemExists(sFolder, FolderTypeName))
                    {
                        this.mLogger.Debug("Creating folder structure for '{0}' in '{1}'...", folder, parent);

                        // At this point we're not from a source folder, so no permissions, etc to pull from source
                        this.mReportingService.CreateFolder(folder, parent, null);
                    }

                    if (parent != "/")
                        parent += "/" + folder;
                    else
                        parent += folder;
                }

                this.mReportingService.CreateFolder(name, parentPath, null);
            }
            catch (SoapException er)
            {
                this.mLogger.Error(er, "CreateFolder - Error creating folder '{0}' in '{1}'", name, parentPath);

                return er.Message;
            }

            return null;
        }
        #endregion

        #region Report Methods
        /// <inheritdoc/>
        public byte[] GetReportDefinition(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            this.mLogger.Debug("GetReportDefinition - reportPath = {0}", reportPath);

            byte[] def = this.mReportingService.GetItemDefinition(reportPath);

            string reportDefinition = SSRSUtil.ByteArrayToString(def);
            reportDefinition = reportDefinition.CleanXml();
            def = SSRSUtil.StringToByteArray(reportDefinition);
            //this.mLogger.Debug("GetReportDefinition - Definition = {0}", reportDefinition);

            return def;
        }

        /// <inheritdoc/>
        public ReportItem GetReport(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            this.mLogger.Debug("GetReport - reportPath = {0}", reportPath);

            string reportName = reportPath.Substring(reportPath.LastIndexOf('/') + 1);

            this.mLogger.Debug("GetReport - reportName = {0}", reportName);

            CatalogItem item = this.GetItem(
                reportName,
                reportPath,
                ReportTypeName);

            if (item != null)
            {
                this.mLogger.Debug("GetReport - Found item = {0}", item.Path);

                ReportItem reportItem = PopulateReportItem(item);
                return reportItem;
            }

            this.mLogger.Debug("GetReport - No item found.");
            return null;
        }

        /// <summary>
        /// Populates the ReportItem from the service (Eg subscriptions, dependencies, datasets, etc)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private ReportItem PopulateReportItem(CatalogItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.TypeName != ReportTypeName)
            {
                throw new InvalidItemTypeException(item.Path, item.TypeName, ReportTypeName);
            }    

            var reportPath = item.Path;
            var reportItem = this.mDataMapper.GetReport(item);
            byte[] def = this.GetReportDefinition(item.Path);
            reportItem.Definition = def;

            reportItem.DataSources = this.GetReportDataSources(reportPath);
            reportItem.Properties = this.GetItemProperties(reportPath);
            reportItem.DependsOn = this.GetReportDependencies(reportPath);
            reportItem.DataSets = this.GetReportDatasets(reportPath);
            reportItem.Policies = this.GetItemPolicies(reportPath);
            reportItem.SnapshotOptions = this.GetReportHistoryOptions(reportPath);
            reportItem.Subscriptions = this.GetSubscriptions(reportPath);
            reportItem.SubReports = this.GetSubReports(SSRSUtil.ByteArrayToString(def));

            // Fix up references?
            // currently the writer responsibility?
            return reportItem;
        }

        /// <summary>
        /// Converts a Folder CatalogItem to a FolderItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private FolderItem PopulateFolderItem(CatalogItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.TypeName != FolderTypeName)
            {
               
                throw new InvalidItemTypeException(item.Path, item.TypeName, FolderTypeName);
                
            }

            // Call the data mapper for this object - this populates the base object properties
            var folder = this.mDataMapper.GetFolder(item);
            folder.Properties = this.GetItemProperties(item.Path);
            folder.Policies = this.GetItemPolicies(item.Path);

            return folder;
        }

        private DataSetItem PopulateDataSetItem(ReportServerItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.ItemType != DatasetTypeName)
            {
                throw new InvalidItemTypeException(item.Path, item.ItemType, DatasetTypeName);
            }

            var dataset = item.ToDatasetItem();


            dataset.Definition = this.mReportingService.GetItemDefinition(item.Path);
            dataset.Policies = this.GetItemPolicies(item.Path);
            dataset.Properties = this.GetItemProperties(item.Path);

            return dataset;
        }

        private DataSourceItem PopulateDataSourceItem(CatalogItem item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (item.TypeName != DataSourceTypeName)
            {

                throw new InvalidItemTypeException(item.Path, item.TypeName, DataSourceTypeName);
            }

            var definition = this.mReportingService.GetDataSourceContents(item.Path);
            var dataSource = this.mDataMapper.GetDataSource(item, definition);
            dataSource.Properties = this.GetItemProperties(item.Path);
            dataSource.Policies = this.GetItemPolicies(item.Path);

            // datasources have subscriptions...we'll ignore these for now
            return dataSource;
            
        }

        private ReportServerItem PopulateReportServerItem(CatalogItem item)
        {
            var reportServerItem = this.mDataMapper.GetReportServerItem(item);
            reportServerItem.Properties = this.GetItemProperties(item.Path);
            reportServerItem.Policies = this.GetItemPolicies(item.Path);
            return reportServerItem;

        }

        /// <inheritdoc />
        public List<ReportItem> GetReports(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReports - path = {0}", path);

            List<ReportItem> reportItems = new List<ReportItem>();
            List<CatalogItem> items = this.GetItems(path, ReportTypeName);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    this.mLogger.Debug("GetReports - Found item = {0}", item.Path);
                    var reportItem = this.PopulateReportItem(item);
                    reportItems.Add(reportItem);
                }

                return reportItems;
            }

            this.mLogger.Debug("GetReports - No item found.");

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<ReportItem> GetReportsLazy(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReportsLazy - path = {0}", path);

            var items = this.GetItemsList<ReportItemProxy>(path, ReportTypeName, r => 
            {
                this.mLogger.Debug("GetReportsLazy - Found item = {0}", r.Path);

                var report = new ReportItemProxy(this)
                {
                    Name = r.Name,
                    Path = r.Path,
                    CreatedBy = r.CreatedBy,
                    CreationDate = r.CreationDate,
                    Description = r.Description,
                    ID = r.ID,
                    ModifiedBy = r.ModifiedBy,
                    ModifiedDate = r.ModifiedDate,
                    Size = r.Size,
                    VirtualPath = r.VirtualPath
                };

                return report;
            });

            if (items != null)
                foreach (ReportItemProxy item in items)
                    yield return item;
        }

        /// <inheritdoc />
        public List<ReportItem> GetSubReports(string reportDefinition)
        {
            if (string.IsNullOrEmpty(reportDefinition))
                throw new ArgumentException("reportDefinition");

            List<ReportItem> subReports = new List<ReportItem>();

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            reportDefinition = reportDefinition.CleanXml();

            doc.LoadXml(reportDefinition);

            XmlNodeList subReportNodes = doc.GetElementsByTagName("Subreport");

            foreach (XmlNode node in subReportNodes)
            {
                foreach (XmlNode nameNode in node.ChildNodes)
                {
                    if (nameNode.Name == "ReportName")
                    {
                        string subReportPath = nameNode.InnerText;
                        int subReportNameIndex = subReportPath.LastIndexOf('/') + 1;

                        CatalogItem subReportItem = this.GetItem(
                            subReportPath.Substring(subReportNameIndex, subReportPath.Length - subReportNameIndex),
                            subReportPath,
                            ReportTypeName);

                        if (subReportItem != null)
                        {
                            ReportItem subReport = this.PopulateReportItem(subReportItem);
                            string subReportDefinition = SSRSUtil.ByteArrayToString(subReport.Definition);
                            List<ReportItem> subReportsInner = this.GetSubReports(subReportDefinition);

                            foreach (ReportItem subReportInner in subReportsInner)
                            {
                                if (!subReports.Contains(subReportInner))
                                    subReports.Add(subReportInner);
                            }

                            if (!subReports.Contains(subReport))
                            {
                                subReports.Add(subReport);
                                continue;
                            }
                        }
                    }
                }
            }
            
            return subReports;
        }

        /// <inheritdoc />
        public string[] WriteReport(string reportPath, ReportItem reportItem, bool overwrite)
        {
            var stringWarnings = new List<string>();

            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            if (reportItem == null)
                throw new ArgumentNullException("reportItem");

            if (reportItem.Definition == null)
                throw new InvalidReportDefinitionException(reportItem.Path);

            this.mLogger.Debug("WriteReport - reportPath = {0}; overwrite = {1}",
                reportPath,
                overwrite);

            Warning[] warnings;

            var item = this.mReportingService.CreateCatalogItem(ReportTypeName,
                reportItem.Name,
                reportPath,
                overwrite,
                reportItem.Definition,
                GetMigrationProperties(reportItem),
                out warnings);

            if (warnings != null)
                if (warnings.Any())
                {
                    for (int i = 0; i < warnings.Count(); i++)
                        this.mLogger.Warn("WriteReport - Code: {0}; Name: {1}; Type: {2}; Severity: {3}; Msg: {4}",
                            warnings[i].Code,
                            warnings[i].ObjectName,
                            warnings[i].ObjectType,
                            warnings[i].Severity,
                            warnings[i].Message);

                    stringWarnings.AddRange(warnings.Select(s => string.Format("{0}: {1}", s.Code, s.Message)).ToArray<string>());
                }

            

            SetPermissions(item.Path, reportItem);

            if (stringWarnings.Count > 0)
                return stringWarnings.ToArray();

            return null;

        }
        #endregion

        #region DataSource Methods
        /// <inheritdoc />
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            this.mLogger.Debug("GetDataSource - dataSourcePath = {0}", dataSourcePath);

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            this.mLogger.Debug("GetDataSource - Name = {0}", dsName);

            CatalogItem item = this.GetItem(dsName, dataSourcePath, DataSourceTypeName);

            if (item != null)
            {
                this.mLogger.Debug("GetDataSource - Found item = {0}", item.Path);
                //DataSourceDefinition def = this.mReportingService.GetDataSourceContents(item.Path);
                var dataSource = this.PopulateDataSourceItem(item);
                return dataSource;
            }

            this.mLogger.Debug("GetDataSource - No item found.");

            return null;
        }

        /// <inheritdoc />
        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            this.mLogger.Debug("GetDataSources - path = {0}", path);

            List<DataSourceItem> dataSourceItems = new List<DataSourceItem>();
            List<CatalogItem> items = this.GetItems(path, DataSourceTypeName);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    this.mLogger.Debug("GetDataSources - Found item = {0}", item.Path);
                    var dataSource = this.PopulateDataSourceItem(item);
                    dataSourceItems.Add(dataSource);
                }

                return dataSourceItems;
            }

            this.mLogger.Debug("GetDataSources - No item found.");

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<DataSourceItem> GetDataSourcesList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            this.mLogger.Debug("GetDataSourcesList - path = {0}", path);

            var items = this.GetItemsList<DataSourceItem>(path,DataSourceTypeName, ds => 
                {
                    this.mLogger.Debug("GetDataSourcesList - Found item = {0}", ds.Path);

                    var dataSource = this.PopulateDataSourceItem(ds);

                    return dataSource;
                });

            if (items != null)
                foreach (DataSourceItem item in items)
                    yield return item;
        }

        /// <inheritdoc />
        public string WriteDataSource(string dataSourcePath, DataSourceItem dataSource, bool overwrite)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException(nameof(dataSourcePath));

            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            this.mLogger.Debug("WriteDataSource - dataSourcePath = {0}; overwrite = {1}; connectString = {2}",
                dataSourcePath,
                overwrite,
                dataSource.ConnectString);

            //TODO Should IDataMapper map DataSourceItem to an SSRS DataSource?
            DataSourceDefinition def = new DataSourceDefinition();

            def.ConnectString = dataSource.ConnectString;
            def.Enabled = dataSource.Enabled;
            def.EnabledSpecified = dataSource.EnabledSpecified;
            def.Extension = dataSource.Extension;
            def.ImpersonateUser = dataSource.ImpersonateUser;
            def.ImpersonateUserSpecified = dataSource.ImpersonateUserSpecified;
            def.OriginalConnectStringExpressionBased = dataSource.OriginalConnectStringExpressionBased;
            def.Password = dataSource.Password;
            def.Prompt = dataSource.Prompt;
            def.UseOriginalConnectString = dataSource.UseOriginalConnectString;
            def.UserName = dataSource.UserName;
            def.WindowsCredentials = dataSource.WindowsCredentials;

            switch (dataSource.CredentialsRetrieval)
            {
                case "Integrated":
                    def.CredentialRetrieval = CredentialRetrievalEnum.Integrated;
                    break;
                case "None":
                    def.CredentialRetrieval = CredentialRetrievalEnum.None;
                    break;
                case "Prompt":
                    def.CredentialRetrieval = CredentialRetrievalEnum.Prompt;
                    break;
                case "Store":
                    def.CredentialRetrieval = CredentialRetrievalEnum.Store;
                    break;
                default:
                    def.CredentialRetrieval = CredentialRetrievalEnum.None;
                    break;
            }

            try
            {
                var item = this.mReportingService.CreateDataSource(dataSource.Name,
                    dataSourcePath,
                    overwrite,
                    def,
                    GetMigrationProperties(dataSource));

                SetPermissions(item.Path, dataSource);
            }
            catch (SoapException er)
            {
                this.mLogger.Error(er, "WriteDataSource - Error writing Data Source '{0}'", dataSourcePath);

                return er.Message;
            }

            return null;
        }
        #endregion

        #region Misc.
        /// <inheritdoc />
        public SqlServerInfo GetSqlServerVersion()
        {
            this.mReportingService.ServerInfoHeaderValue  = new ServerInfoHeader();

            // Make call to ReportingService2010 endpoint otherwise ServerInfoHeaderValue will be NULL.
            this.mReportingService.ListChildren("/", false);
            
            var sqlServerInfo = SSRSUtil.GetSqlServerVersion(this.mReportingService.ServerInfoHeaderValue.ReportServerVersion);

            this.mLogger.Info("SQL Server version = {0}", sqlServerInfo.FullVersion);
            this.mLogger.Info("Detected SQL Server version = {0}", sqlServerInfo.SsrsVersion);

            return sqlServerInfo;
        }
        #endregion

        #region Item Methods
        /// <inheritdoc />
        public bool ItemExists(string itemPath, string itemType)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentException("itemType");

            string actualItemType = this.mReportingService.GetItemType(itemPath);

            if (itemType.ToLower() == actualItemType.ToLower())
                return true;
            else
                return false;
        }

        /// <inheritdoc />
        public void DeleteItem(string itemPath)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            this.mReportingService.DeleteItem(itemPath);
        }

        /// <inheritdoc/>
        public CatalogItem GetItem(string itemName, string itemPath, string itemType)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException("itemName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException("itemPath");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            this.mLogger.Debug("GetItem - itemName = {0}; itemPath = {1}; itemType = {2}; rootPath = {3}",
                itemName,
                itemPath,
                itemType,
                this.mRootPath);

            SearchCondition nameCondition = new SearchCondition();
            nameCondition.Condition = ConditionEnum.Equals;
            nameCondition.ConditionSpecified = true;
            nameCondition.Name = "Name";
            nameCondition.Values = new string[] { itemName };

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[2];
            conditions[0] = nameCondition;
            conditions[1] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(this.mRootPath, BooleanOperatorEnum.And, null, conditions);

            if (items.Any())
            {
                this.mLogger.Debug("GetItem - Items found = {0}", items.Count());

                foreach (CatalogItem item in items)
                        if (item.Path == itemPath)
                            return item;
            }

            this.mLogger.Debug("GetItem - No items found");

            return null;
        }

        /// <inheritdoc/>
        public List<CatalogItem> GetItems(string path, string itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            this.mLogger.Debug("GetItems - path = {0}; itemType = {1}",
                path,
                itemType);

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
            {
                this.mLogger.Debug("GetItems - Items found = {0}", items.Count());
                return items.Select(item => item).ToList<CatalogItem>();
            }
            else
            {
                this.mLogger.Debug("GetItems - No items found");

                return null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<CatalogItem> GetItemsList(string path, string itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            this.mLogger.Debug("GetItemsList - path = {0}; itemType = {1}",
               path,
               itemType);

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
            {
                this.mLogger.Debug("GetItemsList - Items found = {0}", items.Count());

                return items.Select(item => item);
            }
            else
            {
                this.mLogger.Debug("GetItemsList - No items found");

                return null;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetItemsList<T>(string path, string itemType, Func<CatalogItem, T> itemConverter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            this.mLogger.Debug("GetItemsList2 - path = {0}; itemType = {0}",
               path,
               itemType);

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
            {
                this.mLogger.Debug("GetItemsList2 - Items found = {0}", items.Count());

                return items.Select(item => itemConverter(item));
            }
            else
            {
                this.mLogger.Debug("GetItemsList2 - No items found");

                return null;
            }
        }

        /// <inheritdoc />
        public List<ItemReferenceDefinition> GetReportDependencies(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            // Get the item types from the server
            var itemTypes = this.mReportingService.ListItemTypes();
            var items = new List<ItemReferenceDefinition>();
            foreach (var itemType in itemTypes)
            {
                try
                {
                    var references = this.mReportingService.GetItemReferences(reportPath, itemType);
                    var mappedReferences = references.Select(r => this.mDataMapper.GetItemReferenceData(r)).ToList();
                    foreach (var reference in mappedReferences)
                    {
                        // TODO: Do we need to check types, Can you have a report, dataset and data source all named "ABC?"
                        if (!items.Any(r => r.Reference == reference.Reference 
                                        && r.ReferenceType == reference.ReferenceType))
                            items.Add(reference);
                    }
                }
                catch (Exception ex)
                {
                    mLogger.Debug(ex, $"Could not '{itemType}' items for {reportPath}");
                }
            }

            return items.ToList();
        }

        /// <inheritdoc/>
        public ReportServerItem GetItem(string itemPath)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException(nameof(itemPath));

            // Recursively grab all items
            var items = this.mReportingService.FindItems("/", BooleanOperatorEnum.And,
                        new Property[] { new Property { Name = "Recursive", Value = "True" } }, null);

            var item = items.FirstOrDefault(i => i.Path == itemPath);
            if (item != null)
            {
                var reportServerItem = this.PopulateReportItem(item);
                
                return reportServerItem;
            }

            throw new Exception($"Item {itemPath} not found.");

        }

        /// <inheritdoc/>
        public ReportServerItem GetItemFromReference(ItemReferenceDefinition reference)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            var properties = this.mReportingService.GetProperties(reference.Reference, null);

            SearchCondition nameCondition = new SearchCondition();
            nameCondition.Condition = ConditionEnum.Equals;
            nameCondition.ConditionSpecified = true;
            nameCondition.Name = "Name";
            nameCondition.Values = new string[] { properties.Single(p => p.Name == "Name").Value };

            SearchCondition typeCondition = new SearchCondition()
            {
                Condition = ConditionEnum.Equals,
                ConditionSpecified = true,
                Name = "Type",
                Values = new string[] { properties.Single(p => p.Name == "Type").Value }
            };

            SearchCondition[] conditions = new SearchCondition[2];
            conditions[0] = nameCondition;
            conditions[1] = typeCondition;

            var items = this.mReportingService.FindItems("/", BooleanOperatorEnum.And,
                            new Property[] { new Property { Name = "Recursive", Value = "True" } }, conditions);

            var item = items.FirstOrDefault();

            if (item != null)
            {
                var reportServerItem = this.PopulateReportServerItem(item);
                return reportServerItem;
            }

            throw new Exception($"Item {reference.Reference} not found.");
        }

        public DataSetItem GetDataset(string itemPath, string itemName)
        {
            if (string.IsNullOrEmpty(itemPath))
            {
                throw new ArgumentException($"'{nameof(itemPath)}' cannot be null or empty.", nameof(itemPath));
            }

            if (string.IsNullOrEmpty(itemName))
            {
                throw new ArgumentException($"'{nameof(itemName)}' cannot be null or empty.", nameof(itemName));
            }

            var item = this.GetItemFromReference(new ItemReferenceDefinition { Name = itemName, Reference = itemPath, ReferenceType = DatasetTypeName });
            var dataSet = this.PopulateDataSetItem(item);
                           

            return dataSet;
        }

        public List<DataSetItem> GetReportDatasets(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            var itemsReferences = GetReportDependencies(reportPath).Where(i => i.ReferenceType == DatasetTypeName);
            var items = itemsReferences.Select(i=>this.PopulateDataSetItem(this.GetItemFromReference(i))).ToList();

            return items;        
        }

        /// <summary>
        /// Returns the data sources for the specified report
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<DataSourceItem> GetReportDataSources(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            var itemsReferences = GetReportDependencies(reportPath)
                                .Where(i => i.ReferenceType == DataSourceTypeName);
            //var itemReferences = this.mReportingService.GetItemDataSources(reportPath);
            var items = itemsReferences
                .Where(i=>i.Reference != null)
                .Select(i => this.GetDataSource(i.Reference))
                .ToList();

            return items;


        }

        public List<PolicyDefinition> GetItemPolicies(string itemPath)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException(nameof(itemPath));

            bool inheritFromParent;
            var policies = this.mReportingService.GetPolicies(itemPath, out inheritFromParent);

            return policies.Select(p => this.mDataMapper.GetPolicy(p, inheritFromParent)).ToList();
            
        }

        public SnapshotOptionsDefinition GetReportHistoryOptions(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            bool keepSnapshots;
            ReportServer2010.ScheduleDefinitionOrReference definitionOrRef;
            if (this.mReportingService.GetItemHistoryOptions(reportPath, out keepSnapshots, out definitionOrRef))
            {
                return this.mDataMapper.GetHistoryOptionsDefinition(definitionOrRef, keepSnapshots);
            }

            return null;
        }

        /// <inheritdoc />
        public List<SubscriptionDefinition> GetSubscriptions(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            var subscriptionDefinitions = new List<SubscriptionDefinition>();
            var subscriptions = this.mReportingService.ListSubscriptions(reportPath);
            
            foreach (var sub in subscriptions)
            {
                var reportSub = new SubscriptionDefinition();
                reportSub.SourceObject = sub;
                                
                ParameterValueOrFieldReference[] fieldReference = null;
                string description, status, eventType, matchData = null;
                ParameterValue[] parameterValue = null;
                ExtensionSettings extensionSettings = null;
                ActiveState activeState = null;
                DataRetrievalPlan dataRetrievalPlan = null;
                
                

                // If the subscription is data driven send it back
                if (sub.IsDataDriven)
                {
                    this.mReportingService.GetDataDrivenSubscriptionProperties(
                        sub.SubscriptionID,
                        out extensionSettings,
                        out dataRetrievalPlan,
                        out description,
                        out activeState,
                        out status,
                        out eventType,
                        out matchData,
                        out fieldReference
                        );
                }
                else
                {

                    this.mReportingService.GetSubscriptionProperties(
                        sub.SubscriptionID,
                        out extensionSettings,
                        out description,
                        out activeState,
                        out status,
                        out eventType,
                        out matchData,
                        out parameterValue
                        );
                }

                subscriptionDefinitions.Add(reportSub);

            }

            return subscriptionDefinitions;
            

        }

        /// <inheritdoc />
        public string[] WriteDataSet(string path, DataSetItem datasetItem, bool overwrite)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException(nameof(path));

            if (datasetItem == null)
                throw new ArgumentNullException(nameof(datasetItem));

            if (datasetItem.Definition == null)
                throw new InvalidDatasetDefinitionException(path);

            this.mLogger.Debug("WriteSharedDataset - path = {0}; overwrite = {1}",
               path,
               overwrite);

            Warning[] warnings;
            var item = this.mReportingService.CreateCatalogItem(DatasetTypeName,
                datasetItem.Name,
                path,
                overwrite,
                datasetItem.Definition,
                GetMigrationProperties(datasetItem), // any extra properties on a dataset... you know...description, hidden, etc?
                out warnings);

            SetPermissions(item.Path, datasetItem);

            // Might need to make handling this a bit more generic
            if (warnings != null)
                if (warnings.Any())
                {
                    for (int i = 0; i < warnings.Count(); i++)
                        this.mLogger.Warn("WriteSharedDataset - Code: {0}; Name: {1}; Type: {2}; Severity: {3}; Msg: {4}",
                            warnings[i].Code,
                            warnings[i].ObjectName,
                            warnings[i].ObjectType,
                            warnings[i].Severity,
                            warnings[i].Message);

                    return warnings.Select(s => string.Format("{0}: {1}", s.Code, s.Message)).ToArray<string>();
                }
            return null;

        }

        #endregion
        
        /// <summary>
        /// Returns the properties for the specified item path
        /// </summary>
        /// <param name="itemPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<ItemProperty> GetItemProperties(string itemPath)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException(nameof(ItemParameter));

            this.mLogger.Debug("GetProperties - itemPath = {0}",
              itemPath);
            
            var properties = this.mReportingService.GetProperties(itemPath, null);
            return properties.Select(p => new ItemProperty() { Name = p.Name, Value = p.Value }).ToList();
        }

    
        private ReportServer2010.Property[] GetMigrationProperties(ReportServerItem reportServerItem)
        {
            if (reportServerItem == null)
                throw new ArgumentNullException(nameof(reportServerItem));
            if (reportServerItem.Properties == null || reportServerItem.Properties.Count == 0)
                return null;

            return reportServerItem.Properties
                .Where(p => PropertiesToMigrate.Contains(p.Name))
                .Select(p => new ReportServer2010.Property() { Name = p.Name, Value = p.Value })
                .ToArray();
        }

        /// <inheritdoc/>
        public void SetPermissions(string itemPath, ReportServerItem sourceItem)
        {
            var policies = sourceItem.Policies
                       .Where(p => p.InheritFromParent == false)
                       .Select(p => new Policy
                       {
                           GroupUserName = p.GroupUserName,
                           Roles = p.Roles.Select(r => new Role { Name = r.Name, Description = r.Description }).ToArray()
                       })
                       .ToArray();
            if (policies.Length > 0)
            this.mReportingService.SetPolicies(itemPath, policies);
        }


        /// <summary>
        /// Updates the item reference paths
        /// </summary>
        /// <param name="itemPath"></param>
        /// <param name="sourceRoot"></param>
        /// <param name="destinationRoot"></param>
        public void UpdateItemReferences(string itemPath, string sourceRoot, string destinationRoot)
        {
            var types = new string[] { DatasetTypeName, DataSourceTypeName }; // reports?
            var itemReferences = new List<ItemReference>();

            foreach (var t in types)
            {
                var referenceData = this.mReportingService.GetItemReferences(itemPath, t);
                
                foreach (var r in referenceData)
                {
                    var itemReference = new ItemReference { Name = r.Name };
                    if (string.IsNullOrEmpty(r.Reference))
                    {
                        // Find the item from the destination root
                        // Eg Shared Data Source References
                        var relatedItem = this.FindItem(destinationRoot, r.Name, t);
                        if (relatedItem != null)
                            itemReference.Reference = relatedItem.Path;
                    }
                    else
                    {
                        // Remap
                        if (!this.ItemExists(r.Reference, t))
                        {
                            itemReference.Reference = SSRSUtil.GetFullDestinationPathForItem(
                                sourceRoot,
                                destinationRoot,
                                r.Reference);
                        }
                    }

                    // Only include referenced items, and none that we might already have
                    if (!string.IsNullOrEmpty(itemReference.Reference) && !itemReferences.Any(n=>n.Name == itemReference.Name))
                        itemReferences.Add(itemReference);

                }
               
            }

            if (itemReferences.Count > 0)
                mReportingService.SetItemReferences(itemPath, itemReferences.ToArray());

        }

        private CatalogItem FindItem(string rootPath, string itemName, string itemType)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException("itemName");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            this.mLogger.Debug("GetItem - itemName = {0}; itemType = {1}; rootPath = {2}",
                itemName,
                itemType,
                rootPath);

            SearchCondition nameCondition = new SearchCondition();
            nameCondition.Condition = ConditionEnum.Equals;
            nameCondition.ConditionSpecified = true;
            nameCondition.Name = "Name";
            nameCondition.Values = new string[] { itemName };

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[2];
            conditions[0] = nameCondition;
            conditions[1] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(rootPath, BooleanOperatorEnum.And, null, conditions);

            if (items.Any())
            {
                this.mLogger.Debug("GetItem - Items found = {0}", items.Count());

                return items.First();
            }

            this.mLogger.Debug("GetItem - No items found");

            return null;
        }


        public byte[] UpdateDefinitionReferences(byte[] reportDefinition, string root)
        {
            var doc = new XmlDocument();
            UTF8Encoding decoder = new UTF8Encoding();
            string reportDefinitionString = decoder.GetString(reportDefinition);

            if (reportDefinitionString.Substring(0, 1) != "<")
                reportDefinitionString = reportDefinitionString.Substring(1, reportDefinitionString.Length - 1);

            doc.LoadXml(reportDefinitionString);

            // Update DataSourceReference tag to new path
            XmlNodeList dataSourceNodes = doc.GetElementsByTagName("DataSourceReference");

            for (int i = 0; i < dataSourceNodes.Count; i++)
            {
                // Search the report server for the reference
                var item = this.FindItem(root, dataSourceNodes[i].InnerText, DataSourceTypeName);
                if (item != null)
                {
                    dataSourceNodes[i].InnerText = item.Path;
                }
            }

            // Update DataSourceReference tag to new path
            XmlNodeList dataSetNodes = doc.GetElementsByTagName("SharedDataSetReference");

            for (int i = 0; i < dataSetNodes.Count; i++)
            {
                // Search the report server for the reference
                var item = this.FindItem(root, dataSetNodes[i].InnerText, DatasetTypeName);
                if (item != null)
                {
                    dataSetNodes[i].InnerText = item.Path;
                }
            }

            

            return Encoding.UTF8.GetBytes(doc.OuterXml);

        }
    }
}
