using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SSRSMigrate.SSRS.Test
{
    public interface IReportServerTester
    {
        ConnectionTestStatus ReadTest(string path);
        ConnectionTestStatus WriteTest(string path, string itemName);
    }
}
