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
using Ninject;
using Ninject.Extensions.Logging.Log4net;

namespace SSRSMigrate.IntegrationTests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportItem2005ExporterTests
    {
        ReportItemExporter exporter = null;
        StandardKernel kernel = null;

        List<ReportItem> reportItems = null;
        ReportItem reportItem_CompanySales;
        ReportItem reportItem_SalesOrderDetail;
        ReportItem reportItem_StoreContacts;

        string[] testReportFiles = new string[]
        {
            "Test AW Reports\\2005\\Company Sales.rdl",
            "Test AW Reports\\2005\\Sales Order Detail.rdl",
            "Test AW Reports\\2005\\Store Contacts.rdl",
        };

        string outputPath = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EnvironmentSetup();

            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            kernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new DependencyModule());

            exporter = kernel.Get<ReportItemExporter>();

            reportItem_CompanySales = new ReportItem()
            {
                Name = "Company Sales",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = "Adventure Works sales by quarter and product category. This report illustrates the use of a tablix data region with nested row groups and column groups. You can drilldown from summary data into detail data by showing and hiding rows. This report also illustrates the use of a logo image and a background image.",
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[0]))
            };

            reportItem_StoreContacts = new ReportItem()
            {
                Name = "Store Contacts",
                Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
                Description = "AdventureWorks store contacts. This report is a subreport used in Sales Order Detail to show all contacts for a store. Borderstyle is None so lines do not display in main report.",
                ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[2])),
            };

            reportItem_SalesOrderDetail = new ReportItem()
            {
                Name = "Sales Order Detail",
                Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
                Description = "Detail of an individual Adventure Works order. This report can be accessed as a drillthrough report from the Employee Sales Summary and Territory Sales drilldown report. This report illustrates the use of a free form layout, a table, parameters, a subreport that shows multiple store contacts, and expressions.",
                ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportFiles[1])),
                SubReports = new List<ReportItem>()
                {
                    reportItem_StoreContacts
                }
            };

            // Setup GetReports - Expected ReportItems
            reportItems = new List<ReportItem>()
            {
                reportItem_CompanySales,
                reportItem_SalesOrderDetail,
                reportItem_StoreContacts
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
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem_CompanySales.Path, "rdl");

            ExportStatus actualStatus = exporter.SaveItem(reportItem_CompanySales, filePath);

            Assert.True(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.True(File.Exists(actualStatus.ToPath));
            Assert.Null(actualStatus.Errors);
            Assert.True(TesterUtility.CompareTextFiles(testReportFiles[0], actualStatus.ToPath));
        }

        [Test]
        public void ExportReportItem_NullItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem_CompanySales.Path, "rdl");

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
                    exporter.SaveItem(reportItem_CompanySales, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportReportItem_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(reportItem_CompanySales, "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportReportItem_FileDontOverwrite()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(reportItem_CompanySales.Path, "rdl");

            // Create dummy file, so the output file already exists, causing the SaveItem to fail
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, "DUMMY FILE");

            ExportStatus actualStatus = exporter.SaveItem(reportItem_CompanySales, filePath, false);

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
