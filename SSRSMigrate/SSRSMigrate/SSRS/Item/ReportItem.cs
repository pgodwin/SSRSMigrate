using System.Collections.Generic;

namespace SSRSMigrate.SSRS.Item
{
    public class ReportItem : ReportServerItem
    {
        public virtual byte[] Definition { get; set; }
        public List<ReportItem> SubReports { get; set; }

        public ReportItem()
        {
            this.SubReports = new List<ReportItem>();
            this.DependsOn = new List<ItemReferenceDefinition>();
            this.Policies = new List<PolicyDefinition>();
            this.DataSets = new List<DatasetItem>();
            this.SnapshotOptions = new HistoryOptionsDefinition();
            this.DataSources = new List<DataSourceItem>();
        }



        // Support for Dependent Items

        public List<ItemReferenceDefinition> DependsOn { get; set; }

        // Support for Datasets
        public List<DatasetItem> DataSets { get; set; }

        // Support for Subscription
        public List<ReportSubscriptionDefinition> Subscriptions { get; set; } 
        
        // Support for Snapshot Configuration
        public HistoryOptionsDefinition SnapshotOptions { get; set; }

        /// <summary>
        /// Are these shared?
        /// </summary>
        public List<DataSourceItem> DataSources { get; set; }
    }
}
