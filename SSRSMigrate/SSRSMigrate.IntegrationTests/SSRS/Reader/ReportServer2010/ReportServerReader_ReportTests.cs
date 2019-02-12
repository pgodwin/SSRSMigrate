using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.IntegrationTests.SSRS.Reader.ReportServer2010
{
    [TestFixture, Category("ConnectsToSSRS")]
    [CoverageExcludeAttribute]
    class ReportServerReader_ReportTests
    {
        IReportServerReader reader = null;

        #region GetReport - Expected ReportItems
        List<ReportItem> expectedReportItems = null;
        ReportItem expectedReportItem_CompanySales;
        ReportItem expectedReportItem_SalesOrderDetail;
        ReportItem expectedReportItem_StoreContacts;
        #endregion

        #region GetReports - Actual ReportItems
        List<ReportItem> actualReportItems = null;
        #endregion

        private void SetupExpectedResults()
        {
            // Setup expected ReportItems
            expectedReportItem_CompanySales = new ReportItem()
            {
                Name = "Company Sales",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = "Adventure Works sales by quarter and product category. This report illustrates the use of a tablix data region with nested row groups and column groups. You can drilldown from summary data into detail data by showing and hiding rows. This report also illustrates the use of a logo image and a background image.",
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            expectedReportItem_StoreContacts = new ReportItem()
            {
                Name = "Store Contacts",
                Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
                Description = "AdventureWorks store contacts. This report is a subreport used in Sales Order Detail to show all contacts for a store. Borderstyle is None so lines do not display in main report.",
                ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Store Contacts.rdl"))),
            };

            expectedReportItem_SalesOrderDetail = new ReportItem()
            {
                Name = "Sales Order Detail",
                Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
                Description = "Detail of an individual Adventure Works order. This report can be accessed as a drillthrough report from the Employee Sales Summary and Territory Sales drilldown report. This report illustrates the use of a free form layout, a table, parameters, a subreport that shows multiple store contacts, and expressions.",
                ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Sales Order Detail.rdl"))),
                SubReports = new List<ReportItem>()
                {
                    expectedReportItem_StoreContacts
                }
            };

            // Setup GetReports - Expected ReportItems
            expectedReportItems = new List<ReportItem>()
            {
                expectedReportItem_CompanySales,
                expectedReportItem_SalesOrderDetail,
                expectedReportItem_StoreContacts
            };
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            SetupExpectedResults();

            reader = TestKernel.Instance.Get<IReportServerReader>("2010-SRC");
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            reader = null;
        }

        [SetUp]
        public void SetUp()
        {
            actualReportItems = new List<ReportItem>();
        }

        [TearDown]
        public void TearDown()
        {
            actualReportItems = null;
        }


        #region GetReport Tests
        [Test]
        public void GetReport()
        {
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_AW_Tests/Reports/Company Sales");

            Assert.NotNull(actualReportItem);
            Assert.AreEqual(expectedReportItem_CompanySales.Name, actualReportItem.Name);
            Assert.AreEqual(expectedReportItem_CompanySales.Path, actualReportItem.Path);
            Assert.AreEqual(expectedReportItem_CompanySales.SubReports.Count(), actualReportItem.SubReports.Count());
            Assert.AreEqual(expectedReportItem_CompanySales.Definition, actualReportItem.Definition, "Report Definition");
        }

        [Test]
        public void GetReport_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetReport(null);
                });

            Assert.That(ex.Message, Is.EqualTo("reportPath"));
        }

        [Test]
        public void GetReport_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetReport("");
                });

            Assert.That(ex.Message, Is.EqualTo("reportPath"));
        }

        [Test]
        public void GetReport_PathDoesntExist()
        {
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_AW_Tests/Reports/Report Doesnt Exist");

            Assert.Null(actualReportItem);
        }

        [Test]
        public void GetReport_WithSubReports()
        {
        }
        #endregion

        #region GetReports Tests
        [Test]
        public void GetReports()
        {
            string path = "/SSRSMigrate_AW_Tests";

            var actual = reader.GetReports(path);

            Assert.AreEqual(expectedReportItems.Count(), actual.Count());
        }

        [Test]
        public void GetReports_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetReports(null);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetReports_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetReports("");
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetReports_PathDoesntExist()
        {
            string path = "/SSRSMigrate_AW_Tests Doesnt Exist";

            Assert.That(() =>reader.GetReports(path), Throws.TypeOf<System.Web.Services.Protocols.SoapException>());
        }
        #endregion

        #region GetReports Using Action<ReportItem> Delegate Tests
        [Test]
        public void GetReports_UsingDelegate()
        {
            string path = "/SSRSMigrate_AW_Tests";

            reader.GetReports(path, GetReports_Reporter);

            Assert.AreEqual(expectedReportItems.Count(), actualReportItems.Count());
        }

        [Test]
        public void GetReports_UsingDelegate_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetReports(null, GetReports_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetReports_UsingDelegate_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetReports("", GetReports_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetReports_UsingDelegate_NullDelegate()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    reader.GetReports("/SSRSMigrate_AW_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));

        }

        [Test]
        public void GetReports_UsingDelegate_PathDoesntExist()
        {
            string path = "/SSRSMigrate_AW_Tests Doesnt Exist";

            Assert.That(() => reader.GetReports(path, GetReports_Reporter), Throws.TypeOf<System.Web.Services.Protocols.SoapException>());
        }

        public void GetReports_Reporter(ReportItem report)
        {
            if (report != null)
                actualReportItems.Add(report);
        }
        #endregion
    }
}
