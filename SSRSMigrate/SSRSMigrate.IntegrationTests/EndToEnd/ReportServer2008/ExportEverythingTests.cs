using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Exporter;
using System.IO;
using System.Reflection;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using Newtonsoft.Json;
using SSRSMigrate.Utility;

namespace SSRSMigrate.IntegrationTests.EndToEnd.ReportServer2008
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ExportSSRSFolderTests
    {
        ReportServerReader reader = null;

        // Export Writers
        FileExportWriter fileWriter = null;
        FolderExportWriter folderWriter = null;

        // Item Exporters
        ReportItemExporter reportExporter = null;
        DataSourceItemExporter dataSourceExporter = null;
        FolderItemExporter folderExporter = null;

        // Path where the test exports will go
        string outputPath = null;

        string[] testReportFiles = new string[]
        {
            "Test Reports\\2008\\Inquiry.rdl",
            "Test Reports\\2008\\Listing.rdl",
            "Test Reports\\2008\\SUB-Addresses.rdl",
            "Test Reports\\2008\\SUB-Categories.rdl",
            "Test Reports\\2008\\SUB-Phone Numbers.rdl",
            "Test Reports\\2008\\SUB-Related Contacts.rdl",
            "Test Reports\\2008\\SUB-Related Matters.rdl"
        };

        #region Expected Values
        // ReportItem
        List<ReportItem> expectedReportItems = null;
        ReportItem expectedReportItem_Inquiry;
        ReportItem expectedReportItem_Listing;
        ReportItem expectedReportItem_SUBAddress;
        ReportItem expectedReportItem_SUBCategories;
        ReportItem expectedReportItem_SUBPhoneNumbers;
        ReportItem expectedReportItem_SUBRelatedContacts;
        ReportItem expectedReportItem_SUBRelatedMatters;

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
            EnvironmentSetup();
            outputPath = GetOutPutPath();
            SetupExpectedValues();

            // Create the IExportWriter objects
            fileWriter = new FileExportWriter();
            folderWriter = new FolderExportWriter();

            // Create the IItemExporter objects
            reportExporter = new ReportItemExporter(fileWriter);
            dataSourceExporter = new DataSourceItemExporter(fileWriter);
            folderExporter = new FolderItemExporter(folderWriter);

            reader = DependencySingleton.Instance.Get<ReportServerReader>();
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
            //foreach (DirectoryInfo dir in new DirectoryInfo(outputPath).GetDirectories())
            //    dir.Delete(true);

            actualReportItems = null;
            actualDataSourceItems = null;
            actualFolderItems = null;
        }

        #region Setup Expected Values
        private void SetupExpectedValues()
        {
            // ReportItem
            expectedReportItem_Inquiry = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[0]))
            };

            expectedReportItem_SUBAddress = new ReportItem()
            {
                Name = "SUB-Address",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Address",
                Description = null,
                ID = "77b2135b-c52f-4a52-9406-7bd523ad9623",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[2])),
            };

            expectedReportItem_SUBCategories = new ReportItem()
            {
                Name = "SUB-Categories",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Categories",
                Description = null,
                ID = "ab67975e-8535-4cca-88d8-79a1827a099e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[3])),
            };

            expectedReportItem_SUBPhoneNumbers = new ReportItem()
            {
                Name = "SUB-Phone Numbers",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Phone Numbers",
                Description = null,
                ID = "7b64b5e4-4ca2-466c-94ce-19d32d8222f5",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[4])),
            };

            expectedReportItem_SUBRelatedContacts = new ReportItem()
            {
                Name = "SUB-Related Contacts",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Contacts",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[5])),
            };

            expectedReportItem_SUBRelatedMatters = new ReportItem()
            {
                Name = "SUB-Related Matters",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Matters",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[6])),
            };

            expectedReportItem_Listing = new ReportItem()
            {
                Name = "Listing",
                Path = "/SSRSMigrate_Tests/Reports/Listing",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[1])),
                SubReports = new List<ReportItem>()
                {
                    expectedReportItem_SUBAddress,
                    expectedReportItem_SUBCategories,
                    expectedReportItem_SUBPhoneNumbers,
                    expectedReportItem_SUBRelatedContacts,
                    expectedReportItem_SUBRelatedMatters
                }
            };

            expectedReportItems = new List<ReportItem>()
            {
                expectedReportItem_Inquiry,
                expectedReportItem_Listing,
                expectedReportItem_SUBAddress,
                expectedReportItem_SUBCategories,
                expectedReportItem_SUBPhoneNumbers,
                expectedReportItem_SUBRelatedContacts,
                expectedReportItem_SUBRelatedMatters
            };

            // DataSourceItem
            expectedDataSourceItems = new List<DataSourceItem>()
            {
                new DataSourceItem()
                {
                    Description = null,
                    Name = "Test Data Source",
                    Path = "/SSRSMigrate_Tests/Test Data Source",
                    ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                    CredentialsRetrieval ="Integrated",
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
                    Name = "Test 2 Data Source",
                    Path = "/SSRSMigrate_Tests/Test 2 Data Source",
                    ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                    CredentialsRetrieval ="Integrated",
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
                    Path = "/SSRSMigrate_Tests/Reports",
                },
                new FolderItem()
                {
                    Name = "Sub Folder",
                    Path = "/SSRSMigrate_Tests/Reports/Sub Folder",
                },
                new FolderItem()
                {
                    Name = "Test Folder",
                    Path = "/SSRSMigrate_Tests/Test Folder",
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
            //Directory.Delete(GetOutPutPath(), true);
        }
        #endregion

        #region ExportFolders Tests
        [Test]
        public void ExportFolders()
        {
            reader.GetFolders("/SSRSMigrate_Tests", GetFolders_Reporter);

            foreach (FolderItem actualFolderItem in actualFolderItems)
            {
                string saveFilePath =  outputPath + SSRSUtil.GetServerPathToPhysicalPath(actualFolderItem.Path);

                ExportStatus actualStatus = folderExporter.SaveItem(
                    actualFolderItem, 
                    saveFilePath, 
                    true);
            }

            Assert.True(Directory.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports"));
            Assert.True(Directory.Exists(outputPath + "\\SSRSMigrate_Tests\\Test Folder"));
            Assert.True(Directory.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\Sub Folder"));
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion

        #region ExportReports Tests
        [Test]
        public void ExportReports()
        {
            reader.GetReports("/SSRSMigrate_Tests", GetReports_Reporter);

            foreach (ReportItem actualReportItem in actualReportItems)
            {
                string saveFilePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(actualReportItem.Path, "rdl");

                ExportStatus actualStatus = reportExporter.SaveItem(
                    actualReportItem,
                    saveFilePath,
                    true);
            }

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\Inquiry.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[0], outputPath + "\\SSRSMigrate_Tests\\Reports\\Inquiry.rdl"));

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\Listing.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[1], outputPath + "\\SSRSMigrate_Tests\\Reports\\Listing.rdl"));

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Addresses.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[2], outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Addresses.rdl"));

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Categories.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[3], outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Categories.rdl"));

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Phone Numbers.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[4], outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Phone Numbers.rdl"));

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Related Contacts.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[5], outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Related Contacts.rdl"));

            Assert.True(File.Exists(outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Related Matters.rdl"));
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[6], outputPath + "\\SSRSMigrate_Tests\\Reports\\SUB-Related Matters.rdl"));
        }

        private void GetReports_Reporter(ReportItem reportItem)
        {
            actualReportItems.Add(reportItem);
        }
        #endregion
    }
}
