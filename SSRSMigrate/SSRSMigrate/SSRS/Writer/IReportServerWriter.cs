using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.SSRS.Writer
{
    public interface IReportServerWriter
    {
        bool Overwrite { get; set; }

        // Folders
        string WriteFolder(FolderItem folderItemn);
        string[] WriteFolders(FolderItem[] folderItems);

        // Reports
        string[] WriteReport(ReportItem reportItem);
        string[] WriteReports(ReportItem[] reportItems);

        // Data Sources
        string WriteDataSource(DataSourceItem dataSourceItem);
        string[] WriteDataSources(DataSourceItem[] dataSourceItems);

        // Items
        string DeleteItem(string itemPath);
        string DeleteItem(ReportServerItem item);

        // Misc
        SSRSVersion GetSqlServerVersion();
    }
}
