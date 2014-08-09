using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject.Activation;
using SSRSMigrate.ReportServer2005;
using System.Net;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.ReportServer2010;

namespace SSRSMigrate.IntegrationTests
{
    [CoverageExcludeAttribute]
    public class DependencyModule : NinjectModule
    {
        private readonly bool mReportServer2005 = true;

        public DependencyModule(bool reportServer2005)
        {
            this.mReportServer2005 = reportServer2005;
        }

        public override void Load()
        {
            if (this.mReportServer2005 == true)
                this.Bind<IReportServerRepository>().ToProvider(new ReportServer2005RepositoryProvider());
            else
                this.Bind<IReportServerRepository>().ToProvider(new ReportServer2010RepositoryProvider());
        }
    }

    [CoverageExcludeAttribute]
    public class ReportServer2005RepositoryProvider : Provider<IReportServerRepository>
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

            return new ReportServer2005Repository(path, service);
        }
    }

    //TODO This doesn't work.
    [CoverageExcludeAttribute]
    public class ReportServer2010RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.ReportServer2008R2WebServiceUrl;
            string path = Properties.Settings.Default.SourcePath;

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2010Repository(path, service);
        }
    }
}
