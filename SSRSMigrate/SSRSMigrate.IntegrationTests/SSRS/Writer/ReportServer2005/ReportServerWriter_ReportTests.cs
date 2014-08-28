using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.TestHelper;
using System.Net;
using Ninject;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2005
{
    /// <summary>
    /// These integration tests will write ReportItems to a ReportingService2005 endpoint.
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
                Path = "{0}/Reports/Company Sales",
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Description = null,
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/28/2014 12:06:43 PM"),
                Size = 10,
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Company Sales.rdl"))
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Store Contacts.rdl")),
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Sales Order Detail.rdl")),
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
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Company Sales.rdl"))
            };

            reportItem_InvalidPath = new ReportItem()
            {
                Name = "Invalid.Path",
                Path = "/SSRSMigrate_AW_Tests/Reports/Invalid.Path",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Company Sales.rdl"))
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
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region Environment Setup/Teardown
        private void SetupEnvironment()
        {
            ReportingService2005TestEnvironment.SetupEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath);
        }

        private void TeardownEnvironment()
        {
            ReportingService2005TestEnvironment.TeardownEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath);
        }
        #endregion
    }
}
