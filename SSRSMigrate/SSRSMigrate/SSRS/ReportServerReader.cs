using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS
{
    public class ReportServerReader : IReportServerReader
    {
        private IReportServerRepository mReportRepository;

        public ReportServerReader(IReportServerRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.mReportRepository = repository;
        }

        #region Folder Methods
        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            List<FolderItem> folders = this.mReportRepository.GetFolders(path);

            return folders;
        }

        public void GetFolders(string path, Action<FolderItem> progressReporter)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Report Methods
        public ReportItem GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }

        public List<ReportItem> GetReports(string path)
        {
            throw new NotImplementedException();
        }

        public void GetReports(string path, Action<ReportItem> progressReporter)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Data Source Methods
        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            DataSourceItem dataSource = this.mReportRepository.GetDataSource(dataSourcePath);

            return dataSource;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            return this.mReportRepository.GetDataSources(path);
        }

        public void GetDataSources(string path, Action<DataSourceItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            List<DataSourceItem> dataSources = this.mReportRepository.GetDataSources(path);

            foreach (DataSourceItem dataSource in dataSources)
                progressReporter(dataSource);
        }
        #endregion
    }
}
