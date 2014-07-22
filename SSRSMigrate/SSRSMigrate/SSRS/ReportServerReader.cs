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

        public void GetFolders(string path, Action<FolderItem, int> progressReporter)
        {
            throw new NotImplementedException();
        }

        public ReportItem GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }

        public void GetReports(string path, Action<ReportItem, int> progressReporter)
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

        public void GetDataSources(string path, Action<DataSourceItem, int> progressReporter)
        {
            throw new NotImplementedException();
        }
    }
}
