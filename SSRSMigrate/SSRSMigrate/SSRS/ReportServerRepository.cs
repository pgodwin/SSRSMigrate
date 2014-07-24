using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.ReportServer2005;

namespace SSRSMigrate.SSRS
{
    public class ReportServerRepository : IReportServerRepository
    {
        private ReportingService2005 mReportingService;
        private string mRootPath = null;

        public ReportServerRepository(string rootPath, ReportingService2005 reportingService)
        {
            if (string.IsNullOrEmpty(rootPath))
                throw new ArgumentException("rootPath");

            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            this.mRootPath = rootPath;
            this.mReportingService = reportingService;
        }

        public List<FolderItem> GetFolders(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            List<FolderItem> folderItems = new List<FolderItem>();
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.Folder);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    FolderItem folder = new FolderItem();

                    folder.CreatedBy = item.CreatedBy;
                    folder.CreationDate = item.CreationDate;
                    folder.Description = item.Description;
                    folder.ID = item.ID;
                    folder.ModifiedBy = item.ModifiedBy;
                    folder.ModifiedDate = item.ModifiedDate;
                    folder.Name = item.Name;
                    folder.Path = item.Path;
                    folder.Size = item.Size;
                    folder.VirtualPath = item.VirtualPath;

                    folderItems.Add(folder);
                }

