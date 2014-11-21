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

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2010Repository : IReportServerRepository
    {
        private readonly ReportingService2010 mReportingService;
        private readonly ReportingService2010DataMapper mDataMapper;
        private ILogger mLogger = null;
        private string mRootPath = null;
        private string mInvalidPathChars = ":?;@&=+$,\\*><|.\"";
        private int mPathMaxLength = 260;

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
        public string InvalidPathChars
        {
            get { return this.mInvalidPathChars; }
        }

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
        #endregion

        #region Folder Methods

        public FolderItem GetFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolder - path = {0}", path);

            string folderName = path.Substring(path.LastIndexOf('/') + 1);

            this.mLogger.Debug("GetFolder - folderName = {0}", folderName);

            CatalogItem item = this.GetItem(
                folderName,
                path,
                "Folder");

            if (item != null)
            {
                this.mLogger.Debug("GetFolder - Found item = {0}", item.Path);

                return this.mDataMapper.GetFolder(item);
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

            if (items.Any())
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

            if (items.Any())
                foreach (FolderItem item in items)
                    yield return item;
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

            this.mLogger.Debug("GetReportDefinition - Definition = {0}", reportDefinition);

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

                byte[] def = this.GetReportDefinition(item.Path);
                return this.mDataMapper.GetReport(item, def);
            }

            this.mLogger.Debug("GetReport - No item found.");

            return null;
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
                    reportItems.Add(this.mDataMapper.GetReport(item, def));
                }

                return reportItems;
            }

            this.mLogger.Debug("GetReports - No item found.");

            return null;
        }

        public IEnumerable<ReportItem> GetReportsList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReports2 - path = {0}", path);

            var items = this.GetItemsList<ReportItem>(path, "Report", r => 
                {
                    this.mLogger.Debug("GetReports2 - Found item = {0}", r.Path);

                    byte[] def = this.GetReportDefinition(r.Path);
                    return this.mDataMapper.GetReport(r, def);
                });

            if (items != null)
                foreach (ReportItem item in items)
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
                            ReportItem subReport = this.mDataMapper.GetReport(subReportItem, def);

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

            return subReports;
        }

        public string[] WriteReport(string reportPath, ReportItem reportItem, bool overwrite)
        {
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

            this.mReportingService.CreateCatalogItem("Report",
                reportItem.Name,
                reportPath,
                overwrite,
                reportItem.Definition,
                null,
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

                    return warnings.Select(s => string.Format("{0}: {1}", s.Code, s.Message)).ToArray<string>();
                }

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
                return this.mDataMapper.GetDataSource(item, def);
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
                    dataSourceItems.Add(this.mDataMapper.GetDataSource(item, def));
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
                    return this.mDataMapper.GetDataSource(ds, def);
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
                this.mReportingService.CreateDataSource(dataSource.Name,
                    dataSourcePath,
                    overwrite,
                    def,
                    null);
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
        public bool ValidatePath(string path)
        {
            bool isValidPath = true;

            this.mLogger.Debug("ValidatePath - path = {0}", path);

            if (string.IsNullOrEmpty(path))
                isValidPath = false;
            else if (path.IndexOfAny(this.mInvalidPathChars.ToCharArray()) >= 0)
                isValidPath = false;
            else if (path.Length > this.mPathMaxLength)
                isValidPath = false;
            else
                isValidPath = true;

            this.mLogger.Debug("ValidatePath - isValidPath = {0}", isValidPath);

            return isValidPath;
        }

        public SSRSVersion GetSqlServerVersion()
        {
            this.mReportingService.ServerInfoHeaderValue  = new ServerInfoHeader();

            // Make call to ReportingService2010 endpoint otherwise ServerInfoHeaderValue will be NULL.
            this.mReportingService.ListChildren("/", false);
            
            return SSRSUtil.GetSqlServerVersion(this.mReportingService.ServerInfoHeaderValue.ReportServerVersion);
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
        #endregion
    }
}
