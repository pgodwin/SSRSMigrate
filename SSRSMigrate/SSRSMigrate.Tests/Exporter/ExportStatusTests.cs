using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Status;

namespace SSRSMigrate.Tests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ExportStatusTests
    {
        [Test]
        public void Succeed()
        {
            string expectedToPath = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports";
            string expectedFromPath = "/SSRSMigrate_AW_Tests/Reports";
            bool expectedSuccess = true;
            string[] expectedErrors = null;

            ExportStatus actualStatus = new ExportStatus(expectedToPath, expectedFromPath, expectedErrors, expectedSuccess);

            Assert.AreEqual(expectedToPath, actualStatus.ToPath);
            Assert.AreEqual(expectedFromPath, actualStatus.FromPath);
            Assert.AreEqual(expectedErrors, actualStatus.Errors);
            Assert.AreEqual(expectedSuccess, actualStatus.Success);
        }

        [Test]
        public void WithErrors()
        {
            string expectedToPath = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports";
            string expectedFromPath = "/SSRSMigrate_AW_Tests/Reports";
            bool expectedSuccess = false;
            string[] expectedErrors = new string[] { "Permission denied." };

            ExportStatus actualStatus = new ExportStatus(expectedToPath, expectedFromPath, expectedErrors, expectedSuccess);

            Assert.AreEqual(expectedToPath, actualStatus.ToPath);
            Assert.AreEqual(expectedFromPath, actualStatus.FromPath);
            Assert.AreEqual(expectedErrors, actualStatus.Errors);
            Assert.AreEqual(expectedSuccess, actualStatus.Success);
        }

        [Test]
        public void NullToPath()
        {
            string expectedFromPath = "/SSRSMigrate_AW_Tests/Reports";
            bool expectedSuccess = true;
            string[] expectedErrors = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    ExportStatus actualStatus = new ExportStatus(null, expectedFromPath, expectedErrors, expectedSuccess);
                });

            Assert.That(ex.Message, Is.EqualTo("pathTo"));
        }

        [Test]
        public void EmptyToPath()
        {
            string expectedFromPath = "/SSRSMigrate_AW_Tests/Reports";
            bool expectedSuccess = true;
            string[] expectedErrors = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    ExportStatus actualStatus = new ExportStatus("", expectedFromPath, expectedErrors, expectedSuccess);
                });

            Assert.That(ex.Message, Is.EqualTo("pathTo"));
        }

        [Test]
        public void NullFromPath()
        {
            string expectedToPath = "C:\\temp\\SSRSMigrate_Tests\\Reports";
            bool expectedSuccess = true;
            string[] expectedErrors = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    ExportStatus actualStatus = new ExportStatus(expectedToPath, null, expectedErrors, expectedSuccess);
                });

            Assert.That(ex.Message, Is.EqualTo("pathFrom"));
        }

        [Test]
        public void EmptyFromPath()
        {
            string expectedToPath = "C:\\temp\\SSRSMigrate_Tests\\Reports";
            bool expectedSuccess = true;
            string[] expectedErrors = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    ExportStatus actualStatus = new ExportStatus(expectedToPath, "", expectedErrors, expectedSuccess);
                });

            Assert.That(ex.Message, Is.EqualTo("pathFrom"));
        }
    }
}
