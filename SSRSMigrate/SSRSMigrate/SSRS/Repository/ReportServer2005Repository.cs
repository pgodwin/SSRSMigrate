using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.Utility;
using System.Xml;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.DataMapper;
using System.Web.Services.Protocols;

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2005Repository : IReportServerRepository
    {
        private readonly ReportingService2005 mReportingService;
        private readonly ReportingService2005DataMapper mDataMapper;
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
        #endregion

        #region Folder Methods
        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            List<FolderItem> folderItems = new List<FolderItem>();
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.Folder);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                    folderItems.Add(this.mDataMapper.GetFolder(item));

                return folderItems;
            }

            return null;
        }

        public IEnumerable<FolderItem> GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            var items = this.GetItemsList<FolderItem>(path, ItemTypeEnum.Folder, folder => this.mDataMapper.GetFolder(folder));
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

            try
            {
                this.mReportingService.CreateFolder(name, parentPath, null);
            }
            catch (SoapException er)
            {
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

            byte[] def = this.mReportingService.GetReportDefinition(reportPath);

            string reportDefinition = SSRSUtil.ByteArrayToString(def);
            if (reportDefinition.Substring(0, 1) != "<")
                reportDefinition = reportDefinition.Substring(1, reportDefinition.Length - 1);

            def = SSRSUtil.StringToByteArray(reportDefinition);

            return def;
        }

        public ReportItem GetReport(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            string reportName = reportPath.Substring(reportPath.LastIndexOf('/') + 1);

            CatalogItem item = this.GetItem(
                reportName,
                reportPath,
                ItemTypeEnum.Report);

            if (item != null)
            {
                byte[] def = this.GetReportDefinition(item.Path);
                return this.mDataMapper.GetReport(item, def);
            }
                
            return null;
        }

        public List<ReportItem> GetReports(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            List<ReportItem> reportItems = new List<ReportItem>();
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.Report);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    byte[] def = this.GetReportDefinition(item.Path);
                    reportItems.Add(this.mDataMapper.GetReport(item, def));
                }

                return reportItems;
            }

            return null;
        }

        public IEnumerable<ReportItem> GetReportsList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            var items = this.GetItemsList<ReportItem>(path, ItemTypeEnum.Report, r => 
                {
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

        public string[] WriteReport(string reportPath, ReportItem reportItem)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Data Source Methods
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            CatalogItem item = this.GetItem(dsName, dataSourcePath, ItemTypeEnum.DataSource);

            if (item != null)
            {
                DataSourceDefinition def = this.mReportingService.GetDataSourceContents(item.Path);
                return this.mDataMapper.GetDataSource(item, def);
            }

            return null;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            List<DataSourceItem> dataSourceItems = new List<DataSourceItem>();
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.DataSource);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    DataSourceDefinition def = this.mReportingService.GetDataSourceContents(item.Path);
                    dataSourceItems.Add(this.mDataMapper.GetDataSource(item, def));
                }

                return dataSourceItems;
            }

            return null;
        }

        public IEnumerable<DataSourceItem> GetDataSourcesList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var items = this.GetItemsList<DataSourceItem>(path, ItemTypeEnum.DataSource, ds => 
                {
                    DataSourceDefinition def = this.mReportingService.GetDataSourceContents(ds.Path);
                    return this.mDataMapper.GetDataSource(ds, def);
                });

            if (items != null)
                foreach (DataSourceItem item in items)
                    yield return item;
        }

        public string WriteDataSource(string dataSourcePath, DataSourceItem dataSource)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

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
                    true, //TODO Should probably have a method parameter to specify this
                    def,
                    null);
            }
            catch (SoapException er)
            {
                return er.Message;
            }

            return null;
        }
        #endregion

        #region Misc.
        public bool ValidatePath(string path)
        {
            //if (string.IsNullOrEmpty(path))
            //    throw new ArgumentException("path");

            //return path.IndexOfAny(this.mInvalidPathChars.ToCharArray()) < 0;

            if (string.IsNullOrEmpty(path))
                return false;
            else if (path.IndexOfAny(this.mInvalidPathChars.ToCharArray()) >= 0)
                return false;
            else if (path.Length > this.mPathMaxLength)
                return false;
            else
                return true;
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

        public CatalogItem GetItem(string itemName, string itemPath, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException("itemName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException("itemPath");

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
                foreach (CatalogItem item in items)
                    if (item.Type == itemType)
                        if (item.Path == itemPath)
                            return item;
            }

            return null;
        }

        public List<CatalogItem> GetItems(string path, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
                return items.Where(item => item.Type == itemType).Select(item => item).ToList<CatalogItem>();
            else
                return null;
        }

        public IEnumerable<CatalogItem> GetItemsList(string path, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
                return items.Where(item => item.Type == itemType).Select(item => item);
            else
                return null;
        }

        public IEnumerable<T> GetItemsList<T>(string path, ItemTypeEnum itemType, Func<CatalogItem, T> itemConverter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
                return items.Where(item => item.Type == itemType).Select(item => itemConverter(item));
            else
                return null;
        }
        #endregion
    }
}
