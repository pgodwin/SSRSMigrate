using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;
using Moq;
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.Tests.SSRS
{
    [TestFixture]
    class ReportServerReader_ReportTests
    {
        ReportServerReader reader = null;

        #region GetReport - Expected ReportItem
        ReportItem expectedReportItem = null;
        string expectedReportItemDefinition = null;

        #endregion

        #region GetReports - Expected ReportItems
        List<ReportItem> expectedReportItems = null;
        string[] expectedReportItemsDefinitions = null;
        #endregion

        #region GetReports - Actual ReportItems
        List<ReportItem> actualReportItems = null; 
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {

            // Setup GetReport - Expected ReportItem
            expectedReportItem = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
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
                    CreatedBy = "DOMAIN\\user",
                    CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                    Description = null,
                    ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                    ModifiedBy = "DOMAIN\\user",
                    ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                    Size = 10,
                    VirtualPath = null,
                    Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\Listing.rdl")),
                    SubReports = new List<ReportItem>()
                    {
                        new ReportItem()
                        {
                            Name = "SUB-Address",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Address",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = null,
                            ID = "77b2135b-c52f-4a52-9406-7bd523ad9623",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Addresses.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Categories",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Categories",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = null,
                            ID = "ab67975e-8535-4cca-88d8-79a1827a099e",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Categories.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Phone Numbers",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Phone Numbers",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = null,
                            ID = "7b64b5e4-4ca2-466c-94ce-19d32d8222f5",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Phone Numbers.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Related Contacts",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Related Contacts",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = null,
                            ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Related Contacts.rdl")),
                        },
                        new ReportItem()
                        {
                            Name = "SUB-Related Matters",
                            Path = "/SSRSMigrate_Tests/Reports/SUB-Related Matters",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = null,
                            ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TestUtils.StringToByteArray(TestUtils.LoadRDLFile("Test Reports\\2008\\SUB-Related Matters.rdl")),
                        },
                    }
                }
            };

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerRepository.GetReportDefinition Mocks
            reportServerRepositoryMock.Setup(r => r.GetReportDefinition(null))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition(""))
               .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/Inquiry"))
                .Returns(() => expectedReportItem.Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/Listing"))
                .Returns(() => expectedReportItems[1].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/SUB-Address"))
                .Returns(() => expectedReportItems[1].SubReports[0].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/SUB-Categories"))
                .Returns(() => expectedReportItems[1].SubReports[1].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/SUB-Phone Number"))
                .Returns(() => expectedReportItems[1].SubReports[2].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/SUB-Related Contacts"))
                .Returns(() => expectedReportItems[1].SubReports[3].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/SUB-Related Matters"))
                .Returns(() => expectedReportItems[1].SubReports[4].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_Tests/Reports/SUB-Related Matters"))
                .Returns(() => expectedReportItems[1].SubReports[4].Definition);

            // Setup IReportServerRepository.GetReport Mocks
            reportServerRepositoryMock.Setup(r => r.GetReport(null))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.GetReport(""))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_Tests/Reports/Inquiry"))
                .Returns(() => expectedReportItem);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_Tests/Reports/Listing"))
                .Returns(() => expectedReportItems[1]);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_Tests/Reports/Report Doesnt Exist"))
                .Returns(() => null);

            // Setup IReportServerRepository.GetReports Mocks
            reportServerRepositoryMock.Setup(r => r.GetReports(null))
               .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReports(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReports("/SSRSMigrate_Tests"))
               .Returns(() => expectedReportItems);

            reportServerRepositoryMock.Setup(r => r.GetReports("/SSRSMigrate_Tests Doesnt Exist"))
                .Returns(() => new List<ReportItem>());

            // Setup IReportServerRepository.GetReportsList Mocks
            reportServerRepositoryMock.Setup(r => r.GetReportsList(null))
               .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReportsList(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReportsList("/SSRSMigrate_Tests"))
               .Returns(() => expectedReportItems);

            reportServerRepositoryMock.Setup(r => r.GetReportsList("/SSRSMigrate_Tests Doesnt Exist"))
                .Returns(() => new List<ReportItem>());

            reader = new ReportServerReader(reportServerRepositoryMock.Object);
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
            Assert.AreEqual(expectedReportItem.ID, actualReportItem.ID);
            Assert.AreEqual(expectedReportItem.SubReports.Count(), actualReportItem.SubReports.Count());
            Assert.True(expectedReportItem.Definition.SequenceEqual(actualReportItem.Definition));
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
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_Tests/Reports/Listing");

            Assert.NotNull(actualReportItem);
            Assert.AreEqual(expectedReportItems[1].Name, actualReportItem.Name);
            Assert.AreEqual(expectedReportItems[1].Path, actualReportItem.Path);
            Assert.AreEqual(expectedReportItems[1].ID, actualReportItem.ID);
            Assert.True(expectedReportItems[1].Definition.SequenceEqual(actualReportItem.Definition));
            Assert.AreEqual(expectedReportItems[1].SubReports.Count(), actualReportItem.SubReports.Count());
        }
        #endregion

        #region GetReports Tests
        [Test]
        public void GetReports()
        {
            List<ReportItem> actual = reader.GetReports("/SSRSMigrate_Tests");

            Assert.NotNull(actual);
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
            List<ReportItem> actual = reader.GetReports("/SSRSMigrate_Tests Doesnt Exist");

            Assert.NotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }
        #endregion

        #region GetReportsList Tests
        [Test]
        public void GetReports_UsingDelegate()
        {
            reader.GetReports("/SSRSMigrate_Tests", GetReports_Reporter);

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
                    reader.GetReports("/SSRSMigrate_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));
        }

        [Test]
        public void GetReports_UsingDelegate_PathDoesntExist()
        {
            reader.GetReports("/SSRSMigrate_Tests Doesnt Exist", GetReports_Reporter);

            Assert.AreEqual(0, actualReportItems.Count());
        }

        private void GetReports_Reporter(ReportItem reportItem)
        {
            actualReportItems.Add(reportItem);
        }
        #endregion
    }
}
