using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Factory
{
    public interface IReportServerReaderFactory
    {
        T GetReader<T>(string name);
    }
}
