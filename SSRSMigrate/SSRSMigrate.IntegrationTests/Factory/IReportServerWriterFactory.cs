using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Writer;

namespace SSRSMigrate.IntegrationTests.Factory
{
    public interface IReportServerWriterFactory
    {
        T GetWriter<T>(string name);
    }
}
