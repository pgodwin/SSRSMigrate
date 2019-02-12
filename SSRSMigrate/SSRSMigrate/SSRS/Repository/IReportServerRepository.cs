using System;
using System.Collections.Generic;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Item;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item.Proxy;

namespace SSRSMigrate.SSRS.Repository
{
    public interface IReportServerRepository : IDisposable
    {
        // Properties
        string InvalidPathChars { get; }
        ILogger Logger { get; set; }
        string ServerAddress { get;  }
        string RootPath { get; }

        // Folders
        FolderItem GetFolder(string path);
        List<FolderItem> GetFolders(string path);
        IEnumerable<FolderItem> GetFolderList(string path);
        string CreateFolder(string name, string parentPath);

        // Reports
        byte[] GetReportDefinition(string reportPath);
        ReportItem GetReport(string reportPath);
        List<ReportItem> GetReports(string path);
        IEnumerable<ReportItem> GetReportsLazy(string path);
        List<ReportItem> GetSubReports(string reportDefinition);
        string[] WriteReport(string reportPath, ReportItem reportItem, bool overwrite);

        // Data Source
        DataSourceItem GetDataSource(string dataSourcePath);
        List<DataSourceItem> GetDataSources(string path);
        IEnumerable<DataSourceItem> GetDataSourcesList(string path);
        string WriteDataSource(string dataSourcePath, DataSourceItem dataSource, bool overwrite);

        // Misc.
        bool ValidatePath(string path);
        bool ValidateItemPath(string itemPath);
        SSRSVersion GetSqlServerVersion();

        // Items
        bool ItemExists(string itemPath, string itemType);
        void DeleteItem(string itemPath);
    }
}
