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
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
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
        /// Setups the ReportServerWriter integration tests environment for ReportingService2005
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="path">The parent path to write items to (e.g. /SSRSMigrate_Tests).</param>
        /// <exception cref="System.ArgumentException">
        /// url
        /// or
        /// path
        /// </exception>
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

            // Create the parent (base) folder where the integration tests will be written to (e.g. /DEST_SSRSMigrate_Tests)
            ReportingService2005TestEnvironment.CreateFolderFromPath(service, path);

            // Create folder structure
            ReportingService2005TestEnvironment.CreateFolders(service, path);

            // Create the the data sources
            ReportingService2005TestEnvironment.CreateDataSources(service, path);

            // Create the the reports
            ReportingService2005TestEnvironment.CreateReports(service, path);
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

        /// <summary>
        /// Creates the folder structure of a given path. This will go through each folder in the path and create it.
        /// </summary>
        /// <param name="reportingService">The reporting service.</param>
        /// <param name="path">The path.</param>
        /// <exception cref="System.ArgumentNullException">reportingService</exception>
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

        /// <summary>
        /// Checks if an item of the specified type exists at path.
        /// </summary>
        /// <param name="reportingService">The reporting service.</param>
        /// <param name="path">The path.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">reportingService</exception>
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

        private static void CreateFolders(ReportingService2005 reportingService, string path)
        {
            foreach (FolderItem folder in SetupFolderItems)
            {
                string fullPath = string.Format(folder.Path, path);

                ReportingService2005TestEnvironment.CreateFolderFromPath(reportingService, fullPath);
            }
        }

        private static void CreateDataSources(ReportingService2005 reportingService, string path)
        {
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

        private static void CreateReports(ReportingService2005 reportingService, string path)
        {
            foreach (ReportItem report in SetupReportItems)
            {
                string fullPath = string.Format(report.Path, path);
                string parent = TesterUtility.GetParentPath(fullPath);

                reportingService.CreateReport(report.Name, parent, true, report.Definition, null);
            }
        }
    }
}
