using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;
using SSRSMigrate.ReportServer2005;
using Moq;

namespace SSRSMigrate.Tests.SSRS
{
    [TestFixture]
    class ReportServerReader_DataSourceTests
    {
        ReportServerReader reader = null;

        #region GetDataSources - Expected DataSourceItems
        List<DataSourceItem> expectedDataSourceItems = null;
        #endregion

        #region GetDatasources - Actual DataSourceItems
        List<DataSourceItem> actualDataSourceItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Setup expected DataSourceItems
            expectedDataSourceItems = new List<DataSourceItem>()
            {
                new DataSourceItem()
                {
                    CreatedBy = "DOMAIN\\user",
                    CreationDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
                    Description = null,
                    ID = Guid.NewGuid().ToString(),
                    ModifiedBy = "DOMAIN\\user",
                    ModifiedDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
                    Size = 414,
                    VirtualPath = null,
                    Name = "Test Data Source",
                    Path = "/SSRSMigrate_Tests/Test Data Source",
                    ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                    CredentialsRetrieval = CredentialRetrievalEnum.Integrated,
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
                    CreatedBy = "DOMAIN\\user",
                    CreationDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                    Description = null,
                    ID = Guid.NewGuid().ToString(),
                    ModifiedBy = "DOMAIN\\user",
                    ModifiedDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                    Size = 414,
                    VirtualPath = null,
                    Name = "Test  2 Data Source",
                    Path = "/SSRSMigrate_Tests/Test 2 Data Source",
                    ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
                    CredentialsRetrieval = CredentialRetrievalEnum.Integrated,
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

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // IReportServerRepository.GetDataSource Mocks
            reportServerRepositoryMock.Setup(r => r.GetDataSource(null))
                .Throws(new ArgumentException("dataSourcePath"));

            reportServerRepositoryMock.Setup(r => r.GetDataSource(""))
                .Throws(new ArgumentException("dataSourcePath"));

            reportServerRepositoryMock.Setup(r => r.GetDataSource("/SSRSMigrate_Tests/Test Data Source"))
                .Returns(() => expectedDataSourceItems[0]);

            reportServerRepositoryMock.Setup(r => r.GetDataSource("/SSRSMigrate_Tests/Test 2 Data Source"))
                .Returns(() => expectedDataSourceItems[1]);

            reportServerRepositoryMock.Setup(r => r.GetDataSource("/SSRSMigrate_Tests/Test Data Source Doesnt Exist"))
                .Returns(() => null);

            // IReportServerRepository.GetDataSources Mocks
            reportServerRepositoryMock.Setup(r => r.GetDataSources(null))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetDataSources(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetDataSources("/SSRSMigrate_Tests"))
                .Returns(() => expectedDataSourceItems);

            reportServerRepositoryMock.Setup(r => r.GetDataSources("/SSRSMigrate_Tests Doesnt Exist"))
                .Returns(() => new List<DataSourceItem>());

            reportServerRepositoryMock.Setup(r => r.GetDataSourcesList(null))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetDataSourcesList(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetDataSourcesList("/SSRSMigrate_Tests"))
                .Returns(() => expectedDataSourceItems);

            reportServerRepositoryMock.Setup(r => r.GetDataSourcesList("/SSRSMigrate_Tests Doesnt Exist"))
                .Returns(() => new List<DataSourceItem>());

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
            actualDataSourceItems = new List<DataSourceItem>();
        }

        [TearDown]
        public void TearDown()
        {
            actualDataSourceItems = null;
        }

        #region GetDataSource Tests
        [Test]
        public void GetDataSource()
        {
            DataSourceItem actualDataSource = reader.GetDataSource("/SSRSMigrate_Tests/Test Data Source");

            Assert.AreEqual(expectedDataSourceItems[0], actualDataSource);
        }

        [Test]
        public void GetDataSource_PathDoesntExist()
        {
            DataSourceItem actualDataSource = reader.GetDataSource("/SSRSMigrate_Tests/Test Data Source Doesnt Exist");

            Assert.Null(actualDataSource);
        }

        [Test]
        public void GetDataSource_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSource(null);
                });

            Assert.That(ex.Message, Is.EqualTo("dataSourcePath"));
        }

        [Test]
        public void GetDataSource_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSource("");
                });

            Assert.That(ex.Message, Is.EqualTo("dataSourcePath"));
        }
        #endregion

        #region GetDataSources Tests
        [Test]
        public void GetDataSources()
        {
            List<DataSourceItem> dataSourceItems = reader.GetDataSources("/SSRSMigrate_Tests");

            Assert.AreEqual(dataSourceItems.Count(), expectedDataSourceItems.Count());
            Assert.AreEqual(expectedDataSourceItems[0], dataSourceItems[0]);
            Assert.AreEqual(expectedDataSourceItems[1], dataSourceItems[1]);
        }

        [Test]
        public void GetDataSources_PathDoesntExist()
        {
            List<DataSourceItem> dataSourceItems = reader.GetDataSources("/SSRSMigrate_Tests Doesnt Exist");

            Assert.AreEqual(0, dataSourceItems.Count());
        }

        [Test]
        public void GetDataSources_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSources(null);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetDataSources_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSources("");
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }
        #endregion

        #region GetDataSources Using Action<DataSourceItem> Tests
        [Test]
        public void GetDataSources_UsingDelegate()
        {
            reader.GetDataSources("/SSRSMigrate_Tests", GetDataSources_Reporter);

            Assert.AreEqual(expectedDataSourceItems.Count(), actualDataSourceItems.Count());
            Assert.AreEqual(expectedDataSourceItems[0], actualDataSourceItems[0]);
            Assert.AreEqual(expectedDataSourceItems[1], actualDataSourceItems[1]);
        }

        [Test]
        public void GetDataSources_UsingDelegate_PathDoesntExist()
        {
            reader.GetDataSources("/SSRSMigrate_Tests Doesnt Exist", GetDataSources_Reporter);

            Assert.AreEqual(0, actualDataSourceItems.Count());
        }

        [Test]
        public void GetDataSources_UsingDelegate_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSources(null, GetDataSources_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetDataSources_UsingDelegate_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSources("", GetDataSources_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetDataSources_UsingDelegate_NullDelegate()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    reader.GetDataSources("/SSRSMigrate_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));
        }

        private void GetDataSources_Reporter(DataSourceItem dataSourceItem)
        {
            actualDataSourceItems.Add(dataSourceItem);
        }
        #endregion
    }
}
