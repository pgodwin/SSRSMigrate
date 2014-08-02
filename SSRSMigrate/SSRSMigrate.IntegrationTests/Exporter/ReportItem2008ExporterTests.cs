using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using SSRSMigrate.Utility;
using SSRSMigrate.Exporter.Writer;

namespace SSRSMigrate.IntegrationTests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportItem2008ExporterTests
    {
        ReportItemExporter exporter = null;
        FileExportWriter writer = null;

        List<ReportItem> reportItems = null;
        ReportItem reportItem_Inquiry;
        ReportItem reportItem_Listing;
        ReportItem reportItem_SUBAddress;
        ReportItem reportItem_SUBCategories;
        ReportItem reportItem_SUBPhoneNumbers;
        ReportItem reportItem_SUBRelatedContacts;
        ReportItem reportItem_SUBRelatedMatters;

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

        string outputPath = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EnvironmentSetup();

            writer = new FileExportWriter();
            exporter = new ReportItemExporter(writer);

            reportItem_Inquiry = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[0]))
            };

            reportItem_SUBAddress = new ReportItem()
            {
                Name = "SUB-Address",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Address",
                Description = null,
                ID = "77b2135b-c52f-4a52-9406-7bd523ad9623",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[2])),
            };

            reportItem_SUBCategories = new ReportItem()
            {
                Name = "SUB-Categories",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Categories",
                Description = null,
                ID = "ab67975e-8535-4cca-88d8-79a1827a099e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[3])),
            };

            reportItem_SUBPhoneNumbers = new ReportItem()
            {
                Name = "SUB-Phone Numbers",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Phone Numbers",
                Description = null,
                ID = "7b64b5e4-4ca2-466c-94ce-19d32d8222f5",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[4])),
            };

            reportItem_SUBRelatedContacts = new ReportItem()
            {
                Name = "SUB-Related Contacts",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Contacts",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[5])),
            };

            reportItem_SUBRelatedMatters = new ReportItem()
            {
                Name = "SUB-Related Matters",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Matters",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[6])),
            };

            reportItem_Listing = new ReportItem()
            {
                Name = "Listing",
                Path = "/SSRSMigrate_Tests/Reports/Listing",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[1])),
                SubReports = new List<ReportItem>()
                {
                    reportItem_SUBAddress,
                    reportItem_SUBCategories,
                    reportItem_SUBPhoneNumbers,
                    reportItem_SUBRelatedContacts,
                    reportItem_SUBRelatedMatters
                }
            };

            // Setup GetReports - Expected ReportItems
            reportItems = new List<ReportItem>()
            {
                reportItem_Inquiry,
                reportItem_Listing,
                reportItem_SUBAddress,
                reportItem_SUBCategories,
                reportItem_SUBPhoneNumbers,
                reportItem_SUBRelatedContacts,
                reportItem_SUBRelatedMatters
            };

            outputPath = GetOutPutPath();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EnvironmentTearDown();
        }

        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
           foreach (DirectoryInfo dir in new DirectoryInfo(outputPath).GetDirectories())
                dir.Delete(true);
        }

        #region Environment Setup/TearDown
        private string GetOutPutPath()
        {
            string outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(outputPath, "ReportItemExporter_Output");
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

        #region Export Single ReportItem
        [Test]
        public void ExportReportItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem_Inquiry.Path, "rdl");

            ExportStatus actualStatus = exporter.SaveItem(reportItem_Inquiry, filePath);

            Assert.True(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.True(File.Exists(actualStatus.ToPath));
            Assert.Null(actualStatus.Errors);
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[0], actualStatus.ToPath));
        }

        [Test]
        public void ExportReportItem_NullItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem_Inquiry.Path, "rdl");

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    exporter.SaveItem(null, filePath);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
        }

        [Test]
        public void ExportReportItem_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(reportItem_Inquiry, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportReportItem_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(reportItem_Inquiry, "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportReportItem_FileDontOverwrite()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem_Inquiry.Path, "rdl");

            // Create dummy file, so the output file already exists, causing the SaveItem to fail
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, "DUMMY FILE");

            ExportStatus actualStatus = exporter.SaveItem(reportItem_Inquiry, filePath, false);

            Assert.False(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.NotNull(actualStatus.Errors);
            Assert.True(actualStatus.Errors.Any(e => e.Contains(string.Format("File '{0}' already exists.", filePath))));
        }
        #endregion

        #region  Export Many ReportItem
        [Test]
        public void ExportReportItems()
        {
            for (int i = 0; i < reportItems.Count(); i++)
            {
                ReportItem reportItem = reportItems[i];

                string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem.Path, "rdl");

                ExportStatus actualStatus = exporter.SaveItem(reportItem, filePath);

                Assert.True(actualStatus.Success, "Success");
                Assert.AreEqual(filePath, actualStatus.ToPath, "ToPath");
                Assert.True(File.Exists(actualStatus.ToPath)," ToPath.Exists");
                Assert.Null(actualStatus.Errors);
                Assert.True(TesterUtility.CompareTextFiles(testReportFiles[i], actualStatus.ToPath), "CompareTextFiles");
            }
        }
        #endregion
    }
}
