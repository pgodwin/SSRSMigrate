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
        public void GetDataSourceDefinition()
        {
            string dsPath = "/Test/Test Data Source";

            throw new NotImplementedException();

        }
        #endregion
    }
}
