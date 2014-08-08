using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;

namespace SSRSMigrate.IntegrationTests.SSRS.ReportServer2010
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_FolderTests
    {
        ReportServerReader reader = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            reader = DependencySingleton.Instance.Get<ReportServerReader>();
        }
    }
}