                return folderItems;
            }

            return null;
        }

        public IEnumerable<FolderItem> GetFolderList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            var items = this.GetItemsList(path, ItemTypeEnum.Folder);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    FolderItem folder = new FolderItem();

                    folder.CreatedBy = item.CreatedBy;
                    folder.CreationDate = item.CreationDate;
                    folder.Description = item.Description;
                    folder.ID = item.ID;
                    folder.ModifiedBy = item.ModifiedBy;
                    folder.ModifiedDate = item.ModifiedDate;
                    folder.Name = item.Name;
                    folder.Path = item.Path;
                    folder.Size = item.Size;
                    folder.VirtualPath = item.VirtualPath;

                    yield return folder;
                }
            }
        }

        public void CreateFolder(string folderPath)
        {

            throw new NotImplementedException();
        }

        public byte[] GetReportDefinition(string reportPath)
        {
            if (string.IsNullOrEmpty(reportPath))
                throw new ArgumentException("reportPath");

            byte[] def = this.mReportingService.GetReportDefinition(reportPath);

            return def;
        }

        public ReportItem GetReport(string reportPath)
        {
            throw new NotImplementedException();
        }

        public List<ReportItem> GetReports(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReportItem> GetReportsList(string path)
        {
            throw new NotImplementedException();
        }

        public string[] WriteReport(string reportPath, ReportItem reportItem)
        {
            throw new NotImplementedException();
        }

        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentException("dataSourcePath");

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            CatalogItem item = this.GetItem(dsName, dataSourcePath, ItemTypeEnum.DataSource);

            if (item != null)
            {
                DataSourceItem ds = new DataSourceItem();
                DataSourceDefinition dsDef = this.mReportingService.GetDataSourceContents(item.Path);

                ds.Name = item.Name;
                ds.Path = item.Path;
                ds.CreatedBy = item.CreatedBy;
                ds.CreationDate = item.CreationDate;
                ds.Description = item.Description;
                ds.ID = item.ID;
                ds.ModifiedBy = item.ModifiedBy;
                ds.ModifiedDate = item.ModifiedDate;
                ds.Size = item.Size;
                ds.VirtualPath = item.VirtualPath;

                            
                ds.ConnectString = dsDef.ConnectString;
                ds.CredentialsRetrieval = dsDef.CredentialRetrieval;
                ds.Enabled = dsDef.Enabled;
                ds.EnabledSpecified = dsDef.EnabledSpecified;
                ds.Extension = dsDef.Extension;
                ds.ImpersonateUser = dsDef.ImpersonateUser;
                ds.ImpersonateUserSpecified = dsDef.ImpersonateUserSpecified;
                ds.OriginalConnectStringExpressionBased = ds.OriginalConnectStringExpressionBased;
                ds.Password = dsDef.Password;
                ds.Prompt = dsDef.Prompt;
                ds.UseOriginalConnectString = dsDef.UseOriginalConnectString;
                ds.UserName = dsDef.UserName;
                ds.WindowsCredentials = dsDef.WindowsCredentials;

                return ds;
            }

            return null;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            List<DataSourceItem> dataSourceItems = new List<DataSourceItem>();
            List<CatalogItem> items = this.GetItems(path, ItemTypeEnum.DataSource);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                {
                    DataSourceItem ds = new DataSourceItem();
                    DataSourceDefinition dsDef = this.mReportingService.GetDataSourceContents(item.Path);

                    ds.Name = item.Name;
                    ds.Path = item.Path;
                    ds.CreatedBy = item.CreatedBy;
                    ds.CreationDate = item.CreationDate;
                    ds.Description = item.Description;
                    ds.ID = item.ID;
                    ds.ModifiedBy = item.ModifiedBy;
                    ds.ModifiedDate = item.ModifiedDate;
                    ds.Size = item.Size;
                    ds.VirtualPath = item.VirtualPath;


                    ds.ConnectString = dsDef.ConnectString;
                    ds.CredentialsRetrieval = dsDef.CredentialRetrieval;
                    ds.Enabled = dsDef.Enabled;
                    ds.EnabledSpecified = dsDef.EnabledSpecified;
                    ds.Extension = dsDef.Extension;
                    ds.ImpersonateUser = dsDef.ImpersonateUser;
                    ds.ImpersonateUserSpecified = dsDef.ImpersonateUserSpecified;
                    ds.OriginalConnectStringExpressionBased = ds.OriginalConnectStringExpressionBased;
                    ds.Password = dsDef.Password;
                    ds.Prompt = dsDef.Prompt;
                    ds.UseOriginalConnectString = dsDef.UseOriginalConnectString;
                    ds.UserName = dsDef.UserName;
                    ds.WindowsCredentials = dsDef.WindowsCredentials;

                    dataSourceItems.Add(ds);
                }

                return dataSourceItems;
            }

            return null;
        }

        public IEnumerable<DataSourceItem> GetDataSourcesList(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            var items = this.GetItemsList(path, ItemTypeEnum.DataSource);

            if (items != null)
            {
                foreach (CatalogItem item in items)
                {
                    DataSourceItem ds = new DataSourceItem();
                    DataSourceDefinition dsDef = this.mReportingService.GetDataSourceContents(item.Path);

                    ds.Name = item.Name;
                    ds.Path = item.Path;
                    ds.CreatedBy = item.CreatedBy;
                    ds.CreationDate = item.CreationDate;
                    ds.Description = item.Description;
                    ds.ID = item.ID;
                    ds.ModifiedBy = item.ModifiedBy;
                    ds.ModifiedDate = item.ModifiedDate;
                    ds.Size = item.Size;
                    ds.VirtualPath = item.VirtualPath;

                    ds.ConnectString = dsDef.ConnectString;
                    ds.CredentialsRetrieval = dsDef.CredentialRetrieval;
                    ds.Enabled = dsDef.Enabled;
                    ds.EnabledSpecified = dsDef.EnabledSpecified;
                    ds.Extension = dsDef.Extension;
                    ds.ImpersonateUser = dsDef.ImpersonateUser;
                    ds.ImpersonateUserSpecified = dsDef.ImpersonateUserSpecified;
                    ds.OriginalConnectStringExpressionBased = ds.OriginalConnectStringExpressionBased;
                    ds.Password = dsDef.Password;
                    ds.Prompt = dsDef.Prompt;
                    ds.UseOriginalConnectString = dsDef.UseOriginalConnectString;
                    ds.UserName = dsDef.UserName;
                    ds.WindowsCredentials = dsDef.WindowsCredentials;

                    yield return ds;
                }
            }
        }

        public string[] WriteDataSource(string dataSourcePath, DataSourceItem dataSource)
        {
            throw new NotImplementedException();
        }

        public bool ItemExists(string itemPath, string itemType)
        {
            throw new NotImplementedException();
        }

        public CatalogItem GetItem(string itemName, string itemPath, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(itemName))
                throw new ArgumentNullException("itemName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentNullException("itemPath");

            SearchCondition nameCondition = new SearchCondition();
            nameCondition.Condition = ConditionEnum.Equals;
            nameCondition.ConditionSpecified = true;
            nameCondition.Name = "Name";
            nameCondition.Value = itemName;

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = nameCondition;

            CatalogItem[] items = this.mReportingService.FindItems(this.mRootPath, BooleanOperatorEnum.And, conditions);

            if (items.Any())
            {
                foreach (CatalogItem item in items)
                    if (item.Type == itemType)
                        if (item.Path == itemPath)
                            return item;
            }

            return null;
        }

        public List<CatalogItem> GetItems(string path, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
                return items.Where(item => item.Type == itemType).Select(item => item).ToList<CatalogItem>();
            else
                return null;
        }

        public IEnumerable<CatalogItem> GetItemsList(string path, ItemTypeEnum itemType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            CatalogItem[] items = this.mReportingService.ListChildren(path, true);

            if (items.Any())
                return items.Where(item => item.Type == itemType).Select(item => item);
            else
                return null;
        }
    }
}
