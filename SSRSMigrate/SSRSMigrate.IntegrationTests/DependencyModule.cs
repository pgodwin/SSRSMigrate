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
    [CoverageExcludeAttribute]
    public class DependencyModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IReportServerRepository>().ToProvider(new ReportServer2008RepositoryProvider());
        }
    }

    [CoverageExcludeAttribute]
    public class ReportServer2008RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            ReportingService2005 service = new ReportingService2005();
            service.Url = "http://localhost/ReportServer_SQL2008/reportservice2005.asmx";

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2008Repository("/SSRSMigrate_Tests", service);
        }
    }
}
