using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2005;

namespace SSRSMigrate.SSRS
{
    public interface IReportServerReader
    {
        // Folders
        void GetFolders(string path, Action<FolderItem, int> progressReporter);

        // Reports
        ReportItem GetReport(string reportPath);
        void GetReports(string path, Action<ReportItem, int> progressReporter);

        // Data Sources
        DataSourceItem GetDataSource(string dataSourcePath);
        void GetDataSources(string path, Action<DataSourceItem, int> progressReporter);

    }
}
