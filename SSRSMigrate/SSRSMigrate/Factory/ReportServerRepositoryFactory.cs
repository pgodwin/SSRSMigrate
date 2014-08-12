using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using SSRSMigrate.SSRS.Repository;

namespace SSRSMigrate.Factory
{
    public class ReportServerRepositoryFactory : IReportServerRepositoryFactory
    {
        private readonly IKernel mKernel;

        public ReportServerRepositoryFactory(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            this.mKernel = kernel;
        }

        public IReportServerRepository GetRepository(string name)
        {
            return this.mKernel.Get<IReportServerRepository>(name);
        }
    }
}
