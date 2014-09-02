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
        private bool mOverwrite = false;

        public bool Overwrite
        {
            get { return this.mOverwrite; }
            set { this.mOverwrite = value; }
        }

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

            // Verify that the folder's path is valid
            if (!this.mReportRepository.ValidatePath(folderItem.Path))
                throw new InvalidPathException(string.Format("Invalid path '{0}'.", folderItem.Path));

            //TODO Check folderItem.HasValidProperties and throw exception
            //if (!folderItem.HasValidProperties)
            //    throw new InvalidItemException(string.Format("The item with ID '{0}' has a null or empty name or path value.", folderItem.ID));

            // Get the folder's name and path to its parent folder 
            string name = folderItem.Name;
            string parentPath = SSRSUtil.GetParentPath(folderItem);

            // Check if a folder already exists at the specified path
            if (this.mReportRepository.ItemExists(folderItem.Path, "Folder"))
                throw new ItemAlreadyExistsException(string.Format("The folder '{0}' already exists.", folderItem.Path));

            return this.mReportRepository.CreateFolder(name, parentPath);
        }

        public string[] WriteFolders(FolderItem[] folderItems)
        {
            if (folderItems == null)
                throw new ArgumentNullException("folderItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < folderItems.Count(); i++)
            {
                // Verify that the folder's path is valid
                if (!this.mReportRepository.ValidatePath(folderItems[i].Path))
                    throw new InvalidPathException(string.Format("Invalid path '{0}'.", folderItems[i].Path));

                // Get the folder's name and path to its parent folder 
                string name = folderItems[i].Name;
                string parentPath = SSRSUtil.GetParentPath(folderItems[i]);

                // Check if a folder already exists at the specified path
                if (!this.mOverwrite)
                    if (this.mReportRepository.ItemExists(folderItems[i].Path, "Folder"))
                    throw new ItemAlreadyExistsException(string.Format("The folder '{0}' already exists.", folderItems[i].Path));

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

            // Verify that the report's path is valid
            if (!this.mReportRepository.ValidatePath(reportItem.Path))
                throw new InvalidPathException(string.Format("Invalid path '{0}'.", reportItem.Path));

            // Get the report's name and path to its parent folder
            string name = reportItem.Name;
            string parentPath = SSRSUtil.GetParentPath(reportItem);

            // Check if a report already exists at the specified path
            if (!this.mOverwrite)
                if (this.mReportRepository.ItemExists(reportItem.Path, "Report"))
                    throw new ItemAlreadyExistsException(string.Format("The report '{0}' already exists.", reportItem.Path));

            // Create the report at parentPath and return any warnings
            return this.mReportRepository.WriteReport(parentPath, reportItem, this.mOverwrite);
        }

        public string[] WriteReports(ReportItem[] reportItems)
        {
            if (reportItems == null)
                throw new ArgumentNullException("reportItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < reportItems.Count(); i++)
            {
                // Verify that the report's path is valid
                if (!this.mReportRepository.ValidatePath(reportItems[i].Path))
                    throw new InvalidPathException(string.Format("Invalid path '{0}'.", reportItems[i].Path));

                // Get the report's name and path to its parent folder
                string name = reportItems[i].Name;
                string parentPath = SSRSUtil.GetParentPath(reportItems[i]);

                // Check if a report already exists at the specified path
                if (!this.mOverwrite)
                    if (this.mReportRepository.ItemExists(reportItems[i].Path, "Report"))
                    throw new ItemAlreadyExistsException(string.Format("The report '{0}' already exists.", reportItems[i].Path));

                string[] report_warnings = this.mReportRepository.WriteReport(parentPath, reportItems[i], this.mOverwrite);

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

            string name = dataSourceItem.Name;
            string parentPath = SSRSUtil.GetParentPath(dataSourceItem);

            if (!this.mOverwrite)
                if (this.mReportRepository.ItemExists(dataSourceItem.Path, "DataSource"))
                throw new ItemAlreadyExistsException(string.Format("The data source '{0}' already exists.", dataSourceItem.Path));

            return this.mReportRepository.WriteDataSource(parentPath, dataSourceItem, this.mOverwrite);
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

                string name = dataSourceItems[i].Name;
                string parentPath = SSRSUtil.GetParentPath(dataSourceItems[i]);

                if (!this.mOverwrite)
                    if (this.mReportRepository.ItemExists(dataSourceItems[i].Path, "DataSource"))
                    throw new ItemAlreadyExistsException(string.Format("The data source '{0}' already exists.", dataSourceItems[i].Path));

                string warning = this.mReportRepository.WriteDataSource(parentPath, dataSourceItems[i], this.mOverwrite);
                
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
