using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2005;

namespace SSRSMigrate.SSRS
{
    public interface IReportServerRepository
    {
        // Folders
        List<CatalogItem> GetFolders(string path);
        void CreateFolder(string folderPath);

        // Reports
        byte[] GetReportDefinition(string reportPath);
        CatalogItem GetReport(string reportPath);
        List<CatalogItem> GetReports(string path);
        Warning[] WriteReport(string reportPath, ReportItem reportItem);

        // Data Source
        DataSourceDefinition GetDataSource(string dataSourcePath);
        List<CatalogItem> GetDataSources(string path);
        Warning[] WriteDataSource(string dataSourcePath, DataSourceDefinition dataSourceDefinition);

        // Items
        CatalogItem GetItem(string path, string itemName, string itemPath, ItemTypeEnum itemType);
        List<CatalogItem> GetItems(string path, string itemType);
        bool ItemExists(string itemPath, ItemTypeEnum itemType);
    }
}
