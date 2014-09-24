using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Utility;
using System.Xml;
using SSRSMigrate.DataMapper;
using System.Web.Services.Protocols;
using SSRSMigrate.SSRS.Errors;
using Ninject;
using Ninject.Extensions.Logging;

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
        #endregion

        #region Folder Methods

        public FolderItem GetFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            string folderName = path.Substring(path.LastIndexOf('/') + 1);

            CatalogItem item = this.GetItem(
                folderName,
                path,
                "Folder");

            if (item != null)
                return this.mDataMapper.GetFolder(item);

            return null;
        }

        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            List<FolderItem> folderItems = new List<FolderItem>();
            List<CatalogItem> items = this.GetItems(path, "Folder");

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

            var items = this.GetItemsList<FolderItem>(path, "Folder", folder => this.mDataMapper.GetFolder(folder));
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

            byte[] def = this.mReportingService.GetItemDefinition(reportPath);

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
                "Report");

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
            List<CatalogItem> items = this.GetItems(path, "Report");

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

            var items = this.GetItemsList<ReportItem>(path, "Report", r => 
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
                    return warnings.Select(s => string.Format("{0}: {1}", s.Code, s.Message)).ToArray<string>();

            return null;
        }
        #endregion

        #region DataSource Methods
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            CatalogItem item = this.GetItem(dsName, dataSourcePath, "DataSource");

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
            List<CatalogItem> items = this.GetItems(path, "DataSource");

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

            var items = this.GetItemsList<DataSourceItem>(path,"DataSource", ds => 
                {
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

        public CatalogItem GetItem(string itemName, string itemPath, string itemType)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException("itemName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException("itemPath");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

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
                foreach (CatalogItem item in items)
                        if (item.Path == itemPath)
                            return item;
            }

            return null;
        }

        public List<CatalogItem> GetItems(string path, string itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
                return items.Select(item => item).ToList<CatalogItem>();
            else
                return null;
        }

        public IEnumerable<CatalogItem> GetItemsList(string path, string itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
                return items.Select(item => item);
            else
                return null;
        }

        public IEnumerable<T> GetItemsList<T>(string path, string itemType, Func<CatalogItem, T> itemConverter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentNullException("itemType");

            SearchCondition typeCondition = new SearchCondition();
            typeCondition.Condition = ConditionEnum.Equals;
            typeCondition.ConditionSpecified = true;
            typeCondition.Name = "Type";
            typeCondition.Values = new string[] { itemType };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
                return items.Select(item => itemConverter(item));
            else
                return null;
        }
        #endregion
    }
}
