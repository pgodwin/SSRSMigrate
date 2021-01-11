using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.TestHelper.Logging;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Validators;

namespace SSRSMigrate.Tests.SSRS.Reader
{
    [TestFixture]
    class ReportServerReaderTests
    {
        [Test]
        public void ReportServerReader_NullRepository()
        {
            MockLogger logger = new MockLogger();
            var validatorMock = new Mock<IReportServerPathValidator>();
            
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    ReportServerReader reader = new ReportServerReader(null, logger, validatorMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: repository"));
        }
    }
}
