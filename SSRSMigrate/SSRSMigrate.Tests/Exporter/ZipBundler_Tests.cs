using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Exporter;
using Moq;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Tests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ZipBundler_Tests
    {
        ZipBundler zipBundler = null;
        Mock<IZipFileWrapper> zipFileMock = null;
        Mock<ICheckSumGenerator> checkSumGenMock = null;

        #region Expected Values
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            zipFileMock = new Mock<IZipFileWrapper>();
            checkSumGenMock = new Mock<ICheckSumGenerator>();

            zipBundler = new ZipBundler(zipFileMock.Object, checkSumGenMock.Object);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {

        }
    }
}
