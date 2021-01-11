using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SSRSMigrate.SSRS.Validators;
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
            var validatorMock = new Mock<IReportServerPathValidator>();

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
               delegate
               {
                   ReportServerWriter writer = new ReportServerWriter(null, logger, validatorMock.Object);
               });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: repository"));
        
        }

    }
}
