using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.TestHelper;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Repository;
using Moq;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper.Logging;
using SSRSMigrate.Utility;

namespace SSRSMigrate.Tests.SSRS.Writer.ReportServer2010
{
    [TestFixture]
    class ReportServerWriter_ReportTests
    {
        private ReportServerWriter writer = null;
        private Mock<IReportServerPathValidator> pathValidatorMock = null;
        private Mock<IReportServerRepository> reportServerRepositoryMock = null;

        #region Report Items
        List<ReportItem> reportItems = null;
        ReportItem reportItem_CompanySales;
        ReportItem reportItem_SalesOrderDetail;
        ReportItem reportItem_StoreContacts;
        ReportItem reportItem_InvalidPath;
        ReportItem reportItem_AlreadyExists;
        ReportItem reportItem_NullDefinition;
        ReportItem reportItem_Error;
        #endregion

        private void SetupReportItems()
        {
            reportItem_CompanySales = new ReportItem()
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

            reportItem_StoreContacts = new ReportItem()
            {
                Name = "Store Contacts",
                Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Store Contacts.rdl"))),
            };

            reportItem_SalesOrderDetail = new ReportItem()
            {
                Name = "Sales Order Detail",
                Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Sales Order Detail.rdl"))),
                SubReports = new List<ReportItem>()
                {
                    reportItem_StoreContacts
                }
            };

            reportItem_AlreadyExists = new ReportItem()
            {
                Name = "Already Exists",
                Path = "/SSRSMigrate_AW_Tests/Reports/Already Exists",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            reportItem_InvalidPath = new ReportItem()
            {
                Name = "Invalid.Path",
                Path = "/SSRSMigrate_AW_Tests/Reports.Invalid/Invalid.Path",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            reportItem_NullDefinition = new ReportItem()
            {
                Name = "Null Definition",
                Path = "/SSRSMigrate_AW_Tests/Reports/Null Definition",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = null
            };

            reportItem_Error = new ReportItem()
            {
                Name = "ERROR",
                Path = "/SSRSMigrate_AW_Tests/Reports/ERROR",
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

            reportItems = new List<ReportItem>()
            {
                reportItem_CompanySales,
                reportItem_SalesOrderDetail,
                reportItem_StoreContacts
            };
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            SetupReportItems();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
            // Setup IReportServerRepository mock
            reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerPathValidator mock
            pathValidatorMock = new Mock<IReportServerPathValidator>();

            MockLogger logger = new MockLogger();

            writer = new ReportServerWriter(reportServerRepositoryMock.Object, logger, pathValidatorMock.Object);

            writer.Overwrite = false; // Reset overwrite property
        }

        #region WriteReport Tests
        [Test]
        public void WriteReport()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_CompanySales), reportItem_CompanySales, It.IsAny<bool>()))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportItem_CompanySales.ParentPath))
                .Returns(() => true);

            string[] actual = writer.WriteReport(reportItem_CompanySales);

            Assert.Null(actual);
        }

        [Test]
        public void WriteReport_AlreadyExists_OverwriteDisallowed()
        {
            reportServerRepositoryMock.Setup(r => r.ItemExists(reportItem_AlreadyExists.Path, "Report"))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_AlreadyExists.ParentPath))
                .Returns(() => true);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteReport(reportItem_AlreadyExists);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", reportItem_AlreadyExists.Path)));
        }

        [Test]
        public void WriteReport_AlreadyExists_OverwriteAllowed()
        {
            reportServerRepositoryMock.Setup(r => r.ItemExists(reportItem_AlreadyExists.Path, "Report"))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_AlreadyExists.ParentPath))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_AlreadyExists), reportItem_AlreadyExists, true))
                .Returns(() => null);

            writer.Overwrite = true; // Allow for overwriting of report

            string[] actual = writer.WriteReport(reportItem_AlreadyExists);
            Assert.Null(actual);
        }

        [Test]
        public void WriteReport_NullReportItem()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(null, It.IsAny<ReportItem>(), It.IsAny<bool>()))
                .Throws(new ArgumentException("reportPath"));

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
            pathValidatorMock.Setup(r => r.Validate(reportItem_InvalidPath.ParentPath))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReport(reportItem_InvalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", reportItem_InvalidPath.Path)));
        }

        [Test]
        public void WriteReport_ReportItemNullName()
        {
            ReportItem report = new ReportItem()
            {
                Name = null,
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
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
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
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
                Name = "Company Sales",
                Path = null,
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Path"));
        }

        [Test]
        public void WriteReport_ReportItemEmptyPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Company Sales",
                Path = "",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReport(report);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Path"));
        }

        [Test]
        public void WriteReport_ReportItemNullDefinition()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_NullDefinition), reportItem_NullDefinition, It.IsAny<bool>()))
                .Throws(new InvalidReportDefinitionException(reportItem_NullDefinition.Path));

            pathValidatorMock.Setup(r => r.Validate(reportItem_NullDefinition.ParentPath))
                .Returns(() => true);

            InvalidReportDefinitionException ex = Assert.Throws<InvalidReportDefinitionException>(
                delegate
                {
                    writer.WriteReport(reportItem_NullDefinition);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid report definition for report '{0}'.", reportItem_NullDefinition.Path)));
        }

        [Test]
        public void WriteReport_ReportItemError()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_Error), reportItem_Error, It.IsAny<bool>()))
                .Returns(() => new string[] { string.Format("Error writing report '{0}': Error!", reportItem_Error.Path) });

            pathValidatorMock.Setup(r => r.Validate(reportItem_Error.ParentPath))
                .Returns(() => true);

            string[] actual = writer.WriteReport(reportItem_Error);

            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Length);

            Assert.AreEqual(string.Format("Error writing report '{0}': Error!", reportItem_Error.Path), actual[0]);
        }
        #endregion

        #region WriteReports Tests
        [Test]
        public void WriteReports()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_CompanySales), reportItem_CompanySales, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SalesOrderDetail), reportItem_SalesOrderDetail, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_StoreContacts), reportItem_StoreContacts, It.IsAny<bool>()))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportItem_CompanySales.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_SalesOrderDetail.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_StoreContacts.ParentPath))
                .Returns(() => true);

            string[] actual = writer.WriteReports(reportItems.ToArray());

            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void WriteReports_OneOrMoreAlreadyExists_OverwriteDisallowed()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_CompanySales), reportItem_CompanySales, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SalesOrderDetail), reportItem_SalesOrderDetail, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_StoreContacts), reportItem_StoreContacts, It.IsAny<bool>()))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportItem_CompanySales.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_SalesOrderDetail.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_StoreContacts.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_AlreadyExists.ParentPath))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(reportItem_AlreadyExists.Path, "Report"))
                .Returns(() => true);

            List<ReportItem> items = new List<ReportItem>()
            {
                reportItem_AlreadyExists
            };

            items.AddRange(reportItems);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", reportItem_AlreadyExists.Path)));
        }

        [Test]
        public void WriteReports_OneOrMoreAlreadyExists_OverwriteAllowed()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_CompanySales), reportItem_CompanySales, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_SalesOrderDetail), reportItem_SalesOrderDetail, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_StoreContacts), reportItem_StoreContacts, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_AlreadyExists), reportItem_AlreadyExists, true))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportItem_CompanySales.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_SalesOrderDetail.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_StoreContacts.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_AlreadyExists.ParentPath))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(reportItem_AlreadyExists.Path, "Report"))
                .Returns(() => true);

            List<ReportItem> items = new List<ReportItem>()
            {
                reportItem_AlreadyExists
            };

            items.AddRange(reportItems);

            writer.Overwrite = true; // Allow for overwriting of report

            string[] actual = writer.WriteReports(items.ToArray());

            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void WriteReports_NullReportItems()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteReport(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: reportItem"));
        }

        [Test]
        public void WriteReports_OneOrMoreInvalidReportPath()
        {
            pathValidatorMock.Setup(r => r.Validate(reportItem_CompanySales.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_SalesOrderDetail.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_StoreContacts.ParentPath))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportItem_InvalidPath.ParentPath))
                .Returns(() => false);

            List<ReportItem> items = new List<ReportItem>()
            {
                reportItem_InvalidPath
            };

            items.AddRange(reportItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", reportItem_InvalidPath.Path)));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemNullName()
        {
            ReportItem report = new ReportItem()
            {
                Name = null,
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            List<ReportItem> items = new List<ReportItem>()
            {
                report
            };

            items.AddRange(reportItems);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemEmptyName()
        {
            ReportItem report = new ReportItem()
            {
                Name = "",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            List<ReportItem> items = new List<ReportItem>()
            {
                report
            };

            items.AddRange(reportItems);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemNullPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Company Sales",
                Path = null,
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            List<ReportItem> items = new List<ReportItem>()
            {
                report
            };

            items.AddRange(reportItems);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });
            
            Assert.That(ex.Message, Is.EqualTo("item.Path"));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemEmptyPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Company Sales",
                Path = "",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2010\\Company Sales.rdl")))
            };

            List<ReportItem> items = new List<ReportItem>()
            {
                report
            };

            items.AddRange(reportItems);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });
            
            Assert.That(ex.Message, Is.EqualTo("item.Path"));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemNullDefinition()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_NullDefinition), reportItem_NullDefinition, It.IsAny<bool>()))
                .Throws(new InvalidReportDefinitionException(reportItem_NullDefinition.Path));

            pathValidatorMock.Setup(r => r.Validate(reportItem_NullDefinition.ParentPath))
                .Returns(() => true);

            List<ReportItem> items = new List<ReportItem>()
            {
                reportItem_NullDefinition
            };

            items.AddRange(reportItems);

            InvalidReportDefinitionException ex = Assert.Throws<InvalidReportDefinitionException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid report definition for report '{0}'.", reportItem_NullDefinition.Path)));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemError()
        {
            reportServerRepositoryMock.Setup(r => r.WriteReport(TesterUtility.GetParentPath(reportItem_Error), reportItem_Error, It.IsAny<bool>()))
                .Returns(() => new string[] { string.Format("Error writing report '{0}': Error!", reportItem_Error.Path) });

            pathValidatorMock.Setup(r => r.Validate(reportItem_Error.ParentPath))
                .Returns(() => true);

            List<ReportItem> items = new List<ReportItem>()
            {
                reportItem_Error
            };

            items.Add(reportItem_CompanySales);

            string[] actual = writer.WriteReports(items.ToArray());

            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Length);

            Assert.AreEqual(string.Format("Error writing report '{0}': Error!", reportItem_Error.Path), actual[0]);
        }
        #endregion
    }
}
