using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Ionic.Zip;
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
        private Mock<IFileSystem> fileSystemMock = null;

        private string zipFileName = "C:\\temp\\SSRSMigrate_AW_Tests.zip";
        private string unPackDirectory = "C:\\temp\\";

        #region Test Values
        TestData dataSourceAW = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
            CheckSum = "7b4e44d94590f501ba24cd3904a925c3",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"
        };

        ZipEntryReadEvent dataSourceAWEventArgs = null;

        TestData dataSourceTest = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
            CheckSum = "c0815114c3ce9dde35eca314bbfe4bc9",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"
        };

        ZipEntryReadEvent dataSourceTestEventArgs = null;

        TestData folderRoot = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests"
        };

        ZipEntryReadEvent folderRootEventArgs = null;

        TestData folderDataSources = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Data Sources",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
        };

        ZipEntryReadEvent folderDataSourcesEventArgs = null;

        TestData reportsFolder = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportsFolderEventArgs = null;

        TestData subFolder = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Sub Folder"
        };

        ZipEntryReadEvent folderSubFolderEventArgs = null;

        TestData reportCompanySales = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl",
            CheckSum = "1adde7720ca2f0af49550fc676f70804",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportCompanySalesEventArgs = null;

        TestData reportSalesOrderDetail = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
            CheckSum = "640a2f60207f03779fdedfed71d8101d",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportSalesOrderDetailEventArgs = null;

        TestData reportStoreContacts = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportStoreContactsEventArgs = null;

        TestData reportDoesNotExist = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        ZipEntryReadEvent reportDoesNotExistEventArgs = null;

        TestData folderDoesNotExist = new TestData()
        {
            ExtractedTo = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist",
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
      ""CheckSum"": ""42352f007cf07bcec798b5ca9e4643a7""
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
      ""FileName"": ""Test Data Source.json"",
      ""CheckSum"": ""81a151f4b17aba7e1516d0801cadf4ee""
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
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
      ""FileName"": """",
      ""CheckSum"": """"
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
      ""FileName"": """",
      ""CheckSum"": """"
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder"",
      ""FileName"": """",
      ""CheckSum"": """"
    }
  ]
}";
        #endregion

        #region Actual Values
        private List<string> actualReports = null;
        private List<string> actualFolders = null;
        private List<string> actualDataSources = null; 
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
            fileSystemMock = new Mock<IFileSystem>();

            // IZipFileReaderWrapper.UnPack Method Mocks
            // Each time IZipFileReaderWrapper.OnEntryExtracted is called,
            //  get the next ZipEntryReadEvent from the queue and pass that
            //  to ZipBundleReader.EntryExtractedEventHandler instead.
            // Ref: http://haacked.com/archive/2009/09/29/moq-sequences.aspx/
            zipReaderMock.Setup(z => z.UnPack())
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

            // IZipFileReaderWrapper.FileName Property Mocks
            zipReaderMock.SetupSet(z => z.FileName = "NotAZip.txt")
                .Throws(new ZipException(string.Format("'NotAZip.txt' is not a valid zip archive.")));

            // IZipFileReaderWrapper.ReadEntry Method Mocks
            zipReaderMock.Setup(z => z.ReadEntry("ExportSummary.json"))
                .Returns(() => exportSummary);
            
            // ICheckSumGenerator.CreateCheckSum Method Mocks
            checkSumGenMock.Setup(c => c.CreateCheckSum(dataSourceAW.ExtractedTo))
                .Returns(() => "42352f007cf07bcec798b5ca9e4643a7");

            checkSumGenMock.Setup(c => c.CreateCheckSum(dataSourceTest.ExtractedTo))
                .Returns(() => "81a151f4b17aba7e1516d0801cadf4ee");

            checkSumGenMock.Setup(c => c.CreateCheckSum(folderRoot.ExtractedTo))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(folderDataSources.ExtractedTo))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(reportsFolder.ExtractedTo))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(subFolder.ExtractedTo))
                .Returns(() => "");

            checkSumGenMock.Setup(c => c.CreateCheckSum(reportCompanySales.ExtractedTo))
                .Returns(() => "1adde7720ca2f0af49550fc676f70804");

            checkSumGenMock.Setup(c => c.CreateCheckSum(reportSalesOrderDetail.ExtractedTo))
                .Returns(() => "640a2f60207f03779fdedfed71d8101d");

            checkSumGenMock.Setup(c => c.CreateCheckSum(reportStoreContacts.ExtractedTo))
                .Returns(() => "a225b92ed8475e6bc5b59f5b2cc396fa");

            checkSumGenMock.Setup(c => c.CreateCheckSum("C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Doesnt Exist.rdl"))
                .Throws(new FileNotFoundException("C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Doesnt Exist.rdl"));

            // ISystemIOWrapper.DirectoryExists Method Mocks
            fileSystemMock.Setup(i => i.Directory.Exists(folderRoot.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.Directory.Exists(folderDataSources.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.Directory.Exists(reportsFolder.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.Directory.Exists(subFolder.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.Directory.Exists(folderDoesNotExist.ExtractedTo))
                .Returns(() => false);

            // ISystemIOWrapper.FileExists Method Mocks
            fileSystemMock.Setup(i => i.File.Exists(dataSourceAW.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.File.Exists(dataSourceTest.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.File.Exists(reportCompanySales.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.File.Exists(reportSalesOrderDetail.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.File.Exists(reportStoreContacts.ExtractedTo))
                .Returns(() => true);

            fileSystemMock.Setup(i => i.File.Exists(reportDoesNotExist.ExtractedTo))
                .Returns(() => false);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {
            // Initialize for each test so the entries don't get duplicated in ZipBundleReader
            var logger = new MockLogger();

            zipBundleReader = new ZipBundleReader(
                zipFileName,
                unPackDirectory,
                zipReaderMock.Object,
                checkSumGenMock.Object,
                logger,
                fileSystemMock.Object
                );

            // Subscribe to the events again for each test
            zipBundleReader.OnDataSourceRead += OnDataSourceReadEvent;
            zipBundleReader.OnFolderRead += OnFolderReadEvent;
            zipBundleReader.OnReportRead += OnReportReadEvent;

            // Recreate each a list before each test
            actualReports = new List<string>();
            actualFolders = new List<string>();
            actualDataSources = new List<string>();
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
        
        #endregion

        #region Constructor Tests
        [Test]
        public void Constructor()
        {
            var logger = new MockLogger();

            var actual = new ZipBundleReader(
                zipFileName,
                unPackDirectory,
                zipReaderMock.Object,
                checkSumGenMock.Object,
                logger,
                fileSystemMock.Object);

            Assert.NotNull(actual);
        }

        [Test]
        public void Constructor_InvalidFileName()
        {
            ZipException ex = Assert.Throws<ZipException>(
                delegate
                {
                    var logger = new MockLogger();

                    var reader = new ZipBundleReader(
                                "NotAZip.txt",
                                unPackDirectory,
                                zipReaderMock.Object,
                                checkSumGenMock.Object,
                                logger,
                                fileSystemMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("'NotAZip.txt' is not a valid zip archive."));
        }
        #endregion

        #region Extract Tests
        [Test]
        public void Extract()
        {
            string actualUnPackedDirectory = zipBundleReader.Extract();

            zipReaderMock.Verify(z => z.UnPack());

            Assert.AreEqual(unPackDirectory, actualUnPackedDirectory);
        }
        #endregion
        
        #region ReadExportSummary Tests
        [Test]
        public void ReadExportSummary()
        {
            zipBundleReader.ReadExportSummary();

            zipReaderMock.Verify(z => z.ReadEntry("ExportSummary.json"));

            Assert.AreEqual(2, zipBundleReader.Entries["DataSources"].Count);
            Assert.AreEqual(3, zipBundleReader.Entries["Reports"].Count);
            Assert.AreEqual(3, zipBundleReader.Entries["Folders"].Count);
        }

        [Test]
        public void ReadExportSummary_EntryDoesntExist()
        {
            string entryName = "ExportSummary.json";

            var readerMock = new Mock<IZipFileReaderWrapper>();

            readerMock.Setup(z => z.ReadEntry(entryName))
                .Throws(new FileNotFoundException(entryName));

            var logger = new MockLogger();

            var reader = new ZipBundleReader(
                        zipFileName,
                        unPackDirectory,
                        readerMock.Object,
                        checkSumGenMock.Object,
                        logger,
                        fileSystemMock.Object);

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                delegate
                {
                    reader.ReadExportSummary();
                });

            Assert.That(ex.Message, Is.EqualTo(entryName));
        }

        [Test]
        public void ReadExportSummary_EmptySummary()
        {
            string entryName = "ExportSummary.json";

            var readerMock = new Mock<IZipFileReaderWrapper>();

            readerMock.Setup(z => z.ReadEntry(entryName))
                .Returns(() => null);

            var logger = new MockLogger();

            var reader = new ZipBundleReader(
                        zipFileName,
                        unPackDirectory,
                        readerMock.Object,
                        checkSumGenMock.Object,
                        logger,
                        fileSystemMock.Object);

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    reader.ReadExportSummary();
                });

            Assert.That(ex.Message, Is.EqualTo("No data in export summary."));
            Assert.AreEqual(0, reader.Entries["DataSources"].Count);
            Assert.AreEqual(0, reader.Entries["Reports"].Count);
            Assert.AreEqual(0, reader.Entries["Folders"].Count);
        }
        #endregion

        #region Read Tests
        [Test]
        public void Read()
        {
            zipBundleReader.ReadExportSummary();

            zipBundleReader.Read();

            Assert.AreEqual(2, actualDataSources.Count);
            Assert.AreEqual(3, actualFolders.Count);
            Assert.AreEqual(3, actualReports.Count);
        }

        //TODO Read_FileNotFound
        //TODO Read_DirectoryNotFound
        //TODO Read_ChecksumMismatch

        // Event handlers for ZipBundleReader's On*Read events
        private void OnFolderReadEvent(IBundleReader sender, ItemReadEvent e)
        {
            if (e.Success)
                actualFolders.Add(e.FileName);
        }

        private void OnDataSourceReadEvent(IBundleReader sender, ItemReadEvent e)
        {
            if (e.Success)
                actualDataSources.Add(e.FileName);
        }

        private void OnReportReadEvent(IBundleReader sender, ItemReadEvent e)
        {
            if (e.Success)
                actualReports.Add(e.FileName);
        }
        #endregion
    }
}
