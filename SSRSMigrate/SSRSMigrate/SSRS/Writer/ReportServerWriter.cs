using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Utility;

namespace SSRSMigrate.SSRS.Writer
{
    public class ReportServerWriter : IReportServerWriter
    {
        private readonly IReportServerRepository mReportRepository;

        public ReportServerWriter(IReportServerRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.mReportRepository = repository;
        }

        #region Folder Methods
        public string WriteFolder(FolderItem folderItem)
        {
            if (folderItem == null)
                throw new ArgumentNullException("folderItem");

            if (!this.mReportRepository.ValidatePath(folderItem.Path))
                throw new InvalidPathException(string.Format("Invalid path '{0}'.", folderItem.Path));

            string name = folderItem.Name;
            string parentPath = SSRSUtil.GetParentPath(folderItem);

            return this.mReportRepository.CreateFolder(name, parentPath);
        }

        public string[] WriteFolders(FolderItem[] folderItems)
        {
            if (folderItems == null)
                throw new ArgumentNullException("folderItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < folderItems.Count(); i++)
            {
                if (!this.mReportRepository.ValidatePath(folderItems[i].Path))
                    throw new InvalidPathException(string.Format("Invalid path '{0}'.", folderItems[i].Path));

                string name = folderItems[i].Name;
                string parentPath = SSRSUtil.GetParentPath(folderItems[i]);

                string warning = this.mReportRepository.CreateFolder(name, parentPath);

                if (!string.IsNullOrEmpty(warning))
                    warnings.Add(warning);
            }

            return warnings.ToArray();
        }
        #endregion

        #region Report Methods
        public string[] WriteReport(ReportItem reportItem)
        {
            if (reportItem == null)
                throw new ArgumentNullException("reportItem");

            if (!this.mReportRepository.ValidatePath(reportItem.Path))
                throw new InvalidPathException(string.Format("Invalid path '{0}'.", reportItem.Path));

            string name = reportItem.Name;
            string parentPath = SSRSUtil.GetParentPath(reportItem);

            return this.mReportRepository.WriteReport(parentPath, reportItem);
        }

        public string[] WriteReports(ReportItem[] reportItems)
        {
            if (reportItems == null)
                throw new ArgumentNullException("reportItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < reportItems.Count(); i++)
            {
                if (!this.mReportRepository.ValidatePath(reportItems[i].Path))
                    throw new InvalidPathException(string.Format("Invalid path '{0}'.", reportItems[i].Path));

                string name = reportItems[i].Name;
                string parentPath = SSRSUtil.GetParentPath(reportItems[i]);

                string[] report_warnings = this.mReportRepository.WriteReport(parentPath, reportItems[i]);

                if (report_warnings != null)
                    warnings.AddRange(report_warnings);
            }

            return warnings.ToArray();
        }

        #endregion

        #region Data Source Methods
        public string WriteDataSource(DataSourceItem dataSourceItem)
        {
            if (dataSourceItem == null)
                throw new ArgumentNullException("dataSourceItem");

            if (!this.mReportRepository.ValidatePath(dataSourceItem.Path))
                throw new InvalidPathException(string.Format("Invalid path '{0}'.", dataSourceItem.Path));

            if (this.mReportRepository.ItemExists(dataSourceItem.Path, "DataSource"))
                throw new DataSourceAlreadyExistsException(string.Format("The data source '{0}' already exists.", dataSourceItem.Path));

            string name = dataSourceItem.Name;
            string parentPath = SSRSUtil.GetParentPath(dataSourceItem);

            return this.mReportRepository.WriteDataSource(parentPath, dataSourceItem);
        }

        public string[] WriteDataSources(DataSourceItem[] dataSourceItems)
        {
            if (dataSourceItems == null)
                throw new ArgumentNullException("dataSourceItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < dataSourceItems.Count(); i++)
            {
                if (!this.mReportRepository.ValidatePath(dataSourceItems[i].Path))
                    throw new InvalidPathException(string.Format("Invalid path '{0}'.", dataSourceItems[i].Path));

                if (this.mReportRepository.ItemExists(dataSourceItems[i].Path, "DataSource"))
                    throw new DataSourceAlreadyExistsException(string.Format("The data source '{0}' already exists.", dataSourceItems[i].Path));

                string name = dataSourceItems[i].Name;
                string parentPath = SSRSUtil.GetParentPath(dataSourceItems[i]);

                string warning = this.mReportRepository.WriteDataSource(parentPath, dataSourceItems[i]);
                
                if (!string.IsNullOrEmpty(warning))
                    warnings.Add(warning);
            }

            return warnings.ToArray();
        }

        public string DeleteItem(string itemPath)
        {
            throw new NotImplementedException();
        }

        public string DeleteItem(ReportServerItem item)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
