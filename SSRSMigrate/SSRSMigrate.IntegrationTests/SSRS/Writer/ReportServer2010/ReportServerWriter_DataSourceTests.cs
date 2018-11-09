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
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2010
{
    /// <summary>
    /// These integration tests will write DataSourceItems to a ReportingService2010 endpoint.
    /// The DataSourceItem objects used are already 'converted' to contain the destination information.
    /// </summary>
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerWriter_DataSourceTests
    {
        IReportServerWriter writer = null;
        string outputPath = null;

        #region DataSourceItems
        List<DataSourceItem> dataSourceItems = null;
        DataSourceItem alreadyExistsDataSourceItem = null;
        DataSourceItem invalidPathDataSourceItem = null;
        DataSourceItem nullNameDataSourceItem = null;
        DataSourceItem emptyNameDataSourceItem = null;
        DataSourceItem nullPathDataSourceItem = null;
        DataSourceItem emptyPathDataSourceItem = null;
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            outputPath = Properties.Settings.Default.DestinationPath;

            // Setup DataSourceItems
            dataSourceItems = new List<DataSourceItem>()
            {
                new DataSourceItem()
                {
                    Description = null,
                    Name = "AWDataSource",
                    Path = string.Format("{0}/Data Sources/AWDataSource", outputPath),
                    ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
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
                    Name = "Test Data Source",
                    Path = string.Format("{0}/Data Sources/Test Data Source", outputPath),
                    ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
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
                }
            };

            // To test invalid path
            invalidPathDataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = "Test.Data Source",
                Path = string.Format("{0}/Data Sources/Test.Data Source", outputPath),
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
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
            };

            alreadyExistsDataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = "Data Source Already Exists",
                Path = string.Format("{0}/Data Sources/Data Source Already Exists", outputPath),
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
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
            };

            nullNameDataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = null,
                Path = string.Format("{0}/Data Sources/Test Data Source", outputPath),
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
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
            };

            emptyNameDataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = null,
                Path = string.Format("{0}/Data Sources/Test Data Source", outputPath),
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
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
            };

            nullPathDataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = "Test Data Source",
                Path = null,
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
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
            };

            emptyPathDataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = "Test Data Source",
                Path = "",
                ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
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
            };


            writer = TestKernel.Instance.Get<IReportServerWriter>("2010-DEST");
        }

        [OneTimeTearDown]
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
            ReportingService2010TestEnvironment.SetupEnvironment(
                Properties.Settings.Default.ReportServer2008R2WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath,
                TestContext.CurrentContext.TestDirectory);
        }

        private void TeardownEnvironment()
        {
            ReportingService2010TestEnvironment.TeardownEnvironment(
                Properties.Settings.Default.ReportServer2008R2WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath);
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
        public void WriteDataSource_AlreadyExists_OverwriteDisallow()
        {
            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSource(alreadyExistsDataSourceItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSource_AlreadyExists_OverwriteAllowed()
        {
            writer.Overwrite = true; // Allow overwriting of data source

            string actual = writer.WriteDataSource(alreadyExistsDataSourceItem);

            Assert.Null(actual);
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
                    writer.WriteDataSource(invalidPathDataSourceItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPathDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSource_DataSourceItemNullName()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = null,
                Path = string.Format("{0}/Data Sources/Test Data Source", outputPath),
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
                Path = string.Format("{0}/Data Sources/Test Data Source", outputPath),
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

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", dataSource.Path)));
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

            Assert.That(ex.Message, Does.Contain("Invalid path"));
        }
        #endregion

        #region WriteDataSources Tests
        [Test]
        public void WriteDataSources()
        {
            string[] actual = writer.WriteDataSources(dataSourceItems.ToArray());

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void WriteDataSources_OneOrMoreAlreadyExists_OverwriteDisallowed()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();
            items.AddRange(dataSourceItems);
            items.Add(alreadyExistsDataSourceItem);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSources_OneOrMoreAlreadyExists_OverwriteAllowed()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();
            items.AddRange(dataSourceItems);
            items.Add(alreadyExistsDataSourceItem);

            writer.Overwrite = true; // Allow overwriting of data soruce

            string[] actual = writer.WriteDataSources(items.ToArray());

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void WriteDataSources_NullDataSourceItems()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteDataSource(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: dataSourceItem"));

        }

        [Test]
        public void WriteDataSources_OneOrMoreInvalidDataSourcePath()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();
            items.AddRange(dataSourceItems);
            items.Add(invalidPathDataSourceItem);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPathDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemNullName()
        {
            List<DataSourceItem> items = new List<DataSourceItem>()
            {
               nullNameDataSourceItem
            };

            items.AddRange(dataSourceItems);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyName()
        {
            List<DataSourceItem> items = new List<DataSourceItem>()
            {
               emptyNameDataSourceItem
            };

            items.AddRange(dataSourceItems);

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemNullPath()
        {
            List<DataSourceItem> items = new List<DataSourceItem>()
            {
               nullPathDataSourceItem
            };

            items.AddRange(dataSourceItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", nullPathDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyPath()
        {
            List<DataSourceItem> items = new List<DataSourceItem>()
            {
               emptyPathDataSourceItem
            };

            items.AddRange(dataSourceItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Does.Contain("Invalid path"));
        }
        #endregion
    }
}
