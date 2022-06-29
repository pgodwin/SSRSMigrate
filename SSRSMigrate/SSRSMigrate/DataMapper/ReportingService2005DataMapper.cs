using System;
using System.Linq;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.DataMapper
{
    public class ReportingService2005DataMapper : IDataMapper
        <ReportServer2005.CatalogItem,
        ReportServer2005.DataSourceDefinition,
        //ReportServer2005.DataSetDefinition,
        ReportServer2005.CatalogItem,
        ItemReferenceDefinition, // ItemReference doesn't exist in 2005 - so we'll cheat and reuse the in-built one
        ItemReferenceDefinition, // ItemReferenceData doesn't exist in 20015
        ReportServer2005.QueryDefinition,
        ReportServer2005.Role,
        ReportServer2005.Policy,
        ReportServer2005.ScheduleDefinitionOrReference,
        ReportServer2005.Subscription,
        ReportServer2005.ScheduleDefinitionOrReference
        >
    {
        public ReportingService2005DataMapper()
        {
        }

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
            ds.ItemType = item.Type.ToString();

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

            ds.SourceObject = item;

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
            report.ItemType = item.Type.ToString();
            report.SourceObject = item;

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
            folder.ItemType = item.Type.ToString();
            folder.SourceObject = item;

            return folder;
        }

        public DataSetItem GetDataSet(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            //if (item.Type != ItemTypeEnum.)
            //    throw new ArgumentException("Item.Type is not a dataset");
            var datasetItem = new DataSetItem();
            datasetItem.Name = item.Name;
            datasetItem.Path = item.Path;
            datasetItem.CreatedBy = item.CreatedBy;
            datasetItem.CreationDate = item.CreationDate;
            datasetItem.Description = item.Description;
            datasetItem.ID = item.ID;
            datasetItem.ModifiedBy = item.ModifiedBy;
            datasetItem.ModifiedDate = item.ModifiedDate;
            datasetItem.Size = item.Size;
            datasetItem.VirtualPath = item.VirtualPath;
            datasetItem.ItemType = item.Type.ToString();

            // TODO - handle properties?
            datasetItem.SourceObject = item;

            return datasetItem;

        }

        /// <summary>
        /// There's no such thing as ItemReference in 2005 so just map between the two objects
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ItemReferenceDefinition GetItemReference(ItemReferenceDefinition item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var itemReference = new ItemReferenceDefinition()
            { 
                Name = item.Name,
                Reference = item.Reference,
                ReferenceType = item.ReferenceType,
                SourceObject = item.SourceObject
            };

            return itemReference;
        }

        public ItemReferenceDefinition GetItemReferenceData(ItemReferenceDefinition item)
        {
            return GetItemReference(item);
        }

        public SSRS.Item.QueryDefinition GetQueryDefinition(ReportServer2005.QueryDefinition item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var queryDefinition = new SSRS.Item.QueryDefinition()
            { 
                SourceObject = item,
                CommandText = item.CommandText,
                CommandType = item.CommandType,
            };
            if (item.TimeoutSpecified)
                queryDefinition.Timeout = item.Timeout;
            else
                queryDefinition.Timeout = null;

            return queryDefinition;
        }

        public PolicyDefinition GetPolicy(Policy item, bool inherit)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var policyDefinition = new PolicyDefinition()
            { 
                GroupUserName = item.GroupUserName,
                Roles = item.Roles.Select(GetRole).ToList(),
                SourceObject = item,
                InheritFromParent = inherit
            };

            return policyDefinition;
        }

        public RoleDefinition GetRole(Role item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var roleDefinition = new RoleDefinition()
            { 
                SourceObject = item,
                Description = item.Description,
                Name = item.Name
            };

            return roleDefinition;
        }

        public ReportServerItem GetReportServerItem(CatalogItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var serverItem = new ReportServerItem();
            serverItem.Name = item.Name;
            serverItem.Path = item.Path;
            serverItem.CreatedBy = item.CreatedBy;
            serverItem.CreationDate = item.CreationDate;
            serverItem.Description = item.Description;
            serverItem.ID = item.ID;
            serverItem.ModifiedBy = item.ModifiedBy;
            serverItem.ModifiedDate = item.ModifiedDate;
            serverItem.Size = item.Size;
            serverItem.VirtualPath = item.VirtualPath;
            serverItem.SourceObject = item;
            serverItem.ItemType = item.Type.ToString();

            return serverItem;

        }

        public SubscriptionDefinition GetSubscription(Subscription item)
        {
            throw new NotImplementedException();
        }

        public SnapshotOptionsDefinition GetHistoryOptionsDefinition(ScheduleDefinitionOrReference item, bool keepSnapshots)
        {
            throw new NotImplementedException();
        }

        public SSRSScheduleDefinition GetSchedule(ScheduleDefinitionOrReference schedule)
        {
            throw new NotImplementedException();
        }
    }
}
