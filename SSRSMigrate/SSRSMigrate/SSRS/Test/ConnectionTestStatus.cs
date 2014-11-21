using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS.Test
{
    public class ConnectionTestStatus
    {
        public string ServerAddress { get; set; }
        public string Path { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
