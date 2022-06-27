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
        }



        // Support for Dependent Items

        public List<ItemReferenceDefinition> DependsOn { get; set; }

        // Support for Datasets
        
        // Support for Subscription
        
        // Support for schedules

        // Support for policies
        public List<PolicyDefinition> Policies { get; set; }
    }
}
