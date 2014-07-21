using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2005;

namespace SSRSMigrate.SSRS
{
    public class ReportServerRepository : IReportServerRepository
    {
        private ReportingService2005 mReportingService;

        public ReportServerRepository(ReportingService2005 reportingService)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            this.mReportingService = reportingService;
        }

        public List<CatalogItem> GetFolders(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string folderPath)
        {
            throw new NotImplementedException();
        }

        public byte[] GetReportDefinition(string reportPath)
        {
            throw new NotImplementedException();
        }

        public CatalogItem GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }

        public List<CatalogItem> GetReports(string path)
        {
            throw new NotImplementedException();
        }

        public Warning[] WriteReport(string reportPath, ReportItem reportItem)
        {
            throw new NotImplementedException();
        }

        public DataSourceDefinition GetDataSource(string dataSourcePath)
        {
            throw new NotImplementedException();
        }

        public List<CatalogItem> GetDataSources(string path)
        {
            throw new NotImplementedException();
        }

        public Warning[] WriteDataSource(string dataSourcePath, DataSourceDefinition dataSourceDefinition)
        {
            throw new NotImplementedException();
        }

        public CatalogItem GetItem(string path, string itemName, string itemPath, ItemTypeEnum itemType)
        {
            throw new NotImplementedException();
        }

        public List<CatalogItem> GetItems(string path, string itemType)
        {
            throw new NotImplementedException();
        }

        public bool ItemExists(string itemPath, ItemTypeEnum itemType)
        {
            throw new NotImplementedException();
        }
    }
}
