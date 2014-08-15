using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using Ninject;
using SSRSMigrate.IntegrationTests.Factory;

namespace SSRSMigrate.IntegrationTests.SSRS.Reader.ReportServer2010
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_ReportTests
    {
        StandardKernel kernel = null;
        ReportServerReader reader = null;

        #region GetReport - Expected ReportItems
        List<ReportItem> expectedReportItems = null;
        ReportItem expectedReportItem_Inquiry;
        ReportItem expectedReportItem_Listing;
        ReportItem expectedReportItem_SUBAddress;
        ReportItem expectedReportItem_SUBCategories;
        ReportItem expectedReportItem_SUBPhoneNumbers;
        ReportItem expectedReportItem_SUBRelatedContacts;
        ReportItem expectedReportItem_SUBRelatedMatters;
        #endregion

        #region GetReports - Actual ReportItems
        List<ReportItem> actualReportItems = null;
        #endregion

        private void SetupExpectedResults()
        {
            // Setup expected ReportItems
            expectedReportItem_Inquiry = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\Inquiry.rdl"))
            };

            expectedReportItem_SUBAddress = new ReportItem()
            {
                Name = "SUB-Address",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Address",
                Description = null,
                ID = "77b2135b-c52f-4a52-9406-7bd523ad9623",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\SUB-Addresses.rdl")),
            };

            expectedReportItem_SUBCategories = new ReportItem()
            {
                Name = "SUB-Categories",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Categories",
                Description = null,
                ID = "ab67975e-8535-4cca-88d8-79a1827a099e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\SUB-Categories.rdl")),
            };

            expectedReportItem_SUBPhoneNumbers = new ReportItem()
            {
                Name = "SUB-Phone Numbers",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Phone Numbers",
                Description = null,
                ID = "7b64b5e4-4ca2-466c-94ce-19d32d8222f5",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\SUB-Phone Numbers.rdl")),
            };

            expectedReportItem_SUBRelatedContacts = new ReportItem()
            {
                Name = "SUB-Related Contacts",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Contacts",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\SUB-Related Contacts.rdl")),
            };

            expectedReportItem_SUBRelatedMatters = new ReportItem()
            {
                Name = "SUB-Related Matters",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Matters",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\SUB-Related Matters.rdl")),
            };

            expectedReportItem_Listing = new ReportItem()
            {
                Name = "Listing",
                Path = "/SSRSMigrate_Tests/Reports/Listing",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2010\\Listing.rdl")),
                SubReports = new List<ReportItem>()
                {
                    expectedReportItem_SUBAddress,
                    expectedReportItem_SUBCategories,
                    expectedReportItem_SUBPhoneNumbers,
                    expectedReportItem_SUBRelatedContacts,
                    expectedReportItem_SUBRelatedMatters
                }
            };

            // Setup GetReports - Expected ReportItems
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
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            kernel = new StandardKernel(new DependencyModule());

            SetupExpectedResults();

            reader = new ReportServerReader(kernel.Get<IReportServerRepositoryFactory>().GetRepository("2010-SRC"));
        }

        [TestFixtureTearDown]
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
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_Tests/Reports/Inquiry");

            Assert.NotNull(actualReportItem);
            Assert.AreEqual(expectedReportItem_Inquiry.Name, actualReportItem.Name);
            Assert.AreEqual(expectedReportItem_Inquiry.Path, actualReportItem.Path);
            Assert.AreEqual(expectedReportItem_Inquiry.SubReports.Count(), actualReportItem.SubReports.Count());
            Assert.AreEqual(expectedReportItem_Inquiry.Definition, actualReportItem.Definition, "Report Definition");
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
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_Tests/Reports/Report Doesnt Exist");

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
            string path = "/SSRSMigrate_Tests";

            List<ReportItem> actual = reader.GetReports(path);

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
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException),
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found",
            MatchType = MessageMatch.Contains)]

        public void GetReports_PathDoesntExist()
        {
            string path = "/SSRSMigrate_Tests Doesnt Exist";

            List<ReportItem> actual = reader.GetReports(path);

            Assert.IsNull(actual);
        }
        #endregion

        #region GetReports Using Action<ReportItem> Delegate Tests
        [Test]
        public void GetReports_UsingDelegate()
        {
            string path = "/SSRSMigrate_Tests";

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
                    reader.GetReports("SSRSMigrate_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));

        }

        [Test]
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException),
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found",
            MatchType = MessageMatch.Contains)]
        public void GetReports_UsingDelegate_PathDoesntExist()
        {
            string path = "/SSRSMigrate_Tests Doesnt Exist";

            reader.GetReports(path, GetReports_Reporter);

            Assert.IsFalse(actualReportItems.Any());
        }

        public void GetReports_Reporter(ReportItem report)
        {
            if (report != null)
                actualReportItems.Add(report);
        }
        #endregion
    }
}
