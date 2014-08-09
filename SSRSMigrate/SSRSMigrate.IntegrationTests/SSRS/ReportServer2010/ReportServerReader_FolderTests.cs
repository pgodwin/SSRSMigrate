using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.ReportServer2010;
using System.Net;
using SSRSMigrate.SSRS.Repository;
using Ninject;

namespace SSRSMigrate.IntegrationTests.SSRS.ReportServer2010
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_FolderTests
    {
        ReportServerReader reader = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            //TODO Need to fix this with a proper IoC container :\
            //string url = Properties.Settings.Default.ReportServer2008R2WebServiceUrl;
            //string path = Properties.Settings.Default.SourcePath;

            //ReportingService2010 service = new ReportingService2010();
            //service.Url = url;

            //service.Credentials = CredentialCache.DefaultNetworkCredentials;
            //service.PreAuthenticate = true;
            //service.UseDefaultCredentials = true;

            //reader = new ReportServerReader(new ReportServer2010Repository(path, service));

            StandardKernel kernel = new StandardKernel(new DependencyModule(false));
            reader = kernel.Get<ReportServerReader>();
        }
    }
}
