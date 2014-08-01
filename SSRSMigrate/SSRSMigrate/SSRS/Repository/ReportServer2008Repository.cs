using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.Utility;
using System.Xml;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2008Repository : IReportServerRepository
    {
        private ReportingService2005 mReportingService;
        private string mRootPath = null;

        public ReportServer2008Repository(string rootPath, ReportingService2005 reportingService)
        {
            if (string.IsNullOrEmpty(rootPath))
                throw new ArgumentException("rootPath");

            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            this.mRootPath = rootPath;
            this.mReportingService = reportingService;
        }

        ~ReportServer2008Repository()
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
                    this.mReportingService = null;
                }
            }
        }

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
                    folderItems.Add(CatalogItemToFolderItem(item));

                return folderItems;
            }

            return null;
        }

        public IEnumerable<FolderItem> GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            var items = this.GetItemsList<FolderItem>(path, ItemTypeEnum.Folder, folder => CatalogItemToFolderItem(folder));
            if (items.Any())
                foreach (FolderItem item in items)
                    yield return item;
        }

        public string CreateFolder(string folderPath)
        {
            throw new NotImplementedException();
        }

        private FolderItem CatalogItemToFolderItem(CatalogItem item)
        {
            FolderItem folder = new FolderItem();

            folder.CreatedBy = item.CreatedBy;
            folder.CreationDate = item.CreationDate;
            folder.Description = item.Description;
            folder.ID = item.ID;
            folder.ModifiedBy = item.ModifiedBy;
            folder.ModifiedDate = item.ModifiedDate;
            folder.Name = item.Name;
            folder.Path = item.Path;
            folder.Size = item.Size;
            folder.VirtualPath = item.VirtualPath;

            return folder;
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
                return CatalogItemToReportItem(item);
                
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
                    reportItems.Add(CatalogItemToReportItem(item));

                return reportItems;
            }

            return null;
        }

        public IEnumerable<ReportItem> GetReportsList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            var items = this.GetItemsList<ReportItem>(path, ItemTypeEnum.Report, r => CatalogItemToReportItem(r));

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
                            ReportItem subReport = CatalogItemToReportItem(subReportItem);

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

        private ReportItem CatalogItemToReportItem(CatalogItem item)
        {
            ReportItem report = new ReportItem();

            report.Name = item.Name;
            report.Path = item.Path;
            report.CreatedBy = item.CreatedBy;
            report.CreationDate = item.CreationDate;
            report.Description = item.Description;
            report.ID = item.ID;
            report.ModifiedBy = item.ModifiedBy;
            report.ModifiedDate = item.ModifiedDate;
            report.Size = item.Size;
            report.VirtualPath = item.VirtualPath;

            report.Definition = GetReportDefinition(item.Path);
            report.SubReports.AddRange(GetSubReports(SSRSUtil.ByteArrayToString(report.Definition)));

            return report;
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
                return CatalogItemToDataSourceItem(item);

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
                    dataSourceItems.Add(CatalogItemToDataSourceItem(item));

                return dataSourceItems;
            }

            return null;
        }

        public IEnumerable<DataSourceItem> GetDataSourcesList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var items = this.GetItemsList<DataSourceItem>(path, ItemTypeEnum.DataSource, ds => CatalogItemToDataSourceItem(ds));

            if (items != null)
                foreach (DataSourceItem item in items)
                    yield return item;
        }

        public string[] WriteDataSource(string dataSourcePath, DataSourceItem dataSource)
        {
            throw new NotImplementedException();
        }

        private DataSourceItem CatalogItemToDataSourceItem(CatalogItem item)
        {
            DataSourceItem ds = new DataSourceItem();
            DataSourceDefinition dsDef = this.mReportingService.GetDataSourceContents(item.Path);

            ds.Name = item.Name;
            ds.Path = item.Path;
            ds.CreatedBy = item.CreatedBy;
            ds.CreationDate = item.CreationDate;
            ds.Description = item.Description;
            ds.ID = item.ID;
            ds.ModifiedBy = item.ModifiedBy;
            ds.ModifiedDate = item.ModifiedDate;
            ds.Size = item.Size;
            ds.VirtualPath = item.VirtualPath;

            ds.ConnectString = dsDef.ConnectString;

            switch (dsDef.CredentialRetrieval)
            {
                case CredentialRetrievalEnum.Integrated:
                    ds.CredentialsRetrieval = "Integrated"; break;
                case CredentialRetrievalEnum.None:
                    ds.CredentialsRetrieval = "None"; break;
                case CredentialRetrievalEnum.Prompt:
                    ds.CredentialsRetrieval = "Prompt"; break;
                case CredentialRetrievalEnum.Store:
                    ds.CredentialsRetrieval = "Store"; break;
            }

            ds.Enabled = dsDef.Enabled;
            ds.EnabledSpecified = dsDef.EnabledSpecified;
            ds.Extension = dsDef.Extension;
            ds.ImpersonateUser = dsDef.ImpersonateUser;
            ds.ImpersonateUserSpecified = dsDef.ImpersonateUserSpecified;
            ds.OriginalConnectStringExpressionBased = ds.OriginalConnectStringExpressionBased;
            ds.Password = dsDef.Password;
            ds.Prompt = dsDef.Prompt;
            ds.UseOriginalConnectString = dsDef.UseOriginalConnectString;
            ds.UserName = dsDef.UserName;
            ds.WindowsCredentials = dsDef.WindowsCredentials;

            return ds;
        }
        #endregion

        #region Item Methods
        public bool ItemExists(string itemPath, string itemType)
        {
            throw new NotImplementedException();
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
