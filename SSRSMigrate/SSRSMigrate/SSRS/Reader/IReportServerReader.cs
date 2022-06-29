using System;
using System.Collections.Generic;
using SSRSMigrate.Enum;
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
        SqlServerInfo GetSqlServerVersion();

        List<ItemReferenceDefinition> GetDependencies(string itemPath);

        ReportServerItem GetItemFromReference(ItemReferenceDefinition reference);

        ReportServerItem GetItemFromPath(string path);

        List<DataSetItem> GetReportDatasets(string reportPath);

        List<PolicyDefinition> GetItemPolicies(string itemPath);

        SnapshotOptionsDefinition GetReportHistoryOptions(string itemPath);
        List<SubscriptionDefinition> GetSubscriptions(string reportPath);

        DataSetItem GetDataSet(string itemPath, string itemName);

        string Url { get; }
    }
}
