using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Validators
{
    [TestFixture]
    class ReportServerPathValidator_Tests
    {
        private ReportServerPathValidator validator = null;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            MockLogger logger = new MockLogger();

            validator = new ReportServerPathValidator();
            validator.Logger = logger;
        }


        [Test]
        public void ValidPath()
        {
            string path = "/SSRSMigrate_AW_Tests";

            bool actual = validator.Validate(path);

            Assert.IsTrue(actual);
        }

        [Test]
        public void InvalidPath_NULL()
        {
            string path = null;

            bool actual = validator.Validate(path);

            Assert.IsFalse(actual);
        }

        [Test]
        public void InvalidPath_Empty()
        {
            string path = "";

            bool actual = validator.Validate(path);

            Assert.IsFalse(actual);
        }

        [Test]
        public void InvalidPath_LengthExceeded()
        {
            string path = "/SSRSMigrate_AW_Tests";

            path += new String('x', 260);

            bool actual = validator.Validate(path);

            Assert.IsFalse(actual);
        }

        [Test]
        public void InvalidPath_InvalidCharacters()
        {
            string path = "/SSRSMigrate_AW_Tests & Test";

            bool actual = validator.Validate(path);

            Assert.IsFalse(actual);
        }
    }
}
