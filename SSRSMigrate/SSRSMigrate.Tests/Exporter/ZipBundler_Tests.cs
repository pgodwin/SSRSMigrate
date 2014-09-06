using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Exporter;
using Moq;
using SSRSMigrate.Wrappers;
using System.IO;

namespace SSRSMigrate.Tests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ZipBundler_Tests
    {
        ZipBundler zipBundler = null;
        Mock<IZipFileWrapper> zipFileMock = null;
        Mock<ICheckSumGenerator> checkSumGenMock = null;

        #region Test Values
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            zipFileMock = new Mock<IZipFileWrapper>();
            checkSumGenMock = new Mock<ICheckSumGenerator>();

            // IZipFileWrapper Mocks
            zipFileMock.Setup(z => z.AddDirectory(It.IsAny<string>()));
            zipFileMock.Setup(z => z.AddDirectory(It.IsAny<string>(), It.IsAny<string>()));
            zipFileMock.Setup(z => z.AddEntry(It.IsAny<string>(), It.IsAny<string>()));
            zipFileMock.Setup(z => z.AddFile(It.IsAny<string>()));
            zipFileMock.Setup(z => z.AddFile(It.IsAny<string>(), It.IsAny<string>()));
            zipFileMock.Setup(z => z.Dispose());
            zipFileMock.Setup(z => z.Save(It.IsAny<string>()));

            // ICheckSumGenerator Mocks
            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"))
                .Returns(() => "7b4e44d94590f501ba24cd3904a925c3");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"))
                .Returns(() => "c0815114c3ce9dde35eca314bbfe4bc9");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests"))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources"))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Reports"))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Sub Folder"))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl"))
                .Returns(() => "1adde7720ca2f0af49550fc676f70804");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl"))
                .Returns(() => "640a2f60207f03779fdedfed71d8101d");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl"))
                .Returns(() => "a225b92ed8475e6bc5b59f5b2cc396fa");

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl"))
                .Returns(() => "");

            zipBundler = new ZipBundler(zipFileMock.Object, checkSumGenMock.Object);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            zipBundler = null;
        }

        #region Constructor Tests
        [Test]
        public void Constructor()
        {
            ZipBundler actual = new ZipBundler(zipFileMock.Object, checkSumGenMock.Object);

            Assert.NotNull(actual);
        }
        #endregion

        #region GetZipPath Tests
        [Test]
        public void GetZipPath_File()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
            string itemPath = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource";

            string expectedZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

            string actual = zipBundler.GetZipPath(itemFileName, itemPath);
            Assert.AreEqual(expectedZipPath, actual);
        }

        [Test]
        public void GetZipPath_File_NullFileName()
        {
            string itemPath = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.GetZipPath(null, itemPath);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void GetZipPath_File_EmptyFileName()
        {
            string itemPath = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.GetZipPath("", itemPath);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void GetZipPath_File_NullPath()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.GetZipPath(itemFileName, null);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void GetZipPath_File_EmptyPath()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.GetZipPath(itemFileName, "");
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void GetZipPath_File_InvalidPath()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
            string itemPath = "/SSRSMigrate/Data Sources/AWDataSource";

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    zipBundler.GetZipPath(itemFileName, itemPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Item path '{0}' is invalid.", itemPath)));
        
        }
        #endregion

        #region CreateEntrySummary Tests
        /// <summary>
        /// Test CreateEntrySummary for a file by passing in valid values.
        /// </summary>
        [Test]
        public void CreateEntrySummary_File()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
            string expectedZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
            string expectedCheckSum = "7b4e44d94590f501ba24cd3904a925c3";

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, expectedZipPath);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedCheckSum, actual.CheckSum);
            Assert.AreEqual(expectedZipPath, actual.Path);
        }

        /// <summary>
        /// Test CreateEntrySummary for a file by passing in null value for itemFileName
        /// </summary>
        [Test]
        public void CreateEntrySummary_File_NullFileName()
        {
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.CreateEntrySummary(null, zipPath);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        /// <summary>
        /// Test CreateEntrySummary for a file by passing in an empty string for itemFileName
        /// </summary>
        [Test]
        public void CreateEntrySummary_File_EmptyFileName()
        {
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.CreateEntrySummary("", zipPath);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        /// <summary>
        /// Test CreateEntrySummary for a file by passing in null value for zipPath
        /// </summary>
        [Test]
        public void CreateEntrySummary_File_NullPath()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
            
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.CreateEntrySummary(itemFileName, null);
                });

            Assert.That(ex.Message, Is.EqualTo("zipPath"));
        }

        /// <summary>
        /// Test CreateEntrySummary for a file by passing in an empty string for zipPath
        /// </summary>
        [Test]
        public void CreateEntrySummary_File_EmptyPath()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.CreateEntrySummary(itemFileName, "");
                });

            Assert.That(ex.Message, Is.EqualTo("zipPath"));
        }

        /// <summary>
        /// Test CreateEntrySummary for a file that does not exist
        /// </summary>
        [Test]
        public void CreateEntrySummary_File_DoesntExist()
        {
            string itemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl";
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl";

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, zipPath);

            Assert.NotNull(actual);
            Assert.AreEqual("", actual.CheckSum);
            Assert.AreEqual(zipPath, actual.Path);
        }
        #endregion

        //TODO AddItem Tests that verifies entries property

        //TODO CreateSummary Tests

        //TODO Save Tests
    }
}
