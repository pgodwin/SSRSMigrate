using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ninject;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Factory;
using SSRSMigrate.TestHelper;
using System.Net;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2005
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerWriter_DataSourceTests
    {
        StandardKernel kernel = null;
        ReportServerWriter writer = null;
        string outputPath = null;

        #region DataSourceItems
        List<DataSourceItem> dataSourceItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            kernel = new StandardKernel(new DependencyModule());

            // SetupDataSourceItems
            dataSourceItems = new List<DataSourceItem>()
            {
                new DataSourceItem()
                {
                    Description = null,
                    Name = "Test Data Source",
                    Path = "/SSRSMigrate_Tests/Test Data Source",
                    ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                    CredentialsRetrieval ="Integrated",
                    Enabled = true,
                    EnabledSpecified = true,
                    Extension = "SQL",
                    ImpersonateUser = false,
                    ImpersonateUserSpecified = true,
                    OriginalConnectStringExpressionBased = false,
                    Password = null,
                    Prompt = "Enter a user name and password to access the data source:",
                    UseOriginalConnectString = false,
                    UserName = null,
                    WindowsCredentials = false
                },
               new DataSourceItem()
                {
                    Description = null,
                    Name = "Test 2 Data Source",
                    Path = "/SSRSMigrate_Tests/Test 2 Data Source",
                    ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                    CredentialsRetrieval ="Integrated",
                    Enabled = true,
                    EnabledSpecified = true,
                    Extension = "SQL",
                    ImpersonateUser = false,
                    ImpersonateUserSpecified = true,
                    OriginalConnectStringExpressionBased = false,
                    Password = null,
                    Prompt = "Enter a user name and password to access the data source:",
                    UseOriginalConnectString = false,
                    UserName = null,
                    WindowsCredentials = false
                },
            };

            outputPath = Properties.Settings.Default.DestinationPath;

            writer = kernel.Get<IReportServerWriterFactory>().GetWriter<ReportServerWriter>("2005-DEST");
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
        }

        [TearDown]
        public void TearDown()
        {
            TeardownEnvironment();
        }

        #region Environment Setup/Teardown
        private void SetupEnvironment()
        {
            ReportingService2005Utility.SetupEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl, 
                CredentialCache.DefaultNetworkCredentials,
                outputPath);
        }

        private void TeardownEnvironment()
        {
            ReportingService2005Utility.TeardownEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath);
        }
        #endregion

        #region WriteDataSource Tests
        [Test]
        public void WriteDataSource()
        {

        }
        #endregion
    }
}
