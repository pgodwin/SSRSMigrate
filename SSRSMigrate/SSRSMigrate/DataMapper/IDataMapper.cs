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
        TSubscription>
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

        DatasetItem GetDataSet(TDataSet item);

        ItemReferenceDefinition GetItemReference(TItemReference item);

        ItemReferenceDefinition GetItemReferenceData(TItemReferenceData item);

        QueryDefinition GetQueryDefinition(TQueryDefinition item);

        PolicyDefinition GetPolicy(TPolicy item, bool inherit);

        RoleDefinition GetRole(TRole item);

        ReportServerItem GetReportServerItem(TCatalogItem item);

        ReportSubscriptionDefinition GetSubscription(TSubscription item);

        HistoryOptionsDefinition GetHistoryOptionsDefinition(TScheduleDefinitionOrReference item, bool keepSnapshots);
    }
}
