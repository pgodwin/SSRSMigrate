using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Repository;
using Ninject.Parameters;

namespace SSRSMigrate.IntegrationTests.Factory
{
    public class ReportServerWriterFactory : IReportServerWriterFactory
    {
        private readonly IKernel mKernel;

        public ReportServerWriterFactory(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException("kernel");

            this.mKernel = kernel;
        }

        public T GetWriter<T>(string name)
        {
            IReportServerRepository repository = this.mKernel.Get<IReportServerRepository>(name);

            return (T)this.mKernel.Get<IReportServerWriter>(name,
                new ConstructorArgument("repository", repository));
        }
    }
}
