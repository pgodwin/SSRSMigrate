using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using System.Net;
using SSRSMigrate.ReportServer2010;

namespace SSRSMigrate.TestHelper
{
    public static class ReportingService2010TestEnvironment
    {
        // Data to create on environment setup
        private static List<FolderItem> SetupFolderItems = new List<FolderItem>()
        {
            // Create a folder named 'Folder Already Exists' so when we run integration tests that expect
            //  this folder to exist, it does
            new FolderItem()
            {
                Name = "Folder Already Exists",
                Path = "{0}/Folder Already Exists",
            },
            new FolderItem()
            {
                Name = "Data Sources",
                Path = "{0}/Data Sources"
            },
            new FolderItem()
            {
                Name = "Sub Folder",
                Path = "{0}/Reports/Sub Folder"
            }
        };

        private static List<DataSourceItem> SetupDataSourceItems = new List<DataSourceItem>()
        {
            // Create a data source named 'Data Source Already Exists' so when we run integration tests that expect
            //  this data source to exist, it does
            new DataSourceItem()
            {
                Name = "Data Source Already Exists",
                Path = "{0}/Data Sources/Data Source Already Exists",
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
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

        private static List<ReportItem> SetupReportItems = new List<ReportItem>()
        {
            // Create a report named 'Report Already Exists' so when we run integration tests that expect
            //  this report to exist, it does
            new ReportItem()
            {
                Name = "Report Already Exists",
                Path = "{0}/Reports/Report Already Exists",
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Company Sales.rdl"))
            }
        };

        /// <summary>
        /// Gets the reporting service object.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">url</exception>
        private static ReportingService2010 GetReportingService(string url, ICredentials credentials)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (credentials == null)
                credentials = CredentialCache.DefaultNetworkCredentials;

            if (!url.EndsWith("reportservice2005.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2005.asmx", url);
            }

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            service.Credentials = credentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return service;
        }
    
        public static void SetupEnvironment(string url,
            ICredentials credentials,
            string path)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            ReportingService2010 service = ReportingService2010TestEnvironment.GetReportingService(url, credentials);

            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);

            // Create the parent (base) folder where the integration tests will be written to (e.g. /DEST_SSRSMigrate_Tests)
            ReportingService2010TestEnvironment.CreateFolderFromPath(service, path);

            // Create folder structure
            ReportingService2010TestEnvironment.CreateFolders(service, path);

            // Create the the data sources
            ReportingService2010TestEnvironment.CreateDataSources(service, path);

            // Create the the reports
            //ReportingService2010TestEnvironment.CreateReports(service, path);
        }

        /// <summary>
        /// Setups the folder writer environment.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="path">The parent path to write items to (e.g. /SSRSMigrate_Tests).</param>
        /// <param name="folders">The folders to create.</param>
        /// <exception cref="System.ArgumentException">url</exception>
        /// <exception cref="System.ArgumentNullException">folders</exception>
        public static void SetupFolderWriterEnvironment(string url,
            ICredentials credentials,
            string path,
            List<FolderItem> folders)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("path");

            if (folders == null)
                throw new ArgumentNullException("folders");

            ReportingService2010 service = ReportingService2010TestEnvironment.GetReportingService(url, credentials);

