using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Activation;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.ReportServer2005;
using Ninject.Modules;
using System.Net;
using Ninject.Parameters;
using SSRSMigrate.Factory;

namespace SSRSMigrate
{
    [CoverageExcludeAttribute]
    public class DependencyModule : NinjectModule
    {
        private readonly bool mReportServer2005 = true;
        private readonly string mRootPath = null;
        private string mWebServiceUrl = null;
        private readonly bool mDefaultCredentials = true;
        private readonly string mUsername = null;
        private readonly string mPassword = null;
        private readonly string mDomain = null;

        public DependencyModule(
            bool reportServer2005,
            string rootPath,
            string webServiceUrl,
            bool defaultCredentials,
            string username,
            string password,
            string domain)
        {
            this.mReportServer2005 = reportServer2005;
            this.mRootPath = rootPath;
            this.mWebServiceUrl = webServiceUrl;
            this.mDefaultCredentials = defaultCredentials;
            this.mUsername = username;
            this.mPassword = password;
            this.mDomain = domain;
        }

        public override void Load()
        {
            if (this.mReportServer2005 == true)
            {
                if (!this.mWebServiceUrl.EndsWith("reportservice2005.asmx"))
                    if (this.mWebServiceUrl.EndsWith("/"))
                        this.mWebServiceUrl = this.mWebServiceUrl.Substring(0, this.mWebServiceUrl.Length - 1);

                    this.mWebServiceUrl = string.Format("{0}/reportservice2005.asmx",this.mWebServiceUrl);

                ReportingService2005 service = new ReportingService2005();
                service.Url = this.mWebServiceUrl;

                if (this.mDefaultCredentials)
                {
                    service.Credentials = CredentialCache.DefaultNetworkCredentials;
                    service.PreAuthenticate = true;
                    service.UseDefaultCredentials = true;
                }
                else
                {
                    service.Credentials = new NetworkCredential(this.mUsername, this.mPassword, this.mDomain);
                    service.UseDefaultCredentials = false;
                }

                this.Bind<IReportServerRepository>().To<ReportServer2005Repository>().WithConstructorArgument("rootPath", this.mRootPath).WithConstructorArgument("reportingService", service);
            }
            else
            {
                if (!this.mWebServiceUrl.EndsWith("reportservice2010.asmx"))
                    if (this.mWebServiceUrl.EndsWith("/"))
                        this.mWebServiceUrl = this.mWebServiceUrl.Substring(0, this.mWebServiceUrl.Length - 1);

                this.mWebServiceUrl = string.Format("{0}/reportservice2010.asmx", this.mWebServiceUrl);

                ReportingService2010 service = new ReportingService2010();
                service.Url = this.mWebServiceUrl;

                if (this.mDefaultCredentials)
                {
                    service.Credentials = CredentialCache.DefaultNetworkCredentials;
                    service.PreAuthenticate = true;
                    service.UseDefaultCredentials = true;
                }
                else
                {
                    service.Credentials = new NetworkCredential(this.mUsername, this.mPassword, this.mDomain);
                    service.UseDefaultCredentials = false;
                }

                this.Bind<IReportServerRepository>().To<ReportServer2010Repository>().WithConstructorArgument("rootPath", this.mRootPath).WithConstructorArgument("reportingService", service);
            }
        }
    }

    public class ReportServerRepositoryModule : NinjectModule
    {
        public override void Load()
        {
            //this.Bind<IReportServerRepository>()
            //   .To<ReportServer2005Repository>()
            //   .InSingletonScope()
            //   .Named("2005")
            //   .WithConstructorArgument(Properties.Settings.Default.SrcPath)
            //   .WithConstructorArgument(GetSourceReportingService2005());

            //this.Bind<IReportServerRepository>()
            //    .To<ReportServer2010Repository>()
            //    .InSingletonScope()
            //    .Named("2010")
            //    .WithConstructorArgument(Properties.Settings.Default.SrcPath)
            //    .WithConstructorArgument(GetSourceReportingService2010());

            this.Bind<IReportServerRepository>()
               .ToProvider<SourceReportServer2005RepositoryProvider>()
               .InSingletonScope()
               .Named("2005-SRC");

            this.Bind<IReportServerRepository>()
               .ToProvider<SourceReportServer2010RepositoryProvider>()
               .InSingletonScope()
               .Named("2010-SRC");

            this.Bind<IReportServerRepositoryFactory>().To<ReportServerRepositoryFactory>();
        }

        private ReportingService2010 GetDestReportingService2010()
        {
            string url = Properties.Settings.Default.DestWebServiceUrl;
            string version = Properties.Settings.Default.DestVersion;
            bool defaultCred = Properties.Settings.Default.DestDefaultCred;
            string username = Properties.Settings.Default.DestUsername;
            string password = Properties.Settings.Default.DestPassword;
            string domain = Properties.Settings.Default.DestDomain;

            if (!url.EndsWith("reportservice2010.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2010.asmx", url);

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                service.UseDefaultCredentials = false;
            }

            return service;
        }
    }

    public class SourceReportServer2005RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.SrcWebServiceUrl;
            string version = Properties.Settings.Default.SrcVersion;
            bool defaultCred = Properties.Settings.Default.SrcDefaultCred;
            string username = Properties.Settings.Default.SrcUsername;
            string password = Properties.Settings.Default.SrcPassword;
            string domain = Properties.Settings.Default.SrcDomain;
            string path = Properties.Settings.Default.SrcPath;

            if (!url.EndsWith("reportservice2005.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2005.asmx", url);

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                service.UseDefaultCredentials = false;
            }

            return new ReportServer2005Repository(path, service);
        }
    }

    public class SourceReportServer2010RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.SrcWebServiceUrl;
            string version = Properties.Settings.Default.SrcVersion;
            bool defaultCred = Properties.Settings.Default.SrcDefaultCred;
            string username = Properties.Settings.Default.SrcUsername;
            string password = Properties.Settings.Default.SrcPassword;
            string domain = Properties.Settings.Default.SrcDomain;
            string path = Properties.Settings.Default.SrcPath;

            if (!url.EndsWith("reportservice2010.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2010.asmx", url);

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                service.UseDefaultCredentials = false;
            }

            return new ReportServer2010Repository(path, service);
        }
    }
}
