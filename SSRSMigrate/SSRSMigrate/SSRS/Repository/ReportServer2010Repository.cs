﻿using System;
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
                throw new ArgumentException("rootPath");

            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            if (dataMapper == null)
                throw new ArgumentNullException("dataMapper");

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
        public ILogger Logger
        {
            get { return this.mLogger; }
            set { this.mLogger = value; }
        }

        public string ServerAddress
        {
            get { return this.mReportingService.Url; }
        }

        public string RootPath
        {
            get { return this.mRootPath; }
            set { this.mRootPath = value; }
        }

        public SoapHttpClientProtocol SoapClient
        {
            get => this.mReportingService;
        }

     
        #endregion

        #region Folder Methods

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
                "Folder");

            if (item != null)
            {
                this.mLogger.Debug("GetFolder - Found item = {0}", item.Path);

                var folder = this.mDataMapper.GetFolder(item);
                folder.Properties = this.GetItemProperties(item.Path);
                return folder;
            }

            this.mLogger.Debug("GetFolder - No item found.");

            return null;
        }

        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolders - path = {0}", path);

            List<FolderItem> folderItems = new List<FolderItem>();
            List<CatalogItem> items = this.GetItems(path, "Folder");

            if (items != null && items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    this.mLogger.Debug("GetFolders - Found Item = {0}", item.Path);

                    folderItems.Add(this.mDataMapper.GetFolder(item));
                }

                return folderItems;
            }

            this.mLogger.Debug("GetFolders - No item found.");

            return null;
        }

        public IEnumerable<FolderItem> GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFoldersList - path = {0}", path);

            var items = this.GetItemsList<FolderItem>(path, "Folder", folder =>
            {
                this.mLogger.Debug("GetFoldersList - Found item = {0}", folder.Path);

                return this.mDataMapper.GetFolder(folder);
            });

            if (items != null && items.Any())
                foreach (FolderItem item in items)
                    yield return item;
        }

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

                    if (!this.ItemExists(sFolder, "Folder"))
                    {
                        this.mLogger.Debug("Creating folder structure for '{0}' in '{1}'...", folder, parent);

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
        public byte[] GetReportDefinition(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            this.mLogger.Debug("GetReportDefinition - reportPath = {0}", reportPath);

            byte[] def = this.mReportingService.GetItemDefinition(reportPath);

            string reportDefinition = SSRSUtil.ByteArrayToString(def);
            if (reportDefinition.Substring(0, 1) != "<")
                reportDefinition = reportDefinition.Substring(1, reportDefinition.Length - 1);

            def = SSRSUtil.StringToByteArray(reportDefinition);

            //this.mLogger.Debug("GetReportDefinition - Definition = {0}", reportDefinition);

            return def;
        }

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
                "Report");

            if (item != null)
            {
                this.mLogger.Debug("GetReport - Found item = {0}", item.Path);

                ReportItem reportItem = PopulateReportItem(item);

                return reportItem;
            }

            this.mLogger.Debug("GetReport - No item found.");

            return null;
        }

        private ReportItem PopulateReportItem(CatalogItem item)
        {
            var reportPath = item.Path;
            byte[] def = this.GetReportDefinition(item.Path);
            var reportItem = this.mDataMapper.GetReport(item);
            reportItem.Definition = def;

            reportItem.Properties = this.GetItemProperties(reportPath);
            reportItem.DependsOn = this.GetReportDependencies(reportPath);
            reportItem.DataSets = this.GetReportDatasets(reportPath);
            reportItem.Policies = this.GetItemPolicies(reportPath);
            reportItem.SnapshotOptions = this.GetReportHistoryOptions(reportPath);
            reportItem.Subscriptions = this.GetSubscriptions(reportPath);
            reportItem.SubReports = this.GetSubReports(SSRSUtil.ByteArrayToString(def));
            reportItem.DataSources = this.GetReportDataSources(reportPath);

            return reportItem;
        }

        public List<ReportItem> GetReports(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReports - path = {0}", path);

            List<ReportItem> reportItems = new List<ReportItem>();
            List<CatalogItem> items = this.GetItems(path, "Report");

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    this.mLogger.Debug("GetReports - Found item = {0}", item.Path);

                    byte[] def = this.GetReportDefinition(item.Path);
                    var reportItem = this.mDataMapper.GetReport(item);
                    reportItem.Definition = def;

                    reportItems.Add(reportItem);
                }

                return reportItems;
            }

            this.mLogger.Debug("GetReports - No item found.");

            return null;
        }

        public IEnumerable<ReportItem> GetReportsLazy(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReportsLazy - path = {0}", path);

            var items = this.GetItemsList<ReportItemProxy>(path, "Report", r => 
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

        public List<ReportItem> GetSubReports(string reportDefinition)
        {
            if (string.IsNullOrEmpty(reportDefinition))
                throw new ArgumentException("reportDefinition");

            List<ReportItem> subReports = new List<ReportItem>();

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            if (reportDefinition.Substring(0, 1) != "<")
                reportDefinition = reportDefinition.Substring(1, reportDefinition.Length - 1);

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
                            "Report");

                        if (subReportItem != null)
                        {
                            byte[] def = this.GetReportDefinition(subReportItem.Path);
                            ReportItem subReport = this.mDataMapper.GetReport(subReportItem);
                            subReport.Definition = def;

                            string subReportDefinition = SSRSUtil.ByteArrayToString(subReport.Definition);

                            List<ReportItem> subReportsInner = this.GetSubReports(subReportDefinition);

                            foreach (ReportItem subReportInner in subReportsInner)
                                if (!subReports.Contains(subReportInner))
                                    subReports.Add(subReportInner);

                            if (!subReports.Contains(subReport))
                            {
                                subReports.Add(subReport);
                                continue;
                            }
                        }
                    }
                }
            }
            // Set the Properties for each Sub-Report
            foreach (var report in subReports)
                this.SetReportServerItemProperties(report);
            return subReports;
        }

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

            var item = this.mReportingService.CreateCatalogItem("Report",
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
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            this.mLogger.Debug("GetDataSource - dataSourcePath = {0}", dataSourcePath);

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            this.mLogger.Debug("GetDataSource - Name = {0}", dsName);

            CatalogItem item = this.GetItem(dsName, dataSourcePath, "DataSource");

            if (item != null)
            {
                this.mLogger.Debug("GetDataSource - Found item = {0}", item.Path);

                DataSourceDefinition def = this.mReportingService.GetDataSourceContents(item.Path);
                var mappedDataSource = this.mDataMapper.GetDataSource(item, def);
                this.SetReportServerItemProperties(mappedDataSource);
                return mappedDataSource;
            }

            this.mLogger.Debug("GetDataSource - No item found.");

            return null;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            this.mLogger.Debug("GetDataSources - path = {0}", path);

            List<DataSourceItem> dataSourceItems = new List<DataSourceItem>();
            List<CatalogItem> items = this.GetItems(path, "DataSource");

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    this.mLogger.Debug("GetDataSources - Found item = {0}", item.Path);

                    DataSourceDefinition def = this.mReportingService.GetDataSourceContents(item.Path);
                    //dataSourceItems.Add(this.mDataMapper.GetDataSource(item, def));
                    var dataSource = this.mDataMapper.GetDataSource(item, def);
                    dataSource.Properties = this.GetItemProperties(item.Path);
                    dataSource.Policies = this.GetItemPolicies(item.Path);
                    dataSourceItems.Add(dataSource);
                }

                return dataSourceItems;
            }

            this.mLogger.Debug("GetDataSources - No item found.");

            return null;
        }

        public IEnumerable<DataSourceItem> GetDataSourcesList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            this.mLogger.Debug("GetDataSourcesList - path = {0}", path);

            var items = this.GetItemsList<DataSourceItem>(path,"DataSource", ds => 
                {
                    this.mLogger.Debug("GetDataSourcesList - Found item = {0}", ds.Path);

                    DataSourceDefinition def = this.mReportingService.GetDataSourceContents(ds.Path);
                    var dataSource = this.mDataMapper.GetDataSource(ds, def);
                    dataSource.Properties = this.GetItemProperties(ds.Path);
                    dataSource.Policies = this.GetItemPolicies(ds.Path);
                    
                    return dataSource;
                });

            if (items != null)
                foreach (DataSourceItem item in items)
                    yield return item;
        }

        public string WriteDataSource(string dataSourcePath, DataSourceItem dataSource, bool overwrite)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

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

        public void DeleteItem(string itemPath)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            this.mReportingService.DeleteItem(itemPath);
        }

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
                var reportServerItem = this.mDataMapper.GetReportServerItem(item);
                this.SetReportServerItemProperties(reportServerItem);
                return reportServerItem;
            }

            throw new Exception($"Item {itemPath} not found.");

        }

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
                var reportServerItem = this.mDataMapper.GetReportServerItem(item);
                this.SetReportServerItemProperties(reportServerItem);
                return reportServerItem;
            }

            throw new Exception($"Item {reference.Reference} not found.");
        }

        public DatasetItem GetDataset(string itemPath, string itemName)
        {
            if (string.IsNullOrEmpty(itemPath))
            {
                throw new ArgumentException($"'{nameof(itemPath)}' cannot be null or empty.", nameof(itemPath));
            }

            if (string.IsNullOrEmpty(itemName))
            {
                throw new ArgumentException($"'{nameof(itemName)}' cannot be null or empty.", nameof(itemName));
            }


            var item = this.GetItemFromReference(new ItemReferenceDefinition { Name = itemName, Reference = itemPath, ReferenceType = "DataSet" })
                            .ToDatasetItem();

            
            item.Definition = this.mReportingService.GetItemDefinition(item.Path);
            item.Policies = this.GetItemPolicies(item.Path);
            item.Properties = this.GetItemProperties(item.Path);

            return item;
        }

        public List<DatasetItem> GetReportDatasets(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            var itemsReferences = GetReportDependencies(reportPath).Where(i => i.ReferenceType == "DataSet");
            var items = itemsReferences.Select(i=>this.GetItemFromReference(i).ToDatasetItem()).ToList();

            foreach (var item in items)
            {
                item.Definition = this.mReportingService.GetItemDefinition(item.Path);
                item.Policies = this.GetItemPolicies(item.Path);
                item.Properties = this.GetItemProperties(item.Path);
            }


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

            var itemsReferences = GetReportDependencies(reportPath).Where(i => i.ReferenceType == "DataSource");
            var items = itemsReferences.Select(i => this.GetDataSource(i.Reference)).ToList();

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

        public HistoryOptionsDefinition GetReportHistoryOptions(string reportPath)
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
        public List<ReportSubscriptionDefinition> GetSubscriptions(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentNullException(nameof(reportPath));

            var subscriptionDefinitions = new List<ReportSubscriptionDefinition>();
            var subscriptions = this.mReportingService.ListSubscriptions(reportPath);
            
            foreach (var sub in subscriptions)
            {
                var reportSub = new ReportSubscriptionDefinition();
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
        public string[] WriteDataSet(string path, DatasetItem datasetItem, bool overwrite)
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
            var item = this.mReportingService.CreateCatalogItem("DataSet",
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

        private void SetReportServerItemProperties(ReportServerItem sourceItem)
        {
            if (sourceItem == null)
                throw new ArgumentNullException(nameof(sourceItem));

            sourceItem.Properties = this.GetItemProperties(sourceItem.Path);
            sourceItem.Policies = this.GetItemPolicies(sourceItem.Path);
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

        private void SetPermissions(string itemPath, ReportServerItem item)
        {
            var policies = item.Policies
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

        
    }
}
