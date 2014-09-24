using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using SSRSMigrate.TestHelper;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Reader.ReportServer2005
{
    [TestFixture]
    [CoverageExcludeAttribute]
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
                Name = "Company Sales",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = "Adventure Works sales by quarter and product category. This report illustrates the use of a tablix data region with nested row groups and column groups. You can drilldown from summary data into detail data by showing and hiding rows. This report also illustrates the use of a logo image and a background image.",
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Company Sales.rdl"))
            };

            // Setup GetReports - Expected ReportItems
            expectedReportItems = new List<ReportItem>()
            {
                expectedReportItem,
                new ReportItem()
                {
                    Name = "Sales Order Detail",
                    Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
                    CreatedBy = "DOMAIN\\user",
                    CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                    Description = "Detail of an individual Adventure Works order. This report can be accessed as a drillthrough report from the Employee Sales Summary and Territory Sales drilldown report. This report illustrates the use of a free form layout, a table, parameters, a subreport that shows multiple store contacts, and expressions.",
                    ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                    ModifiedBy = "DOMAIN\\user",
                    ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                    Size = 10,
                    VirtualPath = null,
                    Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Sales Order Detail.rdl")),
                    SubReports = new List<ReportItem>()
                    {
                        new ReportItem()
                        {
                            Name = "Store Contacts",
                            Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = "AdventureWorks store contacts. This report is a subreport used in Sales Order Detail to show all contacts for a store. Borderstyle is None so lines do not display in main report.",
                            ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Store Contacts.rdl")),
                        }
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

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_AW_Tests/Reports/Company Sales"))
                .Returns(() => expectedReportItem.Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_AW_Tests/Reports/Sales Order Detail"))
                .Returns(() => expectedReportItems[1].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_AW_Tests/Reports/Store Contacts"))
                .Returns(() => expectedReportItems[1].SubReports[0].Definition);

            // Setup IReportServerRepository.GetReport Mocks
            reportServerRepositoryMock.Setup(r => r.GetReport(null))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.GetReport(""))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_AW_Tests/Reports/Company Sales"))
                .Returns(() => expectedReportItem);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_AW_Tests/Reports/Sales Order Detail"))
                .Returns(() => expectedReportItems[1]);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_AW_Tests/Reports/Report Doesnt Exist"))
                .Returns(() => null);

            // Setup IReportServerRepository.GetReports Mocks
            reportServerRepositoryMock.Setup(r => r.GetReports(null))
               .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReports(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReports("/SSRSMigrate_AW_Tests"))
               .Returns(() => expectedReportItems);

            reportServerRepositoryMock.Setup(r => r.GetReports("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<ReportItem>());

            // Setup IReportServerRepository.GetReportsList Mocks
            reportServerRepositoryMock.Setup(r => r.GetReportsList(null))
               .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReportsList(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetReportsList("/SSRSMigrate_AW_Tests"))
               .Returns(() => expectedReportItems);

            reportServerRepositoryMock.Setup(r => r.GetReportsList("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<ReportItem>());

            // Setup IReportServerRepository.ValidatePath Mocks
            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests"))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests Doesnt Exist"))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests/Reports/Report Doesnt Exist"))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(expectedReportItem.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests/Reports/Sales Order Detail"))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests/Reports/Store Contacts"))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(null))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(""))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s ?? "", "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            MockLogger logger = new MockLogger();

            reader = new ReportServerReader(reportServerRepositoryMock.Object, logger);
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
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_AW_Tests/Reports/Company Sales");

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
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_AW_Tests/Reports/Report Doesnt Exist");

            Assert.Null(actualReportItem);
        }

        [Test]
        public void GetReport_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW_Tests/Reports/Invalid.Report";

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetReport(invalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }

        [Test]
        public void GetReport_WithSubReports()
        {
            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_AW_Tests/Reports/Sales Order Detail");

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
            List<ReportItem> actual = reader.GetReports("/SSRSMigrate_AW_Tests");

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
            List<ReportItem> actual = reader.GetReports("/SSRSMigrate_AW_Tests Doesnt Exist");

            Assert.NotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void GetReports_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetReports(invalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }
        #endregion

        #region GetReportsList Tests
        [Test]
        public void GetReports_UsingDelegate()
        {
            reader.GetReports("/SSRSMigrate_AW_Tests", GetReports_Reporter);

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
            reader.GetReports("/SSRSMigrate_AW_Tests Doesnt Exist", GetReports_Reporter);

            Assert.AreEqual(0, actualReportItems.Count());
        }

        [Test]
        public void GetReports_UsingDelegate_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetReports(invalidPath, GetReports_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }

        private void GetReports_Reporter(ReportItem reportItem)
        {
            actualReportItems.Add(reportItem);
        }
        #endregion

        //#region GetSubReports Tests
        //[Test]
        //public void GetSubReports()
        //{

        //}

        //[Test]
        //public void GetSubReports_NoSubReports()
        //{

        //}

        //[Test]
        //public void GetSubReports_NullDefinition()
        //{

        //}

        //[Test]
        //public void GetSubReports_EmptyDefinition()
        //{

        //}
        //#endregion
    }
}
