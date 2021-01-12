using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using Moq;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Item.Proxy;
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Reader.ReportServer2010
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_ReportTests
    {
        private ReportServerReader reader = null;
        private Mock<IReportServerPathValidator> pathValidatorMock = null;
        private Mock<IReportServerRepository> reportServerRepositoryMock = null;

        #region GetReports - Expected ReportItem
        private ReportItem expectedReportItem = null;
        #endregion

        #region GetReports - Expected ReportItems
        private List<ReportItem> expectedReportItems = null;
        private string[] expectedReportDefinitions = null;
        #endregion

        #region Get Reports - Actual ReportItems
        private List<ReportItem> actualReportItems = null;
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            // Setup IReportServerRepository mock
            reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerPathValidator mock
            pathValidatorMock = new Mock<IReportServerPathValidator>();

            // Setup GetReport - Expected ReportItem
            expectedReportItem = new ReportItemProxy(reportServerRepositoryMock.Object)
            {
                Name = "Company Sales",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            // Setup GetReports - Expected ReportItems
            expectedReportItems = new List<ReportItem>()
            {
                expectedReportItem,
                new ReportItemProxy(reportServerRepositoryMock.Object)
                {
                    Name = "Sales Order Detail",
                    Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
                    CreatedBy = "DOMAIN\\user",
                    CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                    Description = null,
                    ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                    ModifiedBy = "DOMAIN\\user",
                    ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                    Size = 10,
                    VirtualPath = null,
                    Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2005\\Sales Order Detail.rdl"))),
                    SubReports = new List<ReportItem>()
                    {
                        new ReportItem()
                        {
                            Name = "Store Contacts",
                            Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
                            CreatedBy = "DOMAIN\\user",
                            CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Description = null,
                            ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                            ModifiedBy = "DOMAIN\\user",
                            ModifiedDate =  DateTime.Parse("7/28/2014 12:06:43 PM"),
                            Size = 10,
                            VirtualPath = null,
                            Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2005\\Store Contacts.rdl"))),
                        }
                    }
                }
            };
            
            MockLogger logger = new MockLogger();

            reader = new ReportServerReader(reportServerRepositoryMock.Object, logger, pathValidatorMock.Object);
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
            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_AW_Tests/Reports/Company Sales"))
                .Returns(() => expectedReportItem.Definition);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_AW_Tests/Reports/Company Sales"))
                .Returns(() => expectedReportItem);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Reports/"))
              .Returns(() => true);

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
            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Reports/"))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_AW_Tests/Reports/Report Doesnt Exist"))
                .Returns(() => null);

            ReportItem actualReportItem = reader.GetReport("/SSRSMigrate_AW_Tests/Reports/Report Doesnt Exist");

            Assert.Null(actualReportItem);
        }

        [Test]
        public void GetReport_InvalidPath_InvalidCharInPath()
        {
            string invalidPath = "/SSRSMigrate_AW_Tests/Reports.Invalid/Invalid Report";

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Reports.Invalid/"))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetReport(invalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }

        [Test]
        public void GetReport_InvalidPath_NoSlashInPath()
        {
            string invalidPath = "My Path";

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
            reportServerRepositoryMock.Setup(r => r.GetReportDefinition("/SSRSMigrate_AW_Tests/Reports/Sales Order Detail"))
                .Returns(() => expectedReportItems[1].Definition);

            reportServerRepositoryMock.Setup(r => r.GetReport("/SSRSMigrate_AW_Tests/Reports/Sales Order Detail"))
                .Returns(() => expectedReportItems[1]);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Reports/"))
              .Returns(() => true);

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
            reportServerRepositoryMock.Setup(r => r.GetReports("/SSRSMigrate_AW_Tests"))
               .Returns(() => expectedReportItems);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
               .Returns(() => true);

            var actual = reader.GetReports("/SSRSMigrate_AW_Tests");

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
            reportServerRepositoryMock.Setup(r => r.GetReports("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<ReportItem>());

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests Doesnt Exist"))
              .Returns(() => true);

            var actual = reader.GetReports("/SSRSMigrate_AW_Tests Doesnt Exist");

            Assert.NotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void GetReports_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            pathValidatorMock.Setup(r => r.Validate(It.Is<string>(s => Regex.IsMatch(s ?? "", "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetReports(invalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }
        #endregion

        #region GetReports Using Delegate Reporter Tests
        [Test]
        public void GetReports_UsingDelegate()
        {
            reportServerRepositoryMock.Setup(r => r.GetReportsLazy("/SSRSMigrate_AW_Tests"))
               .Returns(() => expectedReportItems);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
              .Returns(() => true);

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
            reportServerRepositoryMock.Setup(r => r.GetReportsLazy("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<ReportItemProxy>());

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests Doesnt Exist"))
              .Returns(() => true);

            reader.GetReports("/SSRSMigrate_AW_Tests Doesnt Exist", GetReports_Reporter);

            Assert.AreEqual(0, actualReportItems.Count());
        }

        [Test]
        public void GetReports_UsingDelegate_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            pathValidatorMock.Setup(r => r.Validate(It.Is<string>(s => Regex.IsMatch(s ?? "", "[:?;@&=+$,\\*><|.\"]+") == true)))
                .Returns(() => false);

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
    }
}
