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

            if (ReportingService2005Utility.ItemExists(service, path, ItemTypeEnum.Folder))
                service.DeleteItem(path);

            ReportingService2005Utility.CreateFolderFromPath(service, path);
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

            if (ReportingService2005Utility.ItemExists(service, path, ItemTypeEnum.Folder))
                service.DeleteItem(path);
        }

        private static void CreateFolderFromPath(ReportingService2005 reportingService, string path)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            string [] folders = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
	
	        string parentPath = "/";

            foreach (string folder in folders)
            {
                string folderPath;

                if (parentPath != "/" && parentPath.EndsWith("/"))
                    parentPath = parentPath.Substring(0, parentPath.LastIndexOf("/"));

                if (parentPath == "/")
                    folderPath = parentPath + folder;
                else
                    folderPath = parentPath + "/" + folder;

                if (!ReportingService2005Utility.ItemExists(reportingService, folderPath, ItemTypeEnum.Folder))
                {
                    reportingService.CreateFolder(folder, parentPath, null);
                }

                if (parentPath != "/")
                    parentPath += "/" + folder;
                else
                    parentPath += folder;
            }
        }

        private static bool ItemExists(ReportingService2005 reportingService, string path, ItemTypeEnum itemType)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            ItemTypeEnum actualItemType = reportingService.GetItemType(path);

            if (itemType == actualItemType)
                return true;
            else
                return false;
        }
    }
}
