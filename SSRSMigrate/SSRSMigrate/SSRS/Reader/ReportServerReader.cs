using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using Ninject.Extensions.Logging;

namespace SSRSMigrate.SSRS.Reader
{
    public class ReportServerReader : IReportServerReader
    {
        private readonly IReportServerRepository mReportRepository;
        private readonly ILogger mLogger = null;

        public ReportServerReader(IReportServerRepository repository, ILogger logger)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mReportRepository = repository;
            this.mLogger = logger;
        }

        #region Folder Methods
        public FolderItem GetFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolder - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            FolderItem folder = this.mReportRepository.GetFolder(path);

            return folder;
        }

        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetFolders - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            List<FolderItem> folders = this.mReportRepository.GetFolders(path);

            return folders;
        }

        public void GetFolders(string path, Action<FolderItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            this.mLogger.Debug("GetFolders2 - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            var folders = this.mReportRepository.GetFolderList(path);

            foreach (FolderItem folder in folders)
            {
                this.mLogger.Trace("GetFolders2 - Calling reporter delegate for '{0}'...", folder.Path);

                progressReporter(folder);
            }
        }
        #endregion

        #region Report Methods
        public ReportItem GetReport(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            this.mLogger.Debug("GetReport - reportPath = {0}", reportPath);

            if (!this.mReportRepository.ValidatePath(reportPath))
                throw new InvalidPathException(reportPath);

            ReportItem report = this.mReportRepository.GetReport(reportPath);

            return report;
        }

        public List<ReportItem> GetReports(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetReports - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            return this.mReportRepository.GetReports(path);
        }

        public void GetReports(string path, Action<ReportItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            this.mLogger.Debug("GetReports2 - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            var reports = this.mReportRepository.GetReportsList(path);

            foreach (ReportItem report in reports)
            {
                this.mLogger.Trace("GetReports2 - Calling reporter delegate for '{0}'...", report.Path);

                progressReporter(report);
            }
        }
        #endregion

        #region Data Source Methods
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            this.mLogger.Debug("GetDataSource - dataSourcePath = {0}", dataSourcePath);

            if (!this.mReportRepository.ValidatePath(dataSourcePath))
                throw new InvalidPathException(dataSourcePath);

            DataSourceItem dataSource = this.mReportRepository.GetDataSource(dataSourcePath);

            return dataSource;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            this.mLogger.Debug("GetDataSources - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            return this.mReportRepository.GetDataSources(path);
        }

        public void GetDataSources(string path, Action<DataSourceItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            this.mLogger.Debug("GetDataSources2 - path = {0}", path);

            if (!this.mReportRepository.ValidatePath(path))
                throw new InvalidPathException(path);

            var dataSources = this.mReportRepository.GetDataSourcesList(path);

            foreach (DataSourceItem dataSource in dataSources)
            {
                this.mLogger.Trace("GetDataSources2 - Calling reporter delegate for '{0}'...", dataSource.Path);

                progressReporter(dataSource);
            }
        }
        #endregion

        #region Misc
        public SSRSVersion GetSqlServerVersion()
        {
            return this.mReportRepository.GetSqlServerVersion();
        }
        #endregion
    }
}
