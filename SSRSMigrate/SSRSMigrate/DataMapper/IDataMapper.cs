using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.DataMapper
{
    public interface IDataMapper
        <TCatalogItem, 
        TDataSource, 
        TDataSet,
        TItemReference, 
        TItemReferenceData,
        TQueryDefinition, 
        TRole, 
        TPolicy,
        TScheduleDefinitionOrReference,
        TSubscription,
        TSchedule>
        // TODO
        //Schedules?
    {
        /// <summary>
        /// Maps an SSRS Data source to an internal DataSourceItem
        /// </summary>
        /// <param name="item"></param>
        /// <param name="definition"></param>
        /// <returns></returns>
        DataSourceItem GetDataSource(TCatalogItem item, TDataSource definition);
        ReportItem GetReport(TCatalogItem item);
        FolderItem GetFolder(TCatalogItem item);

        DataSetItem GetDataSet(TDataSet item);

        ItemReferenceDefinition GetItemReference(TItemReference item);

        ItemReferenceDefinition GetItemReferenceData(TItemReferenceData item);

        QueryDefinition GetQueryDefinition(TQueryDefinition item);

        PolicyDefinition GetPolicy(TPolicy item, bool inherit);

        RoleDefinition GetRole(TRole item);

        ReportServerItem GetReportServerItem(TCatalogItem item);

        SubscriptionDefinition GetSubscription(TSubscription item);

        SnapshotOptionsDefinition GetHistoryOptionsDefinition(TScheduleDefinitionOrReference item, bool keepSnapshots);

        SSRSScheduleDefinition GetSchedule(TSchedule schedule);
    }
}
