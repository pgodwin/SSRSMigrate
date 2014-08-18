using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SSRSMigrate.ReportServer2005;
using System.Web.Services.Protocols;

namespace SSRSMigrate.TestHelper
{
    public static class ReportingService2005Utility
    {
        public static void SetupEnvironment(string url,
            ICredentials credentials,
            string path)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (credentials == null)
                credentials = CredentialCache.DefaultNetworkCredentials;

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (!url.EndsWith("reportservice2005.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2005.asmx", url);
            }

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            service.Credentials = credentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            string name = path.Substring(path.LastIndexOf("/") + 1);
            string parent = TesterUtility.GetParentPath(path);

            try
            {
                service.CreateFolder(name, parent, null);
            }
            catch (SoapException er)
            {
                if (er.Message.Contains("ItemAlreadyExists"))
                {
                    service.DeleteItem(path);

                    service.CreateFolder(name, parent, null);
                }
            }
        }

        public static void TeardownEnvironment(string url,
            ICredentials credentials,
            string path)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (credentials == null)
                credentials = CredentialCache.DefaultNetworkCredentials;

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (!url.EndsWith("reportservice2005.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2005.asmx", url);
            }

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            service.Credentials = credentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            service.DeleteItem(path);
        }   
    }
}
