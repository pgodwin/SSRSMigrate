using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;
using System.Web.Services.Protocols;

namespace SSRSMigrate.IntegrationTests.SSRS
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_DataSourceTests
    {
        ReportServerReader reader = null;
        List<DataSourceItem> actualDataSources = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
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
            actualDataSources = new List<DataSourceItem>();
        }

        [TearDown]
        public void TearDown()
        {
            actualDataSources = null;
        }

        #region GetDataSource Tests
        [Test]
        public void GetDataSourceItem()
        {
            string dsPath = "/SSRSMigrate_Tests/Test Data Source";

            DataSourceItem actual = reader.GetDataSource(dsPath);

            Assert.AreEqual(actual.Name, "Test Data Source");
            Assert.AreEqual(actual.Path, "/SSRSMigrate_Tests/Test Data Source");
            Assert.AreEqual(actual.ConnectString, "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest");
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

            Assert.AreEqual(actual.Count(), 2);
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

            Assert.AreEqual(actualDataSources.Count(), 2);
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

            Assert.IsFalse(actualDataSources.Any());
        }

        public void GetDataSources_Reporter(DataSourceItem dataSource)
        {
            if (dataSource != null)
                actualDataSources.Add(dataSource);
        }
        #endregion
    }
}
