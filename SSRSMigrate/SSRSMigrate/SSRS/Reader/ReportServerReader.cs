using System;
using System.Collections.Generic;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Validators;
using System.Linq;

namespace SSRSMigrate.SSRS.Reader
{
    public class ReportServerReader : IReportServerReader
    {
        private readonly IReportServerRepository mReportRepository;
        private readonly ILogger mLogger = null;
        private readonly IReportServerPathValidator mPathValidator;

        public ReportServerReader(IReportServerRepository repository, ILogger logger, IReportServerPathValidator pathValidator)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            if (logger == null)
                throw new ArgumentNullException("logger");

            if (pathValidator == null)
                throw new ArgumentNullException("pathValidator");

            this.mReportRepository = repository;
            this.mLogger = logger;
            this.mPathValidator = pathValidator;

            this.mLogger.Debug("Repository.ServerAddress: {0}", this.mReportRepository.ServerAddress);
            this.mLogger.Debug("Repository.RootPath: {0}", this.mReportRepository.RootPath);
        }

        #region Folder Methods
        /// <inheritdoc/>
        public FolderItem GetFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolder - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            FolderItem folder = this.mReportRepository.GetFolder(path);

            return folder;
        }

        /// <inheritdoc/>
        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolders - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            List<FolderItem> folders = this.mReportRepository.GetFolders(path);

            return folders;
        }

        /// <inheritdoc/>
        public void GetFolders(string path, Action<FolderItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            this.mLogger.Debug("GetFolders2 - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            var folders = this.mReportRepository.GetFolderList(path);

            foreach (FolderItem folder in folders)
            {
                this.mLogger.Debug("GetFolders2 - Calling reporter delegate for '{0}'...", folder.Path);

                progressReporter(folder);
            }
        }
        #endregion

        #region Report Methods
        /// <inheritdoc/>
        public ReportItem GetReport(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            this.mLogger.Debug("GetReport - reportPath = {0}", reportPath);

            if (reportPath.LastIndexOf('/') < 0)
                throw new InvalidPathException(reportPath);

            string parentPath = reportPath.Substring(0, reportPath.LastIndexOf('/') + 1);

            if (!this.mPathValidator.Validate(parentPath))
                throw new InvalidPathException(reportPath);

            ReportItem report = this.mReportRepository.GetReport(reportPath);

            return report;
        }

        /// <inheritdoc/>
        public List<ReportItem> GetReports(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReports - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            return this.mReportRepository.GetReports(path);
        }

        /// <inheritdoc/>
        public void GetReports(string path, Action<ReportItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            this.mLogger.Debug("GetReports2 - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            var reports = this.mReportRepository.GetReportsLazy(path);

            foreach (ReportItem report in reports)
            {
                this.mLogger.Debug("GetReports2 - Calling reporter delegate for '{0}'...", report.Path);

                progressReporter(report);
            }
        }
        #endregion

        #region Data Source Methods
        
        /// <inheritdoc/>
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            this.mLogger.Debug("GetDataSource - dataSourcePath = {0}", dataSourcePath);

            //TODO 1/11/21 jpann - Should we be getting the parent path? Been so long, I don't recall...
            if (!this.mPathValidator.Validate(dataSourcePath))
                throw new InvalidPathException(dataSourcePath);

            DataSourceItem dataSource = this.mReportRepository.GetDataSource(dataSourcePath);

            return dataSource;
        }

        /// <inheritdoc/>
        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetDataSources - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            return this.mReportRepository.GetDataSources(path);
        }

        /// <inheritdoc/>
        public void GetDataSources(string path, Action<DataSourceItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            this.mLogger.Debug("GetDataSources2 - path = {0}", path);

            if (!this.mPathValidator.Validate(path))
                throw new InvalidPathException(path);

            var dataSources = this.mReportRepository.GetDataSourcesList(path);

            foreach (DataSourceItem dataSource in dataSources)
            {
                this.mLogger.Debug("GetDataSources2 - Calling reporter delegate for '{0}'...", dataSource.Path);

                progressReporter(dataSource);
            }
        }
        #endregion

        #region Misc

        /// <inheritdoc/>
        public SqlServerInfo GetSqlServerVersion()
        {
            return this.mReportRepository.GetSqlServerVersion();
        }

        /// <inheritdoc/>
        public string Url => this.mReportRepository.ServerAddress;

        #endregion


        #region extensions
        /// <inheritdoc/>
        public List<ItemReferenceDefinition> GetDependencies(string reportPath)
        {
            var items = this.mReportRepository.GetReportDependencies(reportPath);

            return items;

        }

        /// <inheritdoc/>
        public ReportServerItem GetItemFromReference(ItemReferenceDefinition reference)
        {
            return this.mReportRepository.GetItemFromReference(reference);
        }

        /// <inheritdoc/>
        public ReportServerItem GetItemFromPath(string path)
        {
            return this.mReportRepository.GetItem(path);
        }

        /// <inheritdoc/>
        public List<DataSetItem> GetReportDatasets(string reportPath)
        {
            return this.mReportRepository.GetReportDatasets(reportPath);
        }

        /// <inheritdoc/>
        public List<PolicyDefinition> GetItemPolicies(string itemPath)
        {
            return this.mReportRepository.GetItemPolicies(itemPath);
        }

        /// <inheritdoc/>
        public SnapshotOptionsDefinition GetReportHistoryOptions(string itemPath)
        {
            return this.mReportRepository.GetReportHistoryOptions(itemPath);
        }

        /// <inheritdoc/>
        public List<SubscriptionDefinition> GetSubscriptions(string reportPath)
        {
            return this.mReportRepository.GetSubscriptions(reportPath); 
        }

        public DataSetItem GetDataSet(string itemPath, string itemName)
        {
            return this.mReportRepository.GetDataset(itemPath, itemName);
        }


        #endregion

    }
}
