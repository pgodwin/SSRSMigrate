using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Enum;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.SSRS.Reader
{
    public interface IReportServerReader
    {
        // Folders
        FolderItem GetFolder(string path);
        List<FolderItem> GetFolders(string path);
        void GetFolders(string path, Action<FolderItem> progressReporter);

        // Reports
        ReportItem GetReport(string reportPath);
        List<ReportItem> GetReports(string path);
        void GetReports(string path, Action<ReportItem> progressReporter);

        // Data Sources
        DataSourceItem GetDataSource(string dataSourcePath);
        List<DataSourceItem> GetDataSources(string path);
        void GetDataSources(string path, Action<DataSourceItem> progressReporter);

        // Misc
        SSRSVersion GetSqlServerVersion();

    }
}
