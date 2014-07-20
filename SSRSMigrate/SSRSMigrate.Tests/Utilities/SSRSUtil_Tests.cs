using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Utilities;

namespace SSRSMigrate.Tests.Utilities
{
    [TestFixture]
    class SSRSUtil_Tests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void UpdateReportDefinition()
        {
            byte[] expected = null;
            byte[] actual = SSRSUtil.UpdateReportDefinition(null, null);

            Assert.AreEqual(expected, "fail");
        }
    }
}
