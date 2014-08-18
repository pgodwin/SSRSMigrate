using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Reader;
using Ninject.Parameters;

namespace SSRSMigrate.Factory
{
    public class ReportServerReaderFactory : IReportServerReaderFactory
    {
        private readonly IKernel mKernel;

        public ReportServerReaderFactory(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            this.mKernel = kernel;
        }

        public T GetReader<T>(string name)
        {
            IReportServerRepository repository = this.mKernel.Get<IReportServerRepository>(name);

            return (T)this.mKernel.Get<IReportServerReader>(name,
                new ConstructorArgument("repository", repository));
        }
    }
}
