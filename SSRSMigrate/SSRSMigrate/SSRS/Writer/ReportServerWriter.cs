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
        private IReportServerRepository mReportRepository;

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
        public string WriteReport(ReportItem reportItem)
        {
            throw new NotImplementedException();
        }

        public string[] WriteReports(ReportItem[] reportItems)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Data Source Methods
        public string WriteDataSource(DataSourceItem dataSourceItem)
        {
            if (dataSourceItem == null)
                throw new ArgumentNullException("dataSourceItem");

            if (!this.mReportRepository.ValidatePath(dataSourceItem.Path))
                throw new InvalidPathException(string.Format("Invalid path '{0}'.", dataSourceItem.Path));

            string name = dataSourceItem.Name;
            string parentPath = SSRSUtil.GetParentPath(dataSourceItem);

            return this.mReportRepository.WriteDataSource(dataSourceItem.Path, dataSourceItem);
        }

        public string[] WriteDataSources(DataSourceItem[] dataSourceItems)
        {
            throw new NotImplementedException();
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
