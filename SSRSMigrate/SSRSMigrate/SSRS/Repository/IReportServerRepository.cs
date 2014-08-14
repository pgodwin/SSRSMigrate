using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.SSRS.Repository
{
    public interface IReportServerRepository : IDisposable
    {
        // Properties
        string InvalidPathChars { get; }

        // Folders
        List<FolderItem> GetFolders(string path);
        IEnumerable<FolderItem> GetFolderList(string path);
        string CreateFolder(string name, string parentPath);

        // Reports
        byte[] GetReportDefinition(string reportPath);
        ReportItem GetReport(string reportPath);
        List<ReportItem> GetReports(string path);
        IEnumerable<ReportItem> GetReportsList(string path);
        List<ReportItem> GetSubReports(string reportDefinition);
        string[] WriteReport(string reportPath, ReportItem reportItem);

        // Data Source
        DataSourceItem GetDataSource(string dataSourcePath);
        List<DataSourceItem> GetDataSources(string path);
        IEnumerable<DataSourceItem> GetDataSourcesList(string path);
        string WriteDataSource(string dataSourcePath, DataSourceItem dataSource);

        // Misc.
        bool ValidatePath(string path);

        // Items
        bool ItemExists(string itemPath, string itemType);
    }
}
