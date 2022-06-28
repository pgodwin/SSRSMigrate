using System;
using System.Collections.Generic;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Item;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item.Proxy;
using System.Web.Services.Protocols;

namespace SSRSMigrate.SSRS.Repository
{


    public interface IReportServerRepository : IDisposable
    {
        // Properties
        ILogger Logger { get; set; }
        string ServerAddress { get;  }
        string RootPath { get; set; }

        SoapHttpClientProtocol SoapClient { get; }

        //TMapper DataMapper { get; set; }


        // Folders
        FolderItem GetFolder(string path);
        List<FolderItem> GetFolders(string path);
        IEnumerable<FolderItem> GetFolderList(string path);
        
        /// <summary>
        /// Creates the specified folder, and any other folders within the path
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentPath"></param>
        /// <returns>Any errors</returns>
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
        SqlServerInfo GetSqlServerVersion();

        // Items
        bool ItemExists(string itemPath, string itemType);
        void DeleteItem(string itemPath);

        /// <summary>
        /// Grabs the items related to a report
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        List<ItemReferenceDefinition> GetReportDependencies(string reportPath);

        /// <summary>
        /// Returns the base ReportServerItem for a given parth
        /// </summary>
        /// <param name="itemPath"></param>
        /// <returns></returns>
        ReportServerItem GetItem(string itemPath);

        /// <summary>
        /// Returns the base ReportServerItem for a given item reference
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        ReportServerItem GetItemFromReference(ItemReferenceDefinition reference);

        /// <summary>
        /// Returns a dataset for the given itemPath
        /// </summary>
        /// <param name="itemPath">Path to the item.</param>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        DatasetItem GetDataset(string itemPath, string itemName);

        /// <summary>
        /// Returns the datasets used by a report
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        List<DatasetItem> GetReportDatasets(string reportPath);

        /// <summary>
        /// Returns the security policies for an item.
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        List<PolicyDefinition> GetItemPolicies(string reportPath);

        /// <summary>
        /// Returns the History.
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        HistoryOptionsDefinition GetReportHistoryOptions(string reportPath);

        /// <summary>
        /// Returns the subscriptions used by a report.
        /// </summary>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        List<ReportSubscriptionDefinition> GetSubscriptions(string reportPath);

        /// <summary>
        /// Creates/Writes a datasetItem into the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="datasetItem"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        string[] WriteDataSet(string path, DatasetItem datasetItem, bool overwrite);
        
        /// <summary>
        /// Returns the properties for the specified item path
        /// </summary>
        /// <param name="itemPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        List<ItemProperty> GetItemProperties(string itemPath);
        List<DataSourceItem> GetReportDataSources(string reportPath);
        
        /// <summary>
        /// Creates a folder including properties.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        string CreateFolder(FolderItem folder);
        void UpdateItemReferences(string itemPath, string sourceRoot, string destinationRoot);
    }
}
