using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;

namespace SSRSMigrate.Tests.SSRS.ReportServer2008
{
    [TestFixture]
    class ReportServerReaderTests
    {
        [Test]
        public void ReportServerReader_NullRepository()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    ReportServerReader reader = new ReportServerReader(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: repository"));
        
        }
    }
}
