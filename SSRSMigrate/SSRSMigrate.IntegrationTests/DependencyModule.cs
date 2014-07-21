using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject.Activation;
using SSRSMigrate.SSRS;
using SSRSMigrate.ReportServer2005;
using System.Net;

namespace SSRSMigrate.IntegrationTests
{
    public class DependencyModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IReportServerRepository>().ToProvider(new ReportServerRepositoryProvider());
        }
    }

    public class ReportServerRepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            ReportingService2005 service = new ReportingService2005();
            service.Url = "http://localhost/ReportServer/reportservice2005.asmx";

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServerRepository(service);
        }
    }
}
