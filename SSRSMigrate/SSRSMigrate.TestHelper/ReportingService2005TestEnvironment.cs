using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SSRSMigrate.ReportServer2005;
using System.Web.Services.Protocols;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.TestHelper
{
    public static class ReportingService2005TestEnvironment
    {
        // Data for checking if an item already exists tests
        private static List<ReportItem> AlreadyExistsReports = new List<ReportItem>()
        {
            new ReportItem()
            {
                Name = "Report Already Exists",
                Path = "{0}/Reports/Report Already Exists",
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            }
        };

        private static List<FolderItem> AlreadyExistsFolders = new List<FolderItem>()
        {
            new FolderItem()
            {
                Name = "Folder Already Exists",
                Path = "{0}/Folder Already Exists",
            }
        };

        private static List<DataSourceItem> AlreadyExistsDataSources = new List<DataSourceItem>()
        {
            new DataSourceItem()
            {
                Name = "Data Source Already Exists",
                Path = "{0}/Data Source Already Exists",
                ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                CredentialsRetrieval = "Integrated",
                Enabled = true,
                EnabledSpecified = true,
                Extension = "SQL",
                ImpersonateUser = false,
                ImpersonateUserSpecified = true,
                OriginalConnectStringExpressionBased = false,
                Password = null,
                Prompt = "Enter a user name and password to access the data source:",
                UseOriginalConnectString = false,
                UserName = null,
                WindowsCredentials = false
            }
        };

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

            if (ReportingService2005TestEnvironment.ItemExists(service, path, ItemTypeEnum.Folder))
                service.DeleteItem(path);

            // Create the parent folder where the integration tests will be written to (e.g. /DEST_SSRSMigrate_Tests)
            ReportingService2005TestEnvironment.CreateFolderFromPath(service, path);

            // Create the the folders used in testing if a folder already exists
            ReportingService2005TestEnvironment.CreateTestAlreadyExistsFolders(service, path);

            // Create the the data sources used in testing if a data source already exists
            ReportingService2005TestEnvironment.CreateTestAlreadyExistsDataSources(service, path);

            // Create the the reports used in testing if a report already exists
            ReportingService2005TestEnvironment.CreateTestAlreadyExistsReports(service, path);
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

            if (ReportingService2005TestEnvironment.ItemExists(service, path, ItemTypeEnum.Folder))
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

                if (!ReportingService2005TestEnvironment.ItemExists(reportingService, folderPath, ItemTypeEnum.Folder))
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

        private static void CreateTestAlreadyExistsFolders(ReportingService2005 reportingService, string path)
        {
            foreach (FolderItem folder in AlreadyExistsFolders)
            {
                string fullPath = string.Format(folder.Path, path);

                ReportingService2005TestEnvironment.CreateFolderFromPath(reportingService, fullPath);
            }
        }

        private static void CreateTestAlreadyExistsDataSources(ReportingService2005 reportingService, string path)
        {
            foreach (DataSourceItem dataSource in AlreadyExistsDataSources)
            {
                DataSourceDefinition def = new DataSourceDefinition();
                def.ConnectString = dataSource.ConnectString;

                switch (dataSource.CredentialsRetrieval)
                {
                    case "Integrated":
                        def.CredentialRetrieval = CredentialRetrievalEnum.Integrated; break;
                    case "None":
                        def.CredentialRetrieval = CredentialRetrievalEnum.None; break;
                    case "Prompt":
                        def.CredentialRetrieval = CredentialRetrievalEnum.Prompt; break;
                    case "Store":
                        def.CredentialRetrieval = CredentialRetrievalEnum.Store; break;
                }

                def.Enabled = dataSource.Enabled;
                def.EnabledSpecified = dataSource.EnabledSpecified;
                def.Extension = dataSource.Extension;
                def.ImpersonateUser = dataSource.ImpersonateUser;
                def.ImpersonateUserSpecified = dataSource.ImpersonateUserSpecified;
                def.OriginalConnectStringExpressionBased = dataSource.OriginalConnectStringExpressionBased;
                def.Password = dataSource.Password;
                def.Prompt = dataSource.Prompt;
                def.UseOriginalConnectString = dataSource.UseOriginalConnectString;
                def.UserName = dataSource.UserName;
                def.WindowsCredentials = dataSource.WindowsCredentials;

                reportingService.CreateDataSource(dataSource.Name, path, true, def, null);
            }
        }

        private static void CreateTestAlreadyExistsReports(ReportingService2005 reportingService, string path)
        {
            foreach (ReportItem report in AlreadyExistsReports)
            {
                reportingService.CreateReport(report.Name, path, true, report.Definition, null);
            }
        }
    }
}
