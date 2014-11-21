using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Parameters;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Test;

namespace SSRSMigrate.Factory
{
    public class ReportServerTesterFactory : IReportServerTesterFactory
    {
        private readonly IKernel mKernel;

        public ReportServerTesterFactory(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            this.mKernel = kernel;
        }

        public T GetTester<T>(string name)
        {
            IReportServerRepository repository = this.mKernel.Get<IReportServerRepository>(name);

            return (T)this.mKernel.Get<IReportServerTester>(name,
                new ConstructorArgument("repository", repository));
        }
    }
}
