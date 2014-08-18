using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ninject;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2005
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerWriter_DataSourceTests
    {
        StandardKernel kernel = null;
        ReportServerWriter writer = null;

        #region DataSourceItems
        List<DataSourceItem> dataSourceItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
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

        }

        private void TeardownEnvironment()
        {

        }
        #endregion
    }
}
