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
    class ReportServerReader_DataSourceTests
    {
        ReportServerReader reader = null;
        List<DataSourceItem> actualDataSources = null;

        [SetUp]
        public void SetUp()
        {
            reader = DependencySingleton.Instance.Get<ReportServerReader>();

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

        //TODO GetDataSourceItem_NullPath

        //TODO GetDataSourceItem_EmptyPath

        [Test]
        public void GetDataSourceItem_PathThatDoesntExist()
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

        //TODO GetDataSources_NullPath

        //TODO GetDataSources_EmptyPath

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
        
        //TODO GetDataSources_UsingDelegate_NullPath

        //TODO GetDataSources_UsingDelegate_EmptyPath

        [Test]
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException),
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found",
            MatchType = MessageMatch.Contains)]
        public void GetDataSources_UsingDelegate_PathDontExist()
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
