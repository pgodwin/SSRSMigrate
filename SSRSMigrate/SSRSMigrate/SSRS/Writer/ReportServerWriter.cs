using System;
using System.Collections.Generic;
using System.Linq;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Utility;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Validators;

namespace SSRSMigrate.SSRS.Writer
{
    public class ReportServerWriter : IReportServerWriter
    {
        private readonly IReportServerRepository mReportRepository;
        private bool mOverwrite = false;
        private readonly ILogger mLogger = null;
        private readonly IReportServerPathValidator mPathValidator;

        public bool Overwrite
        {
            get { return this.mOverwrite; }
            set { this.mOverwrite = value; }
        }

        public ReportServerWriter(IReportServerRepository repository, ILogger logger, IReportServerPathValidator pathValidator)
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
        public string WriteFolder(FolderItem folderItem)
        {
            if (folderItem == null)
                throw new ArgumentNullException("folderItem");

            // Verify that the folder's path is valid
            if (!this.mPathValidator.Validate(folderItem.Path))
                throw new InvalidPathException(folderItem.Path);

            // Get the folder's name and path to its parent folder 
            string name = folderItem.Name;
            //string parentPath = SSRSUtil.GetParentPath(folderItem);
            string parentPath = folderItem.ParentPath;

            // Check if a folder already exists at the specified path
            if (this.mReportRepository.ItemExists(folderItem.Path, "Folder"))
                // If allow overwrite is False, throw ItemAlreadyExistsException, otherwise delete the folder
                if (!this.mOverwrite)
                    throw new ItemAlreadyExistsException(folderItem.Path);
                else
                {
                    //TODO Add tests for ReportServerWriter.WriteFolder where the folder exists and overwrite is True
                    this.mReportRepository.DeleteItem(folderItem.Path);
                }

            //return this.mReportRepository.CreateFolder(name, parentPath);
            return this.mReportRepository.CreateFolder(folderItem);
        }

