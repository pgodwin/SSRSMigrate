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

        public List<FolderItem> GetFolders(string path)
        {
            throw new NotImplementedException();
        }

        public void GetFolders(string path, Action<FolderItem> progressReporter)
        {
            throw new NotImplementedException();
        }

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

        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentNullException("dataSourcePath");

            DataSourceItem dataSource = this.mReportRepository.GetDataSource(dataSourcePath);

            return dataSource;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return this.mReportRepository.GetDataSources(path);
        }

        public void GetDataSources(string path, Action<DataSourceItem> progressReporter)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (progressReporter == null)
                throw new ArgumentNullException("progressReporter");

            List<DataSourceItem> dataSources = this.mReportRepository.GetDataSources(path);

            foreach (DataSourceItem dataSource in dataSources)
                progressReporter(dataSource);
        }
    }
}
