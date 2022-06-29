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
        SqlServerInfo GetSqlServerVersion();
        string[] WriteDataset(DataSetItem datasetItem);

        bool ItemExists(string itemPath, string itemType);

        void UpdateItemReferences(string itemPath, string sourceRoot, string destinationRoot);
        byte[] UpdateDefinitionReferences(byte[] itemDefinition, string destinationRoot);
    }
}
