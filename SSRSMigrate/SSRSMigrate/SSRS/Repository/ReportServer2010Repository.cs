using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2010;

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2010Repository : IReportServerRepository
    {
        private ReportingService2010 mReportingService;

        private string mRootPath = null;
        private string mInvalidChars = "";

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
            get { return this.mInvalidChars; }
        }
        #endregion

        #region Folder Methods
        public List<Item.FolderItem> GetFolders(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item.FolderItem> GetFolderList(string path)
        {
            throw new NotImplementedException();
        }

        public string CreateFolder(string folderPath)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Report Methods
        public byte[] GetReportDefinition(string reportPath)
        {
            throw new NotImplementedException();
        }

        public Item.ReportItem GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }

        public List<Item.ReportItem> GetReports(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item.ReportItem> GetReportsList(string path)
        {
            throw new NotImplementedException();
        }

        public List<Item.ReportItem> GetSubReports(string reportDefinition)
        {
            throw new NotImplementedException();
        }

        public string[] WriteReport(string reportPath, Item.ReportItem reportItem)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DataSource Methods
        public Item.DataSourceItem GetDataSource(string dataSourcePath)
        {
            throw new NotImplementedException();
        }

        public List<Item.DataSourceItem> GetDataSources(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item.DataSourceItem> GetDataSourcesList(string path)
        {
            throw new NotImplementedException();
        }

        public string[] WriteDataSource(string dataSourcePath, Item.DataSourceItem dataSource)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Misc.
        public bool ValidatePath(string path)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Item Methods
        public bool ItemExists(string itemPath, string itemType)
        {
            throw new NotImplementedException();
        }

        public CatalogItem GetItem(string itemName, string itemPath, string itemType)
        {
            throw new NotImplementedException();
        }

        public List<CatalogItem> GetItems(string path, string itemType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CatalogItem> GetItemsList(string path, string itemType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetItemsList<T>(string path, string itemType, Func<CatalogItem, T> itemConverter)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
