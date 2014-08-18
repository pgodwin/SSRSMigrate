using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Factory
{
    public interface IReportServerWriterFactory
    {
        T GetWriter<T>(string name);
    }
}
