using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using Moq;
using SSRSMigrate.SSRS.Repository;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.Tests.SSRS.Writer.ReportServer2005
{
    [TestFixture]
    class ReportServerWriter_ReportTests
    {
        ReportServerWriter writer = null;

        #region Report Items
        List<ReportItem> reportItems = null;
        ReportItem reportItem_Inquiry;
        ReportItem reportItem_Listing;
        ReportItem reportItem_SUBAddress;
        ReportItem reportItem_SUBCategories;
        ReportItem reportItem_SUBPhoneNumbers;
        ReportItem reportItem_SUBRelatedContacts;
        ReportItem reportItem_SUBRelatedMatters;
        ReportItem reportItem_InvalidPath;
        ReportItem reportItem_AlreadyExists;
        ReportItem reportItem_NullDefinition;
        #endregion

        private void SetupReportItems()
        {
            reportItem_Inquiry = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            reportItem_SUBAddress = new ReportItem()
            {
                Name = "SUB-Address",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Address",
                Description = null,
                ID = "77b2135b-c52f-4a52-9406-7bd523ad9623",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\SUB-Addresses.rdl")),
            };

            reportItem_SUBCategories = new ReportItem()
            {
                Name = "SUB-Categories",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Categories",
                Description = null,
                ID = "ab67975e-8535-4cca-88d8-79a1827a099e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\SUB-Categories.rdl")),
            };

            reportItem_SUBPhoneNumbers = new ReportItem()
            {
                Name = "SUB-Phone Numbers",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Phone Numbers",
                Description = null,
                ID = "7b64b5e4-4ca2-466c-94ce-19d32d8222f5",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\SUB-Phone Numbers.rdl")),
            };

            reportItem_SUBRelatedContacts = new ReportItem()
            {
                Name = "SUB-Related Contacts",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Contacts",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\SUB-Related Contacts.rdl")),
            };

            reportItem_SUBRelatedMatters = new ReportItem()
            {
                Name = "SUB-Related Matters",
                Path = "/SSRSMigrate_Tests/Reports/SUB-Related Matters",
                Description = null,
                ID = "a22cf477-4db7-4f0f-bc6e-69e0a8a8bd70",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\SUB-Related Matters.rdl")),
            };

            reportItem_Listing = new ReportItem()
            {
                Name = "Listing",
                Path = "/SSRSMigrate_Tests/Reports/Listing",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Listing.rdl")),
                SubReports = new List<ReportItem>()
                {
                    reportItem_SUBAddress,
                    reportItem_SUBCategories,
                    reportItem_SUBPhoneNumbers,
                    reportItem_SUBRelatedContacts,
                    reportItem_SUBRelatedMatters
                }
            };

            reportItem_AlreadyExists = new ReportItem()
            {
                Name = "Already Exists",
                Path = "/SSRSMigrate_Tests/Reports/Already Exists",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            reportItem_InvalidPath = new ReportItem()
            {
                Name = "Invalid.Path",
                Path = "/SSRSMigrate_Tests/Reports/Invalid.Path",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            reportItem_NullDefinition = new ReportItem()
            {
                Name = "Null Definition",
                Path = "/SSRSMigrate_Tests/Reports/Null Definition",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = null
            };

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
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            SetupReportItems();

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerRepository.WriteReport Mocks
            reportServerRepositoryMock.Setup(r => r.WriteReport(null, It.IsAny<ReportItem>()))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.WriteReport("", It.IsAny<ReportItem>()))
                .Throws(new ArgumentException("reportPath"));

            reportServerRepositoryMock.Setup(r => r.WriteReport(It.IsAny<string>(), null))
                .Throws(new ArgumentException("reportItem"));

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_Inquiry), reportItem_Inquiry))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_Listing), reportItem_Listing))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SUBAddress), reportItem_SUBAddress))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SUBCategories), reportItem_SUBCategories))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SUBPhoneNumbers), reportItem_SUBPhoneNumbers))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SUBPhoneNumbers), reportItem_SUBPhoneNumbers))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SUBRelatedContacts), reportItem_SUBRelatedContacts))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SUBRelatedMatters), reportItem_SUBRelatedMatters))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_AlreadyExists), reportItem_AlreadyExists))
                .Throws(new ReportAlreadyExistsException(string.Format("The report '{0}' already exists.", reportItem_AlreadyExists.Path)));

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_NullDefinition), reportItem_NullDefinition))
                .Throws(new InvalidReportDefinition(string.Format("Invalid report definition for report '{0}'.", reportItem_NullDefinition)));

            // Setup IReportServerRepository.ValidatePath Mocks
            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_Inquiry.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_Listing.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_SUBAddress.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_SUBCategories.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_SUBPhoneNumbers.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_SUBPhoneNumbers.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_SUBRelatedMatters.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_SUBRelatedContacts.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_AlreadyExists.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(reportItem_NullDefinition.Path))
              .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(null))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(""))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s ?? "", "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            writer = new ReportServerWriter(reportServerRepositoryMock.Object);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        #region WriteReport Tests
        [Test]
        public void WriteReport()
        {
            string[] actual = writer.WriteReport(reportItem_Inquiry);

            Assert.Null(actual);
        }

        [Test]
        public void WriteReport_AlreadyExists()
        {
            ReportAlreadyExistsException ex = Assert.Throws<ReportAlreadyExistsException>(
                delegate
                {
                    writer.WriteReport(reportItem_AlreadyExists);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The report '{0}' already exists.", reportItem_AlreadyExists.Path)));
        }

        [Test]
        public void WriteReport_NullReportItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteReport(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: reportItem"));
        }

        [Test]
        public void WriteReport_InvalidReportPath()
        {
            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(reportItem_InvalidPath);
                });

            Assert.That(ex.Message, Is.StringContaining(string.Format("Invalid path '{0}'.", reportItem_InvalidPath.Path)));
        }

        [Test]
        public void WriteReport_ReportItemNullName()
        {
            ReportItem report = new ReportItem()
            {
                Name = null,
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteReport_ReportItemEmptyName()
        {
            ReportItem report = new ReportItem()
            {
                Name = "",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteReport_ReportItemNullPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Inquiry",
                Path = null,
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path ''."));
        }

        [Test]
        public void WriteReport_ReportItemEmptyPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Inquiry",
                Path = "",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path ''."));
        }

        [Test]
        public void WriteReport_ReportItemNullDefinition()
        {
            InvalidReportDefinition ex = Assert.Throws<InvalidReportDefinition>(
                delegate
                {
                    writer.WriteReport(reportItem_NullDefinition);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid report definition for report '{0}'.", reportItem_NullDefinition)));
        }
        #endregion

        #region WriteReports Tests
        [Test]
        public void WriteReports()
        {
            string[] actual = writer.WriteReports(reportItems.ToArray());

            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void WriteReports_AlreadyExists()
        {
            ReportAlreadyExistsException ex = Assert.Throws<ReportAlreadyExistsException>(
                delegate
                {
                    writer.WriteReport(reportItem_AlreadyExists);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The report '{0}' already exists.", reportItem_AlreadyExists.Path)));
        }

        [Test]
        public void WriteReports_NullReportItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteReport(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: reportItem"));
        }

        [Test]
        public void WriteReports_InvalidReportPath()
        {
            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(reportItem_InvalidPath);
                });

            Assert.That(ex.Message, Is.StringContaining(string.Format("Invalid path '{0}'.", reportItem_InvalidPath.Path)));
        }

        [Test]
        public void WriteReports_ReportItemNullName()
        {
            ReportItem report = new ReportItem()
            {
                Name = null,
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteReports_ReportItemEmptyName()
        {
            ReportItem report = new ReportItem()
            {
                Name = "",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteReports_ReportItemNullPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Inquiry",
                Path = null,
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path ''."));
        }

        [Test]
        public void WriteReports_ReportItemEmptyPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Inquiry",
                Path = "",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test Reports\\2005\\Inquiry.rdl"))
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path ''."));
        }

        [Test]
        public void WriteReports_ReportItemNullDefinition()
        {
            InvalidReportDefinition ex = Assert.Throws<InvalidReportDefinition>(
                delegate
                {
                    writer.WriteReport(reportItem_NullDefinition);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid report definition for report '{0}'.", reportItem_NullDefinition)));
        }
        #endregion
    }
}
