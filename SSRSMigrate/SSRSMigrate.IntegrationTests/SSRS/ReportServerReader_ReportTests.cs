using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.IntegrationTests.SSRS
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_ReportTests
    {
        ReportServerReader reader = null;

        #region GetReport - Expected ReportItem
        ReportItem expectedReportItem;
        #endregion

        #region GetReports - Expected ReportItems
        List<ReportItem> expectedReportItems = null;
        #endregion

        #region GetReports - Actual ReportItems
        List<ReportItem> actualReportItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Setup expected ReportItems
            expectedReportItem = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\Inquiry.rdl"))
            };

            // Setup GetReports - Expected ReportItems
            expectedReportItems = new List<ReportItem>()
            {
                expectedReportItem,
                new ReportItem()
                {
                    Name = "Listing",
                    Path = "/SSRSMigrate_Tests/Reports/Listing",
                    Description = null,
                    ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                    VirtualPath = null,
                    Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\Listing.rdl")),
                    SubReports = new List<ReportItem>()
                    {
                        new ReportItem()
                        {
                            Name = "SUB-Address",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Address",
                            Description = null,
                            ID = "77b2135b-c52f-4a52-9406-7bd523ad9623",
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Addresses.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Categories",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Categories",
                            Description = null,
                            ID = "ab67975e-8535-4cca-88d8-79a1827a099e",
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Categories.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Phone Numbers",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Phone Numbers",
                            Description = null,
                            ID = "7b64b5e4-4ca2-466c-94ce-19d32d8222f5",
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Phone Numbers.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Related Contacts",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Related Contacts",
                            Description = null,
                            ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                           VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Related Contacts.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Related Matters",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Related Matters",
                            Description = null,
                            ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Related Matters.rdl")),
                        },
                    }
                }
            };

            reader = DependencySingleton.Instance.Get<ReportServerReader>();
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
            Assert.AreEqual(expectedReportItem.Name, actualReportItem.Name);
            Assert.AreEqual(expectedReportItem.Path, actualReportItem.Path);
            Assert.AreEqual(expectedReportItem.SubReports.Count(), actualReportItem.SubReports.Count());
            Assert.AreEqual(expectedReportItem.Definition, actualReportItem.Definition, "Report Definition");
            //Assert.True(expectedReportItem.Definition.SequenceEqual(actualReportItem.Definition), "Report Definition");
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
        #endregion

        #region GetReportsList Tests
        #endregion
    }
}
