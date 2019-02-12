using System;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.DataMapper
{
    public class ReportingService2010DataMapper : IDataMapper<CatalogItem, DataSourceDefinition>
    {
        public DataSourceItem GetDataSource(CatalogItem item, DataSourceDefinition definition)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (definition == null)
                throw new ArgumentNullException("definition");

            DataSourceItem ds = new DataSourceItem();

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

            ds.ConnectString = definition.ConnectString;

            switch (definition.CredentialRetrieval)
            {
                case CredentialRetrievalEnum.Integrated:
                    ds.CredentialsRetrieval = "Integrated"; break;
                case CredentialRetrievalEnum.None:
                    ds.CredentialsRetrieval = "None"; break;
                case CredentialRetrievalEnum.Prompt:
                    ds.CredentialsRetrieval = "Prompt"; break;
                case CredentialRetrievalEnum.Store:
                    ds.CredentialsRetrieval = "Store"; break;
            }

            ds.Enabled = definition.Enabled;
            ds.EnabledSpecified = definition.EnabledSpecified;
            ds.Extension = definition.Extension;
            ds.ImpersonateUser = definition.ImpersonateUser;
            ds.ImpersonateUserSpecified = definition.ImpersonateUserSpecified;
            ds.OriginalConnectStringExpressionBased = ds.OriginalConnectStringExpressionBased;
            ds.Password = definition.Password;
            ds.Prompt = definition.Prompt;
            ds.UseOriginalConnectString = definition.UseOriginalConnectString;
            ds.UserName = definition.UserName;
            ds.WindowsCredentials = definition.WindowsCredentials;

            return ds;
        }

        public ReportItem GetReport(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            ReportItem report = new ReportItem();

            report.Name = item.Name;
            report.Path = item.Path;
            report.CreatedBy = item.CreatedBy;
            report.CreationDate = item.CreationDate;
            report.Description = item.Description;
            report.ID = item.ID;
            report.ModifiedBy = item.ModifiedBy;
            report.ModifiedDate = item.ModifiedDate;
            report.Size = item.Size;
            report.VirtualPath = item.VirtualPath;

            return report;
        }

        public FolderItem GetFolder(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

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

            return folder;
        }
    }
}
