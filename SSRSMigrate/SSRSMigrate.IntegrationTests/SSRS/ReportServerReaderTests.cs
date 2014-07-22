using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;

namespace SSRSMigrate.IntegrationTests.SSRS
{
    [TestFixture]
    class ReportServerReaderTests
    {
        ReportServerReader reader = null;

        [SetUp]
        public void SetUp()
        {
            reader = DependencySingleton.Instance.Get<ReportServerReader>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region Data Source Tests
        [Test]
        public void GetDataSourceItem()
        {
            string dsPath = "/SSRSMigrate_Tests/Test Data Source";

            DataSourceItem actual = reader.GetDataSource(dsPath);

            Assert.AreEqual(actual.Name, "Test Data Source");
            Assert.AreEqual(actual.Path, "/SSRSMigrate_Tests/Test Data Source");
            Assert.AreEqual(actual.ConnectString, "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest");
        }
        #endregion
    }
}
