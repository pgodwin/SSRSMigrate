using System;
using System.Collections.Generic;
using System.IO;
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
        public string ExtractedTo { get; set; }
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
        TestData dataSourceAW = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
            CheckSum = "7b4e44d94590f501ba24cd3904a925c3",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"
        };

        ZipEntryReadEvent dataSourceAWEventArgs = null;

        TestData dataSourceTest = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
            CheckSum = "c0815114c3ce9dde35eca314bbfe4bc9",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"
        };

        ZipEntryReadEvent dataSourceTestEventArgs = null;

        TestData folderRoot = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests"
        };

        ZipEntryReadEvent folderRootEventArgs = null;

        TestData folderDataSources = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Data Sources",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
        };

        ZipEntryReadEvent folderDataSourcesEventArgs = null;

        TestData reportsFolder = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportsFolderEventArgs = null;

        TestData subFolder = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Sub Folder",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Sub Folder"
        };

        ZipEntryReadEvent folderSubFolderEventArgs = null;

        TestData reportCompanySales = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl",
            CheckSum = "1adde7720ca2f0af49550fc676f70804",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportCompanySalesEventArgs = null;

        TestData reportSalesOrderDetail = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
            CheckSum = "640a2f60207f03779fdedfed71d8101d",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportSalesOrderDetailEventArgs = null;

        TestData reportStoreContacts = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportStoreContactsEventArgs = null;

        TestData reportDoesNotExist = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportDoesNotExistEventArgs = null;

        TestData folderDoesNotExist = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"
        };

        ZipEntryReadEvent folderDoesNotExistEventArgs = null;

        TestData summaryFile = new TestData()
        {
            ExtractedTo = "C:\\temp\\ExportSummary.json",
            CheckSum = "",
            ZipPath = "ExportSummary.json"
        };

        ZipEntryReadEvent summaryFileEventArgs = null;

        string exportSummary = @"{
  ""DataSources"": [
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
      ""FileName"": ""AWDataSource.json"",
      ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
    }
  ],
  ""Reports"": [
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
      ""FileName"": ""Company Sales.rdl"",
      ""CheckSum"": ""1adde7720ca2f0af49550fc676f70804""
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
      ""FileName"": ""Sales Order Detail.rdl"",
      ""CheckSum"": ""640a2f60207f03779fdedfed71d8101d""
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
      ""FileName"": ""Store Contacts.rdl"",
      ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
    }
  ],
  ""Folders"": [
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests"",
      ""FileName"": """",
      ""CheckSum"": """"
    }
  ]
}";
        #endregion

        #region Actual Values
        #endregion

        #region Value Setup
        private void SetupTestValues()
        {
            dataSourceAWEventArgs = new ZipEntryReadEvent(
                dataSourceAW.ZipPath,
                dataSourceAW.ExtractedTo);

            dataSourceTestEventArgs = new ZipEntryReadEvent(
                dataSourceTest.ZipPath,
                 dataSourceTest.ExtractedTo);

            folderRootEventArgs = new ZipEntryReadEvent(
                folderRoot.ZipPath,
                folderRoot.ExtractedTo);

            folderDataSourcesEventArgs = new ZipEntryReadEvent(
                folderDataSources.ZipPath,
                folderDataSources.ExtractedTo);

            reportsFolderEventArgs = new ZipEntryReadEvent(
                reportsFolder.ZipPath,
                reportsFolder.ExtractedTo);

            folderSubFolderEventArgs = new ZipEntryReadEvent(
                subFolder.ZipPath,
                subFolder.ExtractedTo);

            reportCompanySalesEventArgs = new ZipEntryReadEvent(
                reportCompanySales.ZipPath,
                reportCompanySales.ExtractedTo);

            reportSalesOrderDetailEventArgs = new ZipEntryReadEvent(
                reportSalesOrderDetail.ZipPath,
                reportSalesOrderDetail.ExtractedTo);

            reportStoreContactsEventArgs = new ZipEntryReadEvent(
                reportStoreContacts.ZipPath,
                reportStoreContacts.ExtractedTo);

            reportDoesNotExistEventArgs = new ZipEntryReadEvent(
                reportDoesNotExist.ZipPath,
                reportDoesNotExist.ExtractedTo);

            folderDoesNotExistEventArgs = new ZipEntryReadEvent(
                folderDoesNotExist.ZipPath,
                folderDoesNotExist.ExtractedTo);

            summaryFileEventArgs = new ZipEntryReadEvent(
                summaryFile.ZipPath,
                summaryFile.ExtractedTo);
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.SetupTestValues();

            zipReaderMock = new Mock<IZipFileReaderWrapper>();
            checkSumGenMock = new Mock<ICheckSumGenerator>();

            // IZipFileReaderWrapper.UnPack Method Mocks
            // Each time IZipFileReaderWrapper.OnEntryExtracted is called,
            //  get the next ZipEntryReadEvent from the queue and pass that
            //  to ZipBundleReader.EntryExtractedEventHandler instead.
            // Ref: http://haacked.com/archive/2009/09/29/moq-sequences.aspx/
            zipReaderMock.Setup(z => z.UnPack(zipFileName, unPackDirectory))
                .Returns(() => unPackDirectory)
                .Raises(z => z.OnEntryExtracted += null,
                    new Queue<ZipEntryReadEvent>(new[]
                    {
                        folderRootEventArgs,
                        folderDataSourcesEventArgs,
                        reportsFolderEventArgs,
                        folderSubFolderEventArgs,
                        dataSourceAWEventArgs,
                        dataSourceTestEventArgs,
                        reportCompanySalesEventArgs,
                        reportSalesOrderDetailEventArgs,
                        reportStoreContactsEventArgs,
                        summaryFileEventArgs
                    }).Dequeue);

            // ICheckSumGenerator.CreateCheckSum Method Mocks
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
            // Initialize for each test so the entries don't get duplicated in ZipBundleReader
            MockLogger logger = new MockLogger();

            zipBundleReader = new ZipBundleReader(
                zipFileName,
                unPackDirectory,
                zipReaderMock.Object,
                checkSumGenMock.Object,
                logger
                );

            // Subscribe to the events again for each test
            zipBundleReader.OnDataSourceRead += OnDataSourceReadEvent;
            zipBundleReader.OnFolderRead += OnFolderReadEvent;
            zipBundleReader.OnReportRead += OnReportReadEvent;
        }

        [TearDown]
        public void TearDown()
        {
            // Unsubscribe to events after each test
            zipBundleReader.OnDataSourceRead -= OnDataSourceReadEvent;
            zipBundleReader.OnFolderRead -= OnFolderReadEvent;
            zipBundleReader.OnReportRead -= OnReportReadEvent;

            zipBundleReader = null;
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
            string actualUnPackedDirectory = zipBundleReader.Extract();

            Assert.AreEqual(unPackDirectory, actualUnPackedDirectory);
            zipReaderMock.Verify(z => z.UnPack(zipFileName, unPackDirectory));
        }
        #endregion
    }
}
