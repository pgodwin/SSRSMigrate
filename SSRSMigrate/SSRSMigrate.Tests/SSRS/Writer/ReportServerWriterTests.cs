using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;

namespace SSRSMigrate.Tests.SSRS.Writer
{
    [TestFixture]
    class ReportServerWriterTests
    {
        [Test]
        public void ReportServerWriter_NullRepository()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
               delegate
               {
                   ReportServerWriter writer = new ReportServerWriter(null);
               });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: repository"));
        
        }
    }
}
