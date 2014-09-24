using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Writer
{
    [TestFixture]
    class ReportServerWriterTests
    {
        [Test]
        public void ReportServerWriter_NullRepository()
        {
            MockLogger logger = new MockLogger();

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
               delegate
               {
                   ReportServerWriter writer = new ReportServerWriter(null, logger);
               });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: repository"));
        
        }
    }
}