        public string[] WriteFolders(FolderItem[] folderItems)
        {
            if (folderItems == null)
                throw new ArgumentNullException("folderItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < folderItems.Count(); i++)
            {
                // Verify that the folder's path is valid
                if (!this.mPathValidator.Validate(folderItems[i].Path))
                    throw new InvalidPathException(folderItems[i].Path);

                // Get the folder's name and path to its parent folder 
                string name = folderItems[i].Name;
                //string parentPath = SSRSUtil.GetParentPath(folderItems[i]);
                string parentPath = folderItems[i].ParentPath;

                // Check if a folder already exists at the specified path
                if (this.mReportRepository.ItemExists(folderItems[i].Path, "Folder"))
                    // If allow overwrite is False, throw ItemAlreadyExistsException, otherwise delete the folder
                    if (!this.mOverwrite)
                        throw new ItemAlreadyExistsException(folderItems[i].Path);
                    else
                    {
                        //TODO Add tests for ReportServerWriter.WriteFolders where the folder exists and overwrite is True
                        this.mReportRepository.DeleteItem(folderItems[i].Path);
                    }
                

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
            if (!this.mPathValidator.Validate(reportItem.ParentPath))
                throw new InvalidPathException(reportItem.Path);

            // Get the report's name and path to its parent folder
            string name = reportItem.Name;
            //string parentPath = SSRSUtil.GetParentPath(reportItem);
            string parentPath = reportItem.ParentPath;

            //TODO Validate the report's name using ReportServerItemNameValidator

            // Check if a report already exists at the specified path
            if (!this.mOverwrite)
                if (this.mReportRepository.ItemExists(reportItem.Path, "Report"))
                    throw new ItemAlreadyExistsException(reportItem.Path);

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
                if (!this.mPathValidator.Validate(reportItems[i].ParentPath))
                    throw new InvalidPathException(reportItems[i].Path);

                // Get the report's name and path to its parent folder
                string name = reportItems[i].Name;
                //string parentPath = SSRSUtil.GetParentPath(reportItems[i]);
                string parentPath = reportItems[i].ParentPath;

                //TODO Validate the report's name using ReportServerItemNameValidator

                // Check if a report already exists at the specified path
                if (!this.mOverwrite)
                    if (this.mReportRepository.ItemExists(reportItems[i].Path, "Report"))
                        throw new ItemAlreadyExistsException(reportItems[i].Path);

                string[] reportWarnings = this.mReportRepository.WriteReport(parentPath, reportItems[i], this.mOverwrite);

                if (reportWarnings != null)
                    warnings.AddRange(reportWarnings);
            }

            return warnings.ToArray();
        }

        #endregion

        #region Data Source Methods
        public string WriteDataSource(DataSourceItem dataSourceItem)
        {
            if (dataSourceItem == null)
                throw new ArgumentNullException("dataSourceItem");

            //TODO 1/11/21 jpann - Should we be getting the parent path? Been so long, I don't recall...
            if (!this.mPathValidator.Validate(dataSourceItem.Path))
                throw new InvalidPathException(dataSourceItem.Path);

            string name = dataSourceItem.Name;
            //string parentPath = SSRSUtil.GetParentPath(dataSourceItem);
            string parentPath = dataSourceItem.ParentPath;

            if (!this.mOverwrite)
                if (this.mReportRepository.ItemExists(dataSourceItem.Path, "DataSource"))
                    throw new ItemAlreadyExistsException(dataSourceItem.Path);

            return this.mReportRepository.WriteDataSource(parentPath, dataSourceItem, this.mOverwrite);
        }

        public string[] WriteDataSources(DataSourceItem[] dataSourceItems)
        {
            if (dataSourceItems == null)
                throw new ArgumentNullException("dataSourceItems");

            List<string> warnings = new List<string>();

            for (int i = 0; i < dataSourceItems.Count(); i++)
            {
                //TODO 1/11/21 jpann - Should we be getting the parent path? Been so long, I don't recall...
                if (!this.mPathValidator.Validate(dataSourceItems[i].Path))
                    throw new InvalidPathException(dataSourceItems[i].Path);

                string name = dataSourceItems[i].Name;
                //string parentPath = SSRSUtil.GetParentPath(dataSourceItems[i]);
                string parentPath = dataSourceItems[i].ParentPath;

                if (!this.mOverwrite)
                    if (this.mReportRepository.ItemExists(dataSourceItems[i].Path, "DataSource"))
                        throw new ItemAlreadyExistsException(dataSourceItems[i].Path);

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

        #region Dataset Methods
        public string[] WriteDataset(DatasetItem datasetItem)
        {
            if (datasetItem == null)
                throw new ArgumentNullException(nameof(datasetItem));
            if (!this.mPathValidator.Validate(datasetItem.Path))
                throw new InvalidPathException(datasetItem.Path);

            string name = datasetItem.Name;
            //string parentPath = SSRSUtil.GetParentPath(dataSourceItem);
            string parentPath = datasetItem.ParentPath;

            if (!this.mOverwrite)
                if (this.mReportRepository.ItemExists(datasetItem.Path, "DataSet"))
                    throw new ItemAlreadyExistsException(datasetItem.Path);

            return this.mReportRepository.WriteDataSet(parentPath, datasetItem, this.mOverwrite);

        }

        #endregion

        #region Misc
        public SqlServerInfo GetSqlServerVersion()
        {
            return this.mReportRepository.GetSqlServerVersion();
        }

        public bool ItemExists(string itemPath, string itemType)
        {
            return this.mReportRepository.ItemExists(itemPath, itemType);
        }

        public void UpdateItemReferences(string itemPath, string sourceRoot, string destinationRoot)
        {
            this.mReportRepository.UpdateItemReferences(itemPath, sourceRoot, destinationRoot);
        }
        #endregion
    }
}