            // If the path exists, delete it so we don't get any unexpected 'ItemAlreadyExists' exceptions while testing
            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);

            // Go through each folder and if it exists, delete it and then recreate it
            foreach (FolderItem folder in folders)
            {
                if (ReportingService2010TestEnvironment.ItemExists(service, folder.Path, "Folder"))
                    service.DeleteItem(folder.Path);

                ReportingService2010TestEnvironment.CreateFolderFromPath(service, folder.Path);
            }
        }

        public static void SetupReportWriterEnvironment(string url,
            ICredentials credentials,
            string path,
            List<ReportItem> reports)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("path");

            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("path");

            if (reports == null)
                throw new ArgumentNullException("reports");

            ReportingService2010 service = ReportingService2010TestEnvironment.GetReportingService(url, credentials);

            // If the path exists, delete it so we don't get any unexpected 'ItemAlreadyExists' exceptions while testing
            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);

            // Go through each report and if it exists, delete it and then recreate it
            foreach (ReportItem report in reports)
            {
                if (ReportingService2010TestEnvironment.ItemExists(service, report.Path, "Report"))
                    service.DeleteItem(report.Path);

                ReportingService2010TestEnvironment.CreateReport(service, report);
            }
        }

        /// <summary>
        /// Teardowns the environment.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="path">The path to delete.</param>/param>
        /// <exception cref="System.ArgumentException">
        /// url
        /// or
        /// path
        /// </exception>
        public static void TeardownEnvironment(string url,
            ICredentials credentials,
            string path)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            ReportingService2010 service = ReportingService2010TestEnvironment.GetReportingService(url, credentials);

            // If the path exists, delete it to clean up after the tests
            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);

            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);
        }

        /// <summary>
        /// Teardowns the folder writer environment.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="folders">The folders to delete.</param>
        /// <exception cref="System.ArgumentException">url</exception>
        /// <exception cref="System.ArgumentNullException">folders</exception>
        public static void TeardownFolderWriterEnvironment(string url,
            ICredentials credentials,
            string path,
            List<FolderItem> folders)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (folders == null)
                throw new ArgumentNullException("folders");

            ReportingService2010 service = ReportingService2010TestEnvironment.GetReportingService(url, credentials);

            // Go through each folder and delete it if it exists
            foreach (FolderItem folder in folders)
            {
                if (ReportingService2010TestEnvironment.ItemExists(service, folder.Path, "Folder"))
                    service.DeleteItem(folder.Path);
            }

            // Delete the path if it exists
            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);
        }

        public static void TeardownReportWriterEnvironment(string url,
            ICredentials credentials,
            string path,
            List<ReportItem> reports)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("url");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (reports == null)
                throw new ArgumentNullException("reports");

            ReportingService2010 service = ReportingService2010TestEnvironment.GetReportingService(url, credentials);

            // Go through each folder and delete it if it exists
            foreach (ReportItem report in reports)
            {
                if (ReportingService2010TestEnvironment.ItemExists(service, report.Path, "Folder"))
                    service.DeleteItem(report.Path);
            }

            // Delete the path if it exists
            if (ReportingService2010TestEnvironment.ItemExists(service, path, "Folder"))
                service.DeleteItem(path);
        }


        /// <summary>
        /// Creates the folder structure of a given path. This will go through each folder in the path and create it.
        /// </summary>
        /// <param name="reportingService">The reporting service.</param>
        /// <param name="path">The path.</param>
        /// <exception cref="System.ArgumentNullException">reportingService</exception>
        private static void CreateFolderFromPath(ReportingService2010 reportingService, string path)
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

                if (!ReportingService2010TestEnvironment.ItemExists(reportingService, folderPath, "Folder"))
                {
                    reportingService.CreateFolder(folder, parentPath, null);
                }

                if (parentPath != "/")
                    parentPath += "/" + folder;
                else
                    parentPath += folder;
            }
        }

        /// <summary>
        /// Checks if an item of the specified type exists at path.
        /// </summary>
        /// <param name="reportingService">The reporting service.</param>
        /// <param name="path">The path.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">reportingService</exception>
        private static bool ItemExists(ReportingService2010 reportingService, string path, string itemType)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            if (string.IsNullOrEmpty(itemType))
                throw new ArgumentException("itemType");

            string actualItemType = reportingService.GetItemType(path);

            if (itemType == actualItemType)
                return true;
            else
                return false;
        }

        private static void CreateFolders(ReportingService2010 reportingService, string path)
        {
            foreach (FolderItem folder in SetupFolderItems)
            {
                string fullPath = string.Format(folder.Path, path);

                ReportingService2010TestEnvironment.CreateFolderFromPath(reportingService, fullPath);
            }
        }

        private static void CreateDataSource(ReportingService2010 reportingService, DataSourceItem dataSource)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

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

            string parent = TesterUtility.GetParentPath(dataSource.Path);

            ReportingService2010TestEnvironment.CreateFolderFromPath(reportingService, parent);

            reportingService.CreateDataSource(dataSource.Name, parent, true, def, null);
        }

        private static void CreateDataSources(ReportingService2010 reportingService, string path)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            foreach (DataSourceItem dataSource in SetupDataSourceItems)
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

                string fullPath = string.Format(dataSource.Path, path);
                string parent = TesterUtility.GetParentPath(fullPath);

                reportingService.CreateDataSource(dataSource.Name, parent, true, def, null);
            }
        }

        private static void CreateReport(ReportingService2010 reportingService, ReportItem report)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            if (report == null)
                throw new ArgumentNullException("report");

            string parent = TesterUtility.GetParentPath(report.Path);

            ReportingService2010TestEnvironment.CreateFolderFromPath(reportingService, parent);

            Warning[] warnings;

            reportingService.CreateCatalogItem("Report", 
                report.Name, 
                parent, 
                true, 
                report.Definition, 
                null, 
                out warnings);
        }

        private static void CreateReports(ReportingService2010 reportingService, string path)
        {
            if (reportingService == null)
                throw new ArgumentNullException("reportingService");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            foreach (ReportItem report in SetupReportItems)
            {
                string fullPath = string.Format(report.Path, path);
                string parent = TesterUtility.GetParentPath(fullPath);

                Warning[] warnings;
                reportingService.CreateCatalogItem("Report",
                    report.Name,
                    parent,
                    true,
                    report.Definition,
                    null,
                    out warnings);
            }
        }
    
    }
}
