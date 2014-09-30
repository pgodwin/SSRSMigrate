using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Status
{
    public class MigrationStatus
    {
        public ReportServerItem Item { get; set; }
        public string FromPath { get; set; }
        public string ToPath { get; set; }
        public bool Success { get; set; }
        public Exception Error { get; set; }
        public string[] Warnings { get; set; }

        public MigrationStatus()
        {
            this.Warnings = new string[] {};
        }
    }
}
