using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Services.Protocols;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.IntegrationTests.SSRS.ReportServer2008
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_DataSourceTests
    {
        ReportServerReader reader = null;

        #region GetDataSources - Expected DataSourceItems
        List<DataSourceItem> expectedDataSourceItems = null;
        #endregion

        #region GetDataSources - Actual DataSourceItems
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
                    Description = null,
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
                    Description = null,
                    Name = "Test 2 Data Source",
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

            reader = DependencySingleton.Instance.Get<ReportServerReader>();
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
        public void GetDataSourceItem()
        {
            string dsPath = "/SSRSMigrate_Tests/Test Data Source";

            DataSourceItem actual = reader.GetDataSource(dsPath);

            Assert.AreEqual(expectedDataSourceItems[0].Name, actual.Name);
            Assert.AreEqual(expectedDataSourceItems[0].Path, actual.Path);
            Assert.AreEqual(expectedDataSourceItems[0].ConnectString, actual.ConnectString);
            Assert.AreEqual(expectedDataSourceItems[0].Description, actual.Description);
            Assert.AreEqual(expectedDataSourceItems[0].CredentialsRetrieval, actual.CredentialsRetrieval);
            Assert.AreEqual(expectedDataSourceItems[0].Enabled, actual.Enabled);
            Assert.AreEqual(expectedDataSourceItems[0].EnabledSpecified, actual.EnabledSpecified);
            Assert.AreEqual(expectedDataSourceItems[0].Extension, actual.Extension);
            Assert.AreEqual(expectedDataSourceItems[0].ImpersonateUser, actual.ImpersonateUser);
            Assert.AreEqual(expectedDataSourceItems[0].ImpersonateUserSpecified, actual.ImpersonateUserSpecified);
            Assert.AreEqual(expectedDataSourceItems[0].OriginalConnectStringExpressionBased, actual.OriginalConnectStringExpressionBased);
            Assert.AreEqual(expectedDataSourceItems[0].Password, actual.Password);
            Assert.AreEqual(expectedDataSourceItems[0].Prompt, actual.Prompt);
            Assert.AreEqual(expectedDataSourceItems[0].UseOriginalConnectString, actual.UseOriginalConnectString);
            Assert.AreEqual(expectedDataSourceItems[0].UserName, actual.UserName);
            Assert.AreEqual(expectedDataSourceItems[0].WindowsCredentials, actual.WindowsCredentials);
        }

        [Test]
        public void GetDataSourceItem_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSource(null);
                });

            Assert.That(ex.Message, Is.EqualTo("dataSourcePath"));
        }

        [Test]
        public void GetDataSourceItem_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetDataSource("");
                });

            Assert.That(ex.Message, Is.EqualTo("dataSourcePath"));
        }

        [Test]
        public void GetDataSourceItem_PathDoesntExist()
        {
            string dsPath = "/SSRSMigrate_Tests/Test Data Source Doesnt Exist";

            DataSourceItem actual = reader.GetDataSource(dsPath);

            Assert.IsNull(actual);
        }
        #endregion

        #region GetDataSources Tests
        [Test]
        public void GetDataSources()
        {
            string path = "/SSRSMigrate_Tests";

            List<DataSourceItem> actual = reader.GetDataSources(path);

            Assert.AreEqual(expectedDataSourceItems.Count(), actual.Count());
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

        [Test]
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException),
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found",
            MatchType = MessageMatch.Contains)]
        public void GetDataSources_PathDoesntExist()
        {
            string path = "/SSRSMigrate_Tests Doesnt Exist";

            List<DataSourceItem> actual = reader.GetDataSources(path);

            Assert.IsNull(actual);
        }
        #endregion

        #region GetDataSources Using Action<DataSourceItem> Tests
        [Test]
        public void GetDataSources_UsingDelegate()
        {
            string path = "/SSRSMigrate_Tests";

            reader.GetDataSources(path, GetDataSources_Reporter);

            Assert.AreEqual(expectedDataSourceItems.Count(), actualDataSourceItems.Count());
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
                    reader.GetDataSources("SSRSMigrate_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));
        }

        [Test]
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException),
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found",
            MatchType = MessageMatch.Contains)]
        public void GetDataSources_UsingDelegate_PathDoesntExist()
        {
            string path = "/SSRSMigrate_Tests Doesnt Exist";

            reader.GetDataSources(path, GetDataSources_Reporter);

            Assert.IsFalse(actualDataSourceItems.Any());
        }

        public void GetDataSources_Reporter(DataSourceItem dataSource)
        {
            if (dataSource != null)
                actualDataSourceItems.Add(dataSource);
        }
        #endregion
    }
}
