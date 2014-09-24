using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ninject;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.Exporter;
using System.IO;
using SSRSMigrate.SSRS.Item;
using Newtonsoft.Json;
using SSRSMigrate.TestHelper;
using SSRSMigrate.Utility;
using SSRSMigrate.Factory;
using Ninject.Extensions.Logging.Log4net;

namespace SSRSMigrate.IntegrationTests.EndToEnd_Export.ReportServer2010
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ExportEverythingTests
    {
        StandardKernel kernel = null;
        ReportServerReader reader = null;

        // Item Exporters
        ReportItemExporter reportExporter = null;
        DataSourceItemExporter dataSourceExporter = null;
        FolderItemExporter folderExporter = null;

        // Path where the test exports will go
        string outputPath = null;

        string[] testReportFiles = new string[]
        {
            "Test AW Reports\\2010\\Company Sales.rdl",
            "Test AW Reports\\2010\\Sales Order Detail.rdl",
            "Test AW Reports\\2010\\Store Contacts.rdl"
        };

        #region Expected Values
        // ReportItem
        List<ReportItem> expectedReportItems = null;
        ReportItem expectedReportItem_CompanySales;
        ReportItem expectedReportItem_SalesOrderDetail;
        ReportItem expectedReportItem_StoreContacts;

        // DataSourceItem
        List<DataSourceItem> expectedDataSourceItems = null;
        string expectedDataSource1Json = null;
        string expectedDataSource2Json = null;

        List<FolderItem> expectedFolderItems = null;
        #endregion

        #region Actual Values
        List<ReportItem> actualReportItems = null;
        List<DataSourceItem> actualDataSourceItems = null;
        List<FolderItem> actualFolderItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            kernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new DependencyModule());

            EnvironmentSetup();
            outputPath = GetOutPutPath();
            SetupExpectedValues();

            // Create the IItemExporter objects
            reportExporter = kernel.Get<ReportItemExporter>();
            dataSourceExporter = kernel.Get<DataSourceItemExporter>();
            folderExporter = kernel.Get<FolderItemExporter>();

            reader = kernel.Get<IReportServerReaderFactory>().GetReader<ReportServerReader>("2010-SRC");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EnvironmentTearDown();
        }

        [SetUp]
        public void SetUp()
        {
            actualReportItems = new List<ReportItem>();
            actualDataSourceItems = new List<DataSourceItem>();
            actualFolderItems = new List<FolderItem>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(outputPath).GetDirectories())
                dir.Delete(true);

            actualReportItems = null;
            actualDataSourceItems = null;
            actualFolderItems = null;
        }

        #region Setup expected values
        private void SetupExpectedValues()
        {
            // ReportItem
            expectedReportItem_CompanySales = new ReportItem()
            {
                Name = "Company Sales",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = "Adventure Works sales by quarter and product category. This report illustrates the use of a tablix data region with nested row groups and column groups. You can drilldown from summary data into detail data by showing and hiding rows. This report also illustrates the use of a logo image and a background image.",
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[0]))
            };

            expectedReportItem_StoreContacts = new ReportItem()
            {
                Name = "Store Contacts",
                Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
                Description = "AdventureWorks store contacts. This report is a subreport used in Sales Order Detail to show all contacts for a store. Borderstyle is None so lines do not display in main report.",
                ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[2])),
            };

            expectedReportItem_SalesOrderDetail = new ReportItem()
            {
                Name = "Sales Order Detail",
                Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
                Description = "Detail of an individual Adventure Works order. This report can be accessed as a drillthrough report from the Employee Sales Summary and Territory Sales drilldown report. This report illustrates the use of a free form layout, a table, parameters, a subreport that shows multiple store contacts, and expressions.",
                ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[1])),
                SubReports = new List<ReportItem>()
                {
                    expectedReportItem_StoreContacts
                }
            };

            expectedReportItems = new List<ReportItem>()
            {
                expectedReportItem_CompanySales,
                expectedReportItem_SalesOrderDetail,
                expectedReportItem_StoreContacts
            };

            // DataSourceItem
            expectedDataSourceItems = new List<DataSourceItem>()
            {
                new DataSourceItem()
                {
                    Description = null,
                    VirtualPath = null,
                    Name = "AWDataSource",
                    Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource",
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
                },
               new DataSourceItem()
                {
                    Description = null,
                    VirtualPath = null,
                    Name = "Test Data Source",
                    Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
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
                },
            };

            expectedDataSource1Json = JsonConvert.SerializeObject(expectedDataSourceItems[0], Formatting.Indented);
            expectedDataSource2Json = JsonConvert.SerializeObject(expectedDataSourceItems[1], Formatting.Indented);

            // FolderItem
            expectedFolderItems = new List<FolderItem>()
            {
                new FolderItem()
                {
                    Name = "Reports",
                    Path = "/SSRSMigrate_AW_Tests/Reports",
                },
                new FolderItem()
                {
                    Name = "Sub Folder",
                    Path = "/SSRSMigrate_AW_Tests/Reports/Sub Folder",
                },
                new FolderItem()
                {
                    Name = "Data Sources",
                    Path = "/SSRSMigrate_AW_Tests/Data Sources",
                }
            };
        }
        #endregion

        #region Environment Setup/Teardown
        private string GetOutPutPath()
        {
            string outputPath = Path.GetTempPath();
            return Path.Combine(outputPath, "Exporter_Output");
        }

        private void EnvironmentSetup()
        {
            Directory.CreateDirectory(GetOutPutPath());
        }

        public void EnvironmentTearDown()
        {
            Directory.Delete(GetOutPutPath(), true);
        }
        #endregion

        #region Export Folders Tests
        [Test]
        public void ExportFolders()
        {
            reader.GetFolders("/SSRSMigrate_AW_Tests", GetFolders_Reporter);

            foreach (FolderItem actualFolderItem in actualFolderItems)
            {
                string saveFilePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(actualFolderItem.Path);

                ExportStatus actualStatus = folderExporter.SaveItem(
                    actualFolderItem,
                    saveFilePath,
                    true);

                // Export was successful
                Assert.True(actualStatus.Success, string.Format("Success; {0}", actualFolderItem.Path));

                // Exported to the expected location
                Assert.AreEqual(saveFilePath, actualStatus.ToPath, string.Format("ToPath; {0}", actualFolderItem.Path));

                // Was exported from the expected location
                Assert.AreEqual(actualFolderItem.Path, actualStatus.FromPath, string.Format("ToPath; {0}", actualFolderItem.Path));

                // The exported FolderItem exists on disk
                Assert.True(Directory.Exists(actualStatus.ToPath));
            }
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion

        #region Export Reports Tests
        [Test]
        public void ExportReports()
        {
            reader.GetReports("/SSRSMigrate_AW_Tests", GetReports_Reporter);

            foreach (ReportItem actualReportItem in actualReportItems)
            {
                string saveFilePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(actualReportItem.Path, "rdl");

                ExportStatus actualStatus = reportExporter.SaveItem(
                    actualReportItem,
                    saveFilePath,
                    true);

                // Export was successful
                Assert.True(actualStatus.Success, string.Format("Success; {0}", actualReportItem.Path));

                // Exported to the expected location
                Assert.AreEqual(saveFilePath, actualStatus.ToPath, string.Format("ToPath; {0}", actualReportItem.Path));

                // Was exported from the expected location
                Assert.AreEqual(actualReportItem.Path, actualStatus.FromPath, string.Format("ToPath; {0}", actualReportItem.Path));

                // The exported ReportItem exists on disk
                Assert.True(File.Exists(actualStatus.ToPath));
            }

            // The exported reports file matches the expected file
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[0], outputPath + "\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[1], outputPath + "\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[2], outputPath + "\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl"));
        }

        private void GetReports_Reporter(ReportItem reportItem)
        {
            actualReportItems.Add(reportItem);
        }
        #endregion

        #region Export DataSources Tests
        [Test]
        public void ExportDataSources()
        {
            reader.GetDataSources("/SSRSMigrate_AW_Tests", GetDataSources_Reporter);

            foreach (DataSourceItem actualDataSourceItem in actualDataSourceItems)
            {
                string saveFilePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(actualDataSourceItem.Path, "json");

                ExportStatus actualStatus = dataSourceExporter.SaveItem(
                    actualDataSourceItem,
                    saveFilePath,
                    true);

                // Export was successful
                Assert.True(actualStatus.Success, string.Format("Success; {0}", actualDataSourceItem.Path));

                // Was exported to the expected location
                Assert.AreEqual(saveFilePath, actualStatus.ToPath, string.Format("ToPath; {0}", actualDataSourceItem.Path));

                // Was exported from the expected location
                Assert.AreEqual(actualDataSourceItem.Path, actualStatus.FromPath, string.Format("ToPath; {0}", actualDataSourceItem.Path));

                // The exported DataSourceItem exists on disk
                Assert.True(File.Exists(actualStatus.ToPath));
            }
        }

        private void GetDataSources_Reporter(DataSourceItem dataSourceItem)
        {
            actualDataSourceItems.Add(dataSourceItem);
        }
        #endregion
    }
}
