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
}
