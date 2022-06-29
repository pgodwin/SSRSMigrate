using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public class SnapshotOptionsDefinition : BaseSSRSItem
    {
        public string ReportPath { get; set; }
        public bool KeepExecutionSnapshots { get; set; }

        public SSRSScheduleDefinition Schedule { get; set; }
       
    }

   




}
