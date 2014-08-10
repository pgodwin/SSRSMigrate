using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2010Repository : IReportServerRepository
    {
        private ReportingService2010 mReportingService;

        private string mRootPath = null;
        private string mInvalidPathChars = ":?;@&=+$,\\*><|.\"";

        public ReportServer2010Repository(string rootPath, ReportingService2010 reportingService)
        {
            if (string.IsNullOrEmpty(rootPath))
                throw new ArgumentException("rootPath");

            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            this.mRootPath = rootPath;
            this.mReportingService = reportingService;
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
                    this.mReportingService = null;
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
            List<CatalogItem> items = this.GetItems(path, "Folder");

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

            var items = this.GetItemsList<FolderItem>(path, "Folder", folder => CatalogItemToFolderItem(folder));
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
            throw new NotImplementedException();
        }

        public ReportItem GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }

        public List<ReportItem> GetReports(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReportItem> GetReportsList(string path)
        {
            throw new NotImplementedException();
        }

        public List<ReportItem> GetSubReports(string reportDefinition)
        {
            throw new NotImplementedException();
        }

        public string[] WriteReport(string reportPath, ReportItem reportItem)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DataSource Methods
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            throw new NotImplementedException();
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataSourceItem> GetDataSourcesList(string path)
        {
            throw new NotImplementedException();
        }

        public string[] WriteDataSource(string dataSourcePath, DataSourceItem dataSource)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Misc.
        public bool ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            return path.IndexOfAny(this.mInvalidPathChars.ToCharArray()) < 0;
        }
        #endregion

        #region Item Methods
        public bool ItemExists(string itemPath, string itemType)
        {
            throw new NotImplementedException();
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

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = nameCondition;

            CatalogItem[] items = this.mReportingService.FindItems(this.mRootPath, BooleanOperatorEnum.And, null, conditions);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                    if (item.TypeName == itemType)
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
            typeCondition.Values = new string[] { "Folder" };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
                return items.Where(item => item.TypeName == itemType).Select(item => item).ToList<CatalogItem>();
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
            typeCondition.Values = new string[] { "Folder" };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
                return items.Where(item => item.TypeName == itemType).Select(item => item);
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
            typeCondition.Values = new string[] { "Folder" };

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = typeCondition;

            CatalogItem[] items = this.mReportingService.FindItems(path, BooleanOperatorEnum.Or, null, conditions);

            if (items.Any())
                return items.Where(item => item.TypeName == itemType).Select(item => itemConverter(item));
            else
                return null;
        }
        #endregion
    }
}
