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
    #region Test Structures
    // Holds data for test methods and mocks
    public struct TestData
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string CheckSum { get; set; }
        public string ZipPath { get; set; }
    };
    #endregion

    [TestFixture]
    [CoverageExcludeAttribute]
    class ZipBundler_Tests
    {
        ZipBundler zipBundler = null;
        Mock<IZipFileWrapper> zipFileMock = null;
        Mock<ICheckSumGenerator> checkSumGenMock = null;

        #region Test Values
        TestData awDataSource = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
            Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource",
            CheckSum = "7b4e44d94590f501ba24cd3904a925c3",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"
        };

        TestData testDataSource = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
            Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
            CheckSum = "c0815114c3ce9dde35eca314bbfe4bc9",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"
        };

        TestData rootFolder = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests",
            Path = "/SSRSMigrate_AW_Tests",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests"
        };

        TestData dataSourcesFolder = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources",
            Path = "/SSRSMigrate_AW_Tests/Data Sources",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
        };

        TestData reportsFolder = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports",
            Path = "/SSRSMigrate_AW_Tests/Reports",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData subFolder = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Sub Folder",
            Path = "/SSRSMigrate_AW_Tests/Sub Folder",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Sub Folder"
        };

        TestData companySalesReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
            CheckSum = "1adde7720ca2f0af49550fc676f70804",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl"
        };

        TestData salesOrderDetailReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
            CheckSum = "640a2f60207f03779fdedfed71d8101d",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl"
        };

        TestData storeContactsReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl"
        };

        TestData doesNotExistReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/File Doesnt Exist",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl"
        };
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
                awDataSource.FileName))
                .Returns(() => awDataSource.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(testDataSource.FileName
                ))
                .Returns(() => testDataSource.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                rootFolder.FileName))
                .Returns(() => rootFolder.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                dataSourcesFolder.FileName))
                .Returns(() => dataSourcesFolder.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                reportsFolder.FileName))
                .Returns(() => reportsFolder.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                subFolder.FileName))
                .Returns(() => subFolder.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                companySalesReport.FileName))
                .Returns(() => companySalesReport.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                salesOrderDetailReport.FileName))
                .Returns(() => salesOrderDetailReport.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                storeContactsReport.FileName))
                .Returns(() => storeContactsReport.CheckSum);

            checkSumGenMock.Setup(c => c.CreateCheckSum(
                doesNotExistReport.FileName))
                .Returns(() => doesNotExistReport.CheckSum);

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

        [Test]
        public void Constructor_Null_ZipFile()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    new ZipBundler(null, checkSumGenMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: zipFileWrapper"));
        }

        [Test]
        public void Constructor_Null_CheckSumGenFile()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    new ZipBundler(zipFileMock.Object, null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: checkSumGenerator"));
        }
        #endregion

        #region GetZipPath Tests
        [Test]
        public void GetZipPath_File()
        {
            string itemFileName = awDataSource.FileName;
            string itemPath = awDataSource.Path;

            string expectedZipPath = awDataSource.ZipPath;

            string actual = zipBundler.GetZipPath(itemFileName, itemPath);
            Assert.AreEqual(expectedZipPath, actual);
        }

        [Test]
        public void GetZipPath_File_NullFileName()
        {
            string itemPath = awDataSource.Path;

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
            string itemPath = awDataSource.Path;

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
            string itemFileName = awDataSource.FileName;

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
            string itemFileName = awDataSource.FileName;

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
            string itemFileName = awDataSource.FileName;
            string itemPath = "/SSRSMigrate/Data Sources/AWDataSource"; // This path is not contained within awDataSource.FileName, so it is invalid

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
            string itemFileName = awDataSource.FileName;
            string expectedZipPath = awDataSource.ZipPath;
            string expectedCheckSum = awDataSource.CheckSum;

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
            string zipPath = awDataSource.ZipPath;

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
            string zipPath = awDataSource.ZipPath;

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
            string itemFileName = awDataSource.FileName;
            
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
            string itemFileName = awDataSource.FileName;

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
            string itemFileName = doesNotExistReport.FileName;
            string zipPath = doesNotExistReport.ZipPath;

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
