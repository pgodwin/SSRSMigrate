using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ninject;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using System.Net;
using SSRSMigrate.Factory;
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2010
{
    /// <summary>
    /// These integration tests will write ReportItems to a ReportingService2010 endpoint.
    /// The ReportItem objects used are already 'converted' to contain the destination information.
    /// </summary>
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerWriter_ReportTests
    {
        StandardKernel kernel = null;
        ReportServerWriter writer = null;
        string outputPath = null;

        #region Report Items
        List<ReportItem> reportItems = null;
        ReportItem reportItem_CompanySales;
        ReportItem reportItem_SalesOrderDetail;
        ReportItem reportItem_StoreContacts;
        ReportItem reportItem_InvalidPath;
        ReportItem reportItem_AlreadyExists;
        ReportItem reportItem_NullDefinition;
        #endregion

        private void SetupReportItems()
        {
            reportItem_CompanySales = new ReportItem()
            {
                Name = "Company Sales",
                Path = string.Format("{0}/Reports/Company Sales", outputPath),
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
            };

            reportItem_StoreContacts = new ReportItem()
            {
                Name = "Store Contacts",
                Path = string.Format("{0}/Reports/Store Contacts", outputPath),
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "18fc782e-dd5f-4c65-95ff-957e1bdc98de",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Store Contacts.rdl")),
            };

            reportItem_SalesOrderDetail = new ReportItem()
            {
                Name = "Sales Order Detail",
                Path = string.Format("{0}/Reports/Sales Order Detail", outputPath),
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "70650568-7dd4-4ef4-aeaa-67502de11b4f",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Sales Order Detail.rdl")),
                SubReports = new List<ReportItem>()
                {
                    reportItem_StoreContacts
                }
            };

            reportItem_AlreadyExists = new ReportItem()
            {
                Name = "Already Exists",
                Path = string.Format("{0}/Reports/Already Exists", outputPath),
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
            };

            reportItem_InvalidPath = new ReportItem()
            {
                Name = "Invalid.Path",
                Path = string.Format("{0}/Reports/Invalid.Path", outputPath),
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
            };

            reportItem_NullDefinition = new ReportItem()
            {
                Name = "Null Definition",
                Path = string.Format("{0}/Reports/Null Definition", outputPath),
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = null
            };

            reportItems = new List<ReportItem>()
            {
                reportItem_CompanySales,
                reportItem_SalesOrderDetail,
                reportItem_StoreContacts
            };
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            outputPath = Properties.Settings.Default.DestinationPath;

            kernel = new StandardKernel(new DependencyModule());

            SetupReportItems();

            writer = kernel.Get<IReportServerWriterFactory>().GetWriter<ReportServerWriter>("2010-DEST");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
            SetupEnvironment();

            writer.Overwrite = false; // Reset overwrite property
        }

        [TearDown]
        public void TearDown()
        {
            TeardownEnvironment();
        }

        #region Environment Setup/Teardown
        private void SetupEnvironment()
        {
            ReportingService2010TestEnvironment.SetupReportWriterEnvironment(
                Properties.Settings.Default.ReportServer2008R2WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath,
                new List<ReportItem>()
                {
                    reportItem_AlreadyExists
                });
        }

        private void TeardownEnvironment()
        {
            ReportingService2010TestEnvironment.TeardownReportWriterEnvironment(
                Properties.Settings.Default.ReportServer2008R2WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath,
                new List<ReportItem>()
                {
                    reportItem_AlreadyExists
                });
        }
        #endregion

        #region WriteReport Tests
        [Test]
        public void WriteReport()
        {
            string[] actual = writer.WriteReport(reportItem_CompanySales);

            Assert.Null(actual);
        }

        [Test]
        public void WriteReport_AlreadyExists_OverwriteDisallowed()
        {
            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteReport(reportItem_AlreadyExists);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The report '{0}' already exists.", reportItem_AlreadyExists.Path)));
        }

        [Test]
        public void WriteReport_AlreadyExists_OverwriteAllowed()
        {
            writer.Overwrite = true; // Allow overwriting of report

            string[] actual = writer.WriteReport(reportItem_AlreadyExists);

            Assert.Null(actual);
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
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
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
                Name = "Company Sales",
                Path = "",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
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
            InvalidReportDefinitionException ex = Assert.Throws<InvalidReportDefinitionException>(
                delegate
                {
                    writer.WriteReport(reportItem_NullDefinition);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid report definition for report '{0}'.", reportItem_NullDefinition.Path)));
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
        public void WriteReports_OneOrMoreAlreadyExists_OverwriteDisallowed()
        {
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

            Assert.That(ex.Message, Is.EqualTo(string.Format("The report '{0}' already exists.", reportItem_AlreadyExists.Path)));
        }

        [Test]
        public void WriteReports_OneOrMoreAlreadyExists_OverwriteAllowedllowed()
        {
            List<ReportItem> items = new List<ReportItem>()
            {
                reportItem_AlreadyExists
            };

            items.AddRange(reportItems);

            writer.Overwrite = true; // Allow overwriting of report

            string[] actual = writer.WriteReports(items.ToArray());

            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void WriteReports_NullReportItems()
        {

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteReports(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: reportItems"));
        }

        [Test]
        public void WriteReports_OneOrMoreInvalidReportPath()
        {
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

            Assert.That(ex.Message, Is.StringContaining(string.Format("Invalid path '{0}'.", reportItem_InvalidPath.Path)));
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
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
        public void WriteReports_OneOrMoteReportItemNullPath()
        {
            ReportItem report = new ReportItem()
            {
                Name = "Company Sales",
                Path = null,
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
            };

            List<ReportItem> items = new List<ReportItem>()
            {
                report
            };

            items.AddRange(reportItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path ''."));
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2010\\Company Sales.rdl"))
            };

            List<ReportItem> items = new List<ReportItem>()
            {
                report
            };

            items.AddRange(reportItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteReports(items.ToArray());
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path ''."));
        }

        [Test]
        public void WriteReports_OneOrMoreReportItemNullDefinition()
        {
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
        #endregion
    }
}
