using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.SSRS.Writer
{
    public interface IReportServerWriter
    {
        // Folders
        string WriteFolder(FolderItem folderItemn);
        string[] WriteFolders(FolderItem[] folderItems);

        // Reports
        string[] WriteReport(string reportPath, ReportItem reportItem);

        // Data Sources
        string[] WriteDataSource(string dataSourcePath, DataSourceItem dataSourceItem);

        // Items
        string[] DeleteItem(string itemPath);
    }
}
