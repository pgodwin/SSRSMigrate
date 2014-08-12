using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Repository;

namespace SSRSMigrate.Factory
{
    public interface IReportServerRepositoryFactory
    {
        IReportServerRepository GetRepository(string name);
    }
}
