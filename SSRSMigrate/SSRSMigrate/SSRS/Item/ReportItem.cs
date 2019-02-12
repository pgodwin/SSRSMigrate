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
        }
    }
}
