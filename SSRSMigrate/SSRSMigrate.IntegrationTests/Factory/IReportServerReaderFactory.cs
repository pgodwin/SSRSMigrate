using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Reader;

namespace SSRSMigrate.IntegrationTests.Factory
{
    public interface IReportServerReaderFactory
    {
        //TODO This is probably dumb =/
        T GetReader<T>(string name);
    }
}
