using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS.Repository
{
    public class ReportServer2010Repository : IReportServerRepository
    {
        private string mRootPath = null;
        private string mInvalidChars = "";

        public ReportServer2010Repository(string roothPath)
        {

        }

        #region Properties
        public string InvalidPathChars
        {
            get { return this.mInvalidChars; }
        }
        #endregion

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

        public bool ValidatePath(string path)
        {
            throw new NotImplementedException();
        }

        public bool ItemExists(string itemPath, string itemType)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
