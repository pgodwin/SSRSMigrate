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
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2005
{
    /// <summary>
    /// This integration test will write DataSourceItems to a ReportingServices2005 endpoint.
    /// The DataSourceItem objects used are already 'converted' to contain the destination information.
    /// </summary>
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
            outputPath = Properties.Settings.Default.DestinationPath;

            kernel = new StandardKernel(new DependencyModule());

            // Setup DataSourceItems
            dataSourceItems = new List<DataSourceItem>()
            {
                new DataSourceItem()
                {
                    Description = null,
                    Name = "Test Data Source",
                    Path = string.Format("{0}/Test Data Source", outputPath),
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
                    Path = string.Format("{0}/Test 2 Data Source", outputPath),
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
                // To test invalid path
                new DataSourceItem()
                {
                    Description = null,
                    Name = "Test.Data Source",
                    Path = string.Format("{0}/Test.Data Source", outputPath),
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

            //TODO Need to deploy a DataSource that already exists for the 'WriteDataSource_AlreadyExists' test
        }

        private void TeardownEnvironment()
        {
            ReportingService2005Utility.TeardownEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath);

            //TODO Teardown DataSource that already exists for the 'WriteDataSource_AlreadyExists' test
        }
        #endregion

        #region WriteDataSource Tests
        [Test]
        public void WriteDataSource()
        {
            string actual = writer.WriteDataSource(dataSourceItems[0]);

            // If actual is not null, then the DataSourceItem was not written and actual will contain the error
            Assert.Null(actual);
        }

        [Test]
        public void WriteDataSource_AlreadyExists()
        {
            //TODO Need to create the test DataSource during environment setup
            DataSourceAlreadyExistsException ex = Assert.Throws<DataSourceAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSource(dataSourceItems[0]);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The data source '{0}' already exists.", dataSourceItems[0].Path)));
        }

        [Test]
        public void WriteDataSource_NullDataSourceItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteDataSource(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: dataSourceItem"));
        }

        [Test]
        public void WriteDataSource_InvalidDataSourcePath()
        {
            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(dataSourceItems[2]);
                });

            Assert.That(ex.Message, Is.StringContaining(string.Format("Invalid path '{0}'.", dataSourceItems[2].Path)));
        
        }

        [Test]
        public void WriteDataSource_DataSourceItemNullName()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = null,
                Path = string.Format("{0}/Test Data Source", outputPath),
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemEmptyName()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "",
                Path = string.Format("{0}/Test Data Source", outputPath),
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemNullPath()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = null,
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemEmptyPath()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = "",
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }
        #endregion

        #region WriteDataSources Tests
        [Test]
        public void WriteDataSources()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_OneOrMoreAlreadyExists()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_NullDataSourceItems()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_OneOrMoreInvalidDataSourcePath()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemNullName()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyName()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemNullPath()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyPath()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
