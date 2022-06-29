using SSRSMigrate.SSRS.Item.Interfaces;
using System.Collections.Generic;

namespace SSRSMigrate.SSRS.Item
{
    public class ReportItem : 
        ReportServerItem,
        // Has dependencies
        IDependsOn, 
        // Has Properites
        IProperties, 
        // Has Data Sources
        IDataSources, 
        // Has Cache Options
        ICacheOptions, 
        // Has Subscriptions
        ISubscriptions,
        // Has Snapshots
        ISnapshotOptions,
        // Has Policies
        IPolicies,

        IDefinition
    {

        public ReportItem() : base()
        {
            this.SubReports = new List<ReportItem>();
            this.DependsOn = new List<ItemReferenceDefinition>();
            this.Policies = new List<PolicyDefinition>();
            this.DataSets = new List<DataSetItem>();
            this.SnapshotOptions = new SnapshotOptionsDefinition();
            this.DataSources = new List<DataSourceItem>();
            this.Subscriptions = new List<SubscriptionDefinition>();
            this.CacheOptions = new CacheDefinition();
        }

        /// <inheritdoc>
        public List<ItemReferenceDefinition> DependsOn { get; set; }

        /// <inheritdoc>
        public List<DataSetItem> DataSets { get; set; }


        /// <inheritdoc>
        public virtual byte[] Definition { get; set; }

        /// <summary>
        /// Sub-reports related to this report
        /// </summary>
        public List<ReportItem> SubReports { get; set; }


        /// <inheritdoc>
        public SnapshotOptionsDefinition SnapshotOptions { get; set; }

        /// <inheritdoc>
        public List<DataSourceItem> DataSources { get; set; }

        /// <inheritdoc>
        public CacheDefinition CacheOptions { get; set; }

        /// <inheritdoc>
        public List<SubscriptionDefinition> Subscriptions { get; set; }
    }
}
