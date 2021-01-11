using NUnit.Framework;
using System;
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Validators
{
    [TestFixture]
    class ReportServerItemNameValidator_Tests
    {
        private ReportServerItemNameValidator validator = null;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            MockLogger logger = new MockLogger();

            validator = new ReportServerItemNameValidator();
            validator.Logger = logger;
        }


        [Test]
        public void ValidName()
        {
            string name = "My Name";

            bool actual = validator.Validate(name);

            Assert.IsTrue(actual);
        }

        [Test]
        public void InvalidName_NULL()
        {
            string name = null;

            bool actual = validator.Validate(name);

            Assert.IsFalse(actual);
        }

        [Test]
        public void InvalidName_Empty()
        {
            string name = "";

            bool actual = validator.Validate(name);

            Assert.IsFalse(actual);
        }

        [Test]
        public void InvalidName_LengthExceeded()
        {
            string name = "MY Test";

            name += new String('x', 260);

            bool actual = validator.Validate(name);

            Assert.IsFalse(actual);
        }

        [Test]
        public void InvalidName_InvalidCharacters()
        {
            string name = "My Test & Test";

            bool actual = validator.Validate(name);

            Assert.IsFalse(actual);
        }
    }
}
