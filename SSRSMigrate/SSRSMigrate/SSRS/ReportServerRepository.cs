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
                throw new ArgumentNullException("rootPath");

            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            this.mRootPath = rootPath;
            this.mReportingService = reportingService;
        }

        public List<FolderItem> GetFolders(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string folderPath)
        {
            throw new NotImplementedException();
        }

        public byte[] GetReportDefinition(string reportPath)
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

        public string[] WriteReport(string reportPath, ReportItem reportItem)
        {
            throw new NotImplementedException();
        }

        public DataSourceItem GetDataSource(string dataSourcePath)
        {
            if (string.IsNullOrEmpty(dataSourcePath))
                throw new ArgumentNullException("dataSourcePath");

            string dsName = dataSourcePath.Substring(dataSourcePath.LastIndexOf('/') + 1);

            CatalogItem[] items = null;

            SearchCondition nameCondition = new SearchCondition();
            nameCondition.Condition = ConditionEnum.Equals;
            nameCondition.ConditionSpecified = true;
            nameCondition.Name = "Name";
            nameCondition.Value = dsName;

            SearchCondition[] conditions = new SearchCondition[1];
            conditions[0] = nameCondition;

            items = this.mReportingService.FindItems(this.mRootPath, BooleanOperatorEnum.And, conditions);

            if (items != null)
            {
                foreach (CatalogItem item in items)
                {
                    if (item.Type == ItemTypeEnum.DataSource)
                        if (item.Path == dataSourcePath)
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
                }
            }

            return null;
        }

        public List<DataSourceItem> GetDataSources(string path)
        {
            throw new NotImplementedException();
        }

        public string[] WriteDataSource(string dataSourcePath, DataSourceItem dataSource)
        {
            throw new NotImplementedException();
        }

        public CatalogItem GetItem(string path, string itemName, string itemPath, string itemType)
        {
            throw new NotImplementedException();
        }

        public List<CatalogItem> GetItems(string path, string itemType)
        {
            throw new NotImplementedException();
        }

        public bool ItemExists(string itemPath, string itemType)
        {
            throw new NotImplementedException();
        }
    }
}
