using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Bundler.Events
{
    public class ItemReadEvent : EventArgs
    {
        public ReportServerItem Item { get; private set; }
        public bool Success { get; private set; }
        public string[] Errors { get; private set; }

        public ItemReadEvent(ReportServerItem item, bool success, string[] errors = null)
        {
            this.Item = item;
            this.Success = success;
            this.Errors = errors;
        }
    }
}
