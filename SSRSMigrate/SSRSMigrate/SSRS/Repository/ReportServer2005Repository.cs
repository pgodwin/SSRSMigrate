using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Extensions.Logging;
using SSRSMigrate.Enum;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.Utility;
using System.Xml;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.DataMapper;
using System.Web.Services.Protocols;
using SSRSMigrate.SSRS.Errors;
using Ninject;

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2005Repository : IReportServerRepository
    {
        private readonly ReportingService2005 mReportingService;
        private readonly ReportingService2005DataMapper mDataMapper;
        private ILogger mLogger = null;
        private string mRootPath = null;
        private string mInvalidPathChars = ":?;@&=+$,\\*><|.\"";
        private int mPathMaxLength = 260;

        public ReportServer2005Repository(string rootPath,
            ReportingService2005 reportingService,
            ReportingService2005DataMapper dataMapper)
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

        ~ReportServer2005Repository()
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
                ItemTypeEnum.Folder);

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
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.Folder);

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

            var items = this.GetItemsList<FolderItem>(path, ItemTypeEnum.Folder, folder =>
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

            byte[] def = this.mReportingService.GetReportDefinition(reportPath);

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
                ItemTypeEnum.Report);

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
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.Report);

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

            var items = this.GetItemsList<ReportItem>(path, ItemTypeEnum.Report, r => 
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
                            ItemTypeEnum.Report);

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

            Warning[] warnings = this.mReportingService.CreateReport(reportItem.Name,
                reportPath,
                overwrite,
                reportItem.Definition,
                null);

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

        #region Data Source Methods
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            this.mLogger.Debug("GetDataSource - dataSourcePath = {0}", dataSourcePath);

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            this.mLogger.Debug("GetDataSource - Name = {0}", dsName);

            CatalogItem item = this.GetItem(dsName, dataSourcePath, ItemTypeEnum.DataSource);

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
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.DataSource);

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

            var items = this.GetItemsList<DataSourceItem>(path, ItemTypeEnum.DataSource, ds => 
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
            
            switch(dataSource.CredentialsRetrieval)
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
                isValidPath =  false;
            else if (path.Length > this.mPathMaxLength)
                isValidPath =  false;
            else
                isValidPath = true;

            this.mLogger.Debug("ValidatePath - isValidPath = {0}", isValidPath);

            return isValidPath;
        }

        public SSRSVersion GetSqlServerVersion()
        {
            this.mReportingService.ListSecureMethods();

            return SSRSUtil.GetSqlServerVersion(this.mReportingService.ServerInfoHeaderValue.ReportServerVersion);
        }
        #endregion

        #region Item Methods
        private ItemTypeEnum TypeNameToTypeEnum(string itemType)
        {
            ItemTypeEnum type = ItemTypeEnum.Unknown;

            switch (itemType.ToLower())
            {
                case "unknown":
                    type = ItemTypeEnum.Unknown; break;
                case "datasource":
                    type = ItemTypeEnum.DataSource; break;
                case "folder":
                    type = ItemTypeEnum.Folder; break;
                case "linkedreport":
                    type = ItemTypeEnum.LinkedReport; break;
                case "model":
                    type = ItemTypeEnum.Model; break;
                case "report":
                    type = ItemTypeEnum.Report; break;
                case "resource":
                    type = ItemTypeEnum.Resource; break;
                default:
                    type = ItemTypeEnum.Unknown; break;
            };

            return type;
        }

        public bool ItemExists(string itemPath, string itemType)
        {
            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentException("itemType");

            ItemTypeEnum actualItemType = this.mReportingService.GetItemType(itemPath);

            if (this.TypeNameToTypeEnum(itemType) == actualItemType)
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

        //TODO Need to decide if this should always FindItems in '/' or the root path
        public CatalogItem GetItem(string itemName, string itemPath, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException("itemName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException("itemPath");

            this.mLogger.Debug("GetItem - itemName = {0}; itemPath = {1}; itemType = {2}; rootPath = {3}",
                itemName, 
                itemPath, 
                itemType, 
                this.mRootPath);

            SearchCondition nameCondition = new SearchCondition();
            nameCondition.Condition = ConditionEnum.Equals;
            nameCondition.ConditionSpecified = true;
            nameCondition.Name = "Name";
            nameCondition.Value = itemName;

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = nameCondition;

            CatalogItem[] items = this.mReportingService.FindItems(this.mRootPath, BooleanOperatorEnum.And, conditions);

            if (items.Any())
            {
                this.mLogger.Debug("GetItem - Items found = {0}", items.Count());

                foreach (CatalogItem item in items)
                    if (item.Type == itemType)
                        if (item.Path == itemPath)
                            return item;
            }

            this.mLogger.Debug("GetItem - No items found");

            return null;
        }

        public List<CatalogItem> GetItems(string path, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetItems - path = {0}; itemType = {1}", 
                path,
                itemType);

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
            {
                this.mLogger.Debug("GetItems - Items found = {0}", items.Count());

                return items.Where(item => item.Type == itemType).Select(item => item).ToList<CatalogItem>();
            }
            else
            {
                this.mLogger.Debug("GetItems - No items found");

                return null;
            }
        }

        public IEnumerable<CatalogItem> GetItemsList(string path, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetItemsList - path = {0}; itemType = {1}",
                path,
                itemType);

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
            {
                int found = items.Where(item => item.Type == itemType).Select(item => item).Count();
                this.mLogger.Debug("GetItemsList - Items found = {0}", found);

                return items.Where(item => item.Type == itemType).Select(item => item);
            }
            else
            {
                this.mLogger.Debug("GetItemsList - No items found");

                return null;
            }
        }

        public IEnumerable<T> GetItemsList<T>(string path, ItemTypeEnum itemType, Func<CatalogItem, T> itemConverter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetItemsList2 - path = {0}; itemType = {0}",
               path,
               itemType);

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
            {
                int found = items.Where(item => item.Type == itemType).Select(item => item).Count();
                this.mLogger.Debug("GetItemsList2 - Items found = {0}", found);

                return items.Where(item => item.Type == itemType).Select(item => itemConverter(item));
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
