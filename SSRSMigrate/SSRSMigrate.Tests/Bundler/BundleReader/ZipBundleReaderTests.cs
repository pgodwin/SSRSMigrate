using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SSRSMigrate.Bundler;
using SSRSMigrate.Bundler.Events;
using SSRSMigrate.TestHelper.Logging;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Tests.Bundler.BundlerReader
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
    class ZipBundleReaderTests
    {
        private ZipBundleReader zipBundleReader = null;
        private Mock<IZipFileReaderWrapper> zipReaderMock = null;
        private Mock<ICheckSumGenerator> checkSumGenMock = null;
        private string zipFileName = "C:\\temp\\SSRSMigrate_AW_Tests.zip";
        private string unPackDirectory = "C:\\temp\\";


        #region Test Values
        TestData awDataSource = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
            Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource",
            CheckSum = "7b4e44d94590f501ba24cd3904a925c3",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
        };

        TestData testDataSource = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
            Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
            CheckSum = "c0815114c3ce9dde35eca314bbfe4bc9",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
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
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData salesOrderDetailReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail",
            CheckSum = "640a2f60207f03779fdedfed71d8101d",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData storeContactsReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts",
            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData doesNotExistReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl",
            Path = "/SSRSMigrate_AW_Tests/Reports/File Doesnt Exist",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData folderDesNotExistReport = new TestData()
        {
            FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist",
            Path = "/SSRSMigrate_AW_Tests/Folder Doesnt Exist",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"
        };

        TestData summaryFile = new TestData()
        {
            FileName = "C:\\temp\\ExportSummary.json",
            Path = "",
            CheckSum = "",
            ZipPath = "ExportSummary.json"
        };
        #endregion

        #region Actual Values
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            zipReaderMock = new Mock<IZipFileReaderWrapper>();
            checkSumGenMock = new Mock<ICheckSumGenerator>();

            // IZipFileReaderWrapper.UnPackDirectory Property Mocks
            // IZipFileReaderWrapper.UnPack Method Mocks

            // ICheckSumGenerator.CreateCheckSum Method Mocks

            MockLogger logger = new MockLogger();
            zipBundleReader = new ZipBundleReader(
                zipFileName,
                zipReaderMock.Object,
                checkSumGenMock.Object,
                logger
                );

            zipBundleReader.OnDataSourceRead += OnDataSourceReadEvent;
            zipBundleReader.OnFolderRead += OnFolderReadEvent;
            zipBundleReader.OnReportRead += OnReportReadEvent;
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            zipBundleReader.OnDataSourceRead -= OnDataSourceReadEvent;
            zipBundleReader.OnFolderRead -= OnFolderReadEvent;
            zipBundleReader.OnReportRead -= OnReportReadEvent;

            zipBundleReader = null;
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {

        }

        #region Event Handlers
        private void OnFolderReadEvent(IBundleReader sender, ItemReadEvent e)
        {
            
        }

        private void OnDataSourceReadEvent(IBundleReader sender, ItemReadEvent e)
        {

        }

        private void OnReportReadEvent(IBundleReader sender, ItemReadEvent e)
        {

        }
        #endregion

        #region Constructor Tests
        [Test]
        public void Constructor()
        {
            MockLogger logger = new MockLogger();

            ZipBundleReader actual = new ZipBundleReader(
                zipFileName,
                zipReaderMock.Object,
                checkSumGenMock.Object,
                logger);

            Assert.NotNull(actual);
        }
        #endregion

        #region Extract Tests
        [Test]
        public void Extract()
        {
            ZipEntryReadEvent readEvent = new ZipEntryReadEvent(
                summaryFile.ZipPath,
                summaryFile.FileName,
                1,
                10);

            //TODO Trigger events during Extract()
            zipReaderMock.Raise(e => e.OnEntryExtracted += null, readEvent);

            string actualUnPackedDirectory = zipBundleReader.Extract(zipFileName, unPackDirectory);

            Assert.AreEqual(unPackDirectory, actualUnPackedDirectory);
        }
        #endregion
    }
}
