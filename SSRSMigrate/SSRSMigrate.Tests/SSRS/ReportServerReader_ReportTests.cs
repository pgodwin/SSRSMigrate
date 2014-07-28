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
        }
    }
}
