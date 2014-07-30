using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject.Activation;
using SSRSMigrate.ReportServer2005;
using System.Net;
using SSRSMigrate.SSRS.Repository;

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
            string url = Properties.Settings.Default.ReportServer2008WebServiceUrl;
            string path = Properties.Settings.Default.SourcePath;

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2008Repository(path, service);
        }
    }
}
