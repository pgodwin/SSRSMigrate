using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Ninject;
using Ninject.Extensions.Logging.Log4net;
using NUnit.Framework;
using SSRSMigrate.Bundler;
using SSRSMigrate.Bundler.Events;
using SSRSMigrate.Errors;
using SSRSMigrate.TestHelper.Logging;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.IntegrationTests.Bundler.BundleReader
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
    class ZipBundleReader_Tests
    {
        private StandardKernel kernel = null;
        private IBundleReader bundleReader = null;

        #region Test Data
        private string zipFilename = "SSRSMigrate_AW_Tests.zip";
        private string unpackDir = null;
        #endregion

//        #region Expected Values
//        TestData dataSourceAW = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
//            CheckSum = "7b4e44d94590f501ba24cd3904a925c3",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"
//        };

//        TestData dataSourceTest = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
//            CheckSum = "c0815114c3ce9dde35eca314bbfe4bc9",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"
//        };

//        TestData folderRoot = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests",
//            CheckSum = "",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests"
//        };

//        TestData folderDataSources = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources",
//            CheckSum = "",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
//        };

//        TestData reportsFolder = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports",
//            CheckSum = "",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
//        };

//        TestData subFolder = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
//            CheckSum = "",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Sub Folder"
//        };

//        TestData reportCompanySales = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl",
//            CheckSum = "1adde7720ca2f0af49550fc676f70804",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
//        };

//        TestData reportSalesOrderDetail = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
//            CheckSum = "640a2f60207f03779fdedfed71d8101d",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
//        };

//        TestData reportStoreContacts = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
//            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
//        };

//        TestData reportDoesNotExist = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl",
//            CheckSum = "",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
//        };

//        TestData folderDoesNotExist = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist",
//            CheckSum = "",
//            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"
//        };

//        TestData summaryFile = new TestData()
//        {
//            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\ExportSummary.json",
//            CheckSum = "",
//            ZipPath = "ExportSummary.json"
//        };

//        private string exportSummary = @"{
//          ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
//          ""SourceVersion"": ""SqlServer2008R2"",
//          ""Entries"": {
//            ""DataSources"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""AWDataSource.json"",
//                ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""Test Data Source.json"",
//                ""CheckSum"": ""81a151f4b17aba7e1516d0801cadf4ee""
//              }
//            ],
//            ""Reports"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Company Sales.rdl"",
//                ""CheckSum"": ""1adde7720ca2f0af49550fc676f70804""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Sales Order Detail.rdl"",
//                ""CheckSum"": ""640a2f60207f03779fdedfed71d8101d""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Store Contacts.rdl"",
//                ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
//              }
//            ],
//            ""Folders"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              }
//            ]
//          }
//        }";

//        // For testing when a file in the export summary does not exist on disk
//        string exportSummaryFileDoesntExist = @"{
//          ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
//          ""SourceVersion"": ""SqlServer2008R2"",
//          ""Entries"": {
//            ""DataSources"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""AWDataSource.json"",
//                ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""Test Data Source.json"",
//                ""CheckSum"": ""81a151f4b17aba7e1516d0801cadf4ee""
//              }
//            ],
//            ""Reports"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Company Sales.rdl"",
//                ""CheckSum"": ""1adde7720ca2f0af49550fc676f70804""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Sales Order Detail.rdl"",
//                ""CheckSum"": ""640a2f60207f03779fdedfed71d8101d""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Store Contacts.rdl"",
//                ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""File Doesnt Exist.rdl"",      
//                ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
//              }
//            ],
//            ""Folders"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              }
//            ]
//          }
//        }";

//        // For testing when a directory in the export summary does not exist on disk
//        string exportSummaryDirectoryDoesntExist = @"{
//          ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
//          ""SourceVersion"": ""SqlServer2008R2"",
//          ""Entries"": {
//            ""DataSources"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""AWDataSource.json"",
//                ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""Test Data Source.json"",
//                ""CheckSum"": ""81a151f4b17aba7e1516d0801cadf4ee""
//              }
//            ],
//            ""Reports"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Company Sales.rdl"",
//                ""CheckSum"": ""1adde7720ca2f0af49550fc676f70804""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Sales Order Detail.rdl"",
//                ""CheckSum"": ""640a2f60207f03779fdedfed71d8101d""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Store Contacts.rdl"",
//                ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
//              }
//            ],
//            ""Folders"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              }
//            ]
//          }
//        }";

//        // For testing when a checksum does not match
//        string exportSummaryChecksumMismatch = @"{
//          ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
//          ""SourceVersion"": ""SqlServer2008R2"",
//          ""Entries"": {
//            ""DataSources"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""AWDataSource.json"",
//                ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": ""Test Data Source.json"",
//                ""CheckSum"": ""81a151f4b17aba7e1516d0801cadf4ee""
//              }
//            ],
//            ""Reports"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Company Sales.rdl"",
//                ""CheckSum"": ""BAD CHECKSUM HERE""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Sales Order Detail.rdl"",
//                ""CheckSum"": ""640a2f60207f03779fdedfed71d8101d""
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": ""Store Contacts.rdl"",
//                ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
//              }
//            ],
//            ""Folders"": [
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              },
//              {
//                ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
//                ""FileName"": """",
//                ""CheckSum"": """"
//              }
//            ]
//          }
//        }";
//        #endregion

        #region Actual Values
        private List<string> actualReports = null;
        private List<string> actualFolders = null;
        private List<string> actualDataSources = null;
        #endregion

        #region Environment Methods
        private string GetTempExtractPath()
        {
            string path = Path.Combine(Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

            return path;
        }

        private string GetTestDataPath()
        {
            string outputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            outputPath = outputPath.Replace("file:\\", "");

            return Path.Combine(outputPath, "Test AW Data\\Imports\\");
        }

        private void EnvironmentSetup()
        {
            // Get a random directory in %TEMP% and use it as the directory
            //  to extract the archive into
            unpackDir = GetTempExtractPath();

            if (Directory.Exists(unpackDir))
                Directory.Delete(unpackDir, true);

            Directory.CreateDirectory(unpackDir);
        }

        private void EnvironmentTearDown()
        {
            if (Directory.Exists(unpackDir))
                Directory.Delete(unpackDir, true);
        }
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            kernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new DependencyModule());
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            // Setup the environment on disk before each test
            this.EnvironmentSetup();

            bundleReader = kernel.Get<IBundleReader>();

            // Set IBundleReader's properties for the archive to extract
            //  and where to extract it to
            bundleReader.ArchiveFileName = Path.Combine(GetTestDataPath(), zipFilename);
            bundleReader.UnPackDirectory = unpackDir;

            // Subscribe to the events again for each test
            bundleReader.OnDataSourceRead += OnDataSourceReadEvent;
            bundleReader.OnFolderRead += OnFolderReadEvent;
            bundleReader.OnReportRead += OnReportReadEvent;

            // Recreate each a list before each test
            actualReports = new List<string>();
            actualFolders = new List<string>();
            actualDataSources = new List<string>();
        }

        [TearDown]
        public void TearDown()
        {
            // Tear down the evironment that is on disk after each test
            this.EnvironmentTearDown();

            // Unsubscribe to events after each test
            bundleReader.OnDataSourceRead -= OnDataSourceReadEvent;
            bundleReader.OnFolderRead -= OnFolderReadEvent;
            bundleReader.OnReportRead -= OnReportReadEvent;

            bundleReader = null;
        }

        #region Constructor Tests
        /// <summary>
        /// Test the contructor when passed all valid parameters.
        /// </summary>
        [Test]
        public void Constructor()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), zipFilename);

            var actualReader = new ZipBundleReader(
                zipFile,
                unpackDir,
                zipReader,
                checkSumGen,
                logger,
                fileSystem,
                serializeWrapper);

            Assert.NotNull(actualReader);

            actualReader = null;
            zipReader = null;
            checkSumGen = null;
            fileSystem = null;
            serializeWrapper = null;
        }

        /// <summary>
        /// Test the constructor when passed a filename parameter that is not a zip archive.
        /// </summary>
        [Test]
        public void Constructor_InvalidFileName()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), "NotAZip.txt");

            InvalidFileArchiveException ex = Assert.Throws<InvalidFileArchiveException>(
                delegate
                {
                    var actualReader = new ZipBundleReader(
                        zipFile,
                        unpackDir,
                        zipReader,
                        checkSumGen,
                        logger,
                        fileSystem,
                        serializeWrapper);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("'{0}' is not a valid archive.", zipFile)));
        
            zipReader = null;
            checkSumGen = null;
            fileSystem = null;
            serializeWrapper = null;
        }

        /// <summary>
        /// Tests the constructor when passed a filename that does not exist.
        /// </summary>
        [Test]
        public void Constructor_ArchiveNotFound()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), "NotFound.zip");

            InvalidFileArchiveException ex = Assert.Throws<InvalidFileArchiveException>(
                delegate
                {
                    var actualReader = new ZipBundleReader(
                       zipFile,
                       unpackDir,
                       zipReader,
                       checkSumGen,
                       logger,
                       fileSystem,
                       serializeWrapper);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("'{0}' is not a valid archive.", zipFile)));

            zipReader = null;
            checkSumGen = null;
            fileSystem = null;
            serializeWrapper = null;
        }
        #endregion

        #region Extract Tests
        /// <summary>
        /// Test extracting the zip archive successfully.
        /// </summary>
        [Test]
        public void Extract()
        {
            string actualUnPackedDirectory = bundleReader.Extract();

            // Verify extracted structure matches what we expect
            Assert.True(Directory.Exists(actualUnPackedDirectory), "UnpackedDirectory");
            Assert.True(Directory.Exists(Path.Combine(actualUnPackedDirectory, "Export")), "UnpackedDirectory\\Export");
            Assert.True(File.Exists(Path.Combine(actualUnPackedDirectory, "ExportSummary.json")),
                "UnpackedDirectory\\ExportSummary.json");
            Assert.True(Directory.Exists(Path.Combine(actualUnPackedDirectory, "Export\\SSRSMigrate_AW_Tests")), "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests");
            Assert.True(Directory.Exists(Path.Combine(actualUnPackedDirectory, "Export\\SSRSMigrate_AW_Tests\\Data Sources")), "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Data Sources");

            // Verify data source files
            Assert.True(
                File.Exists(Path.Combine(actualUnPackedDirectory,
                    "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json")),
                "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json");
            Assert.True(
                File.Exists(Path.Combine(actualUnPackedDirectory,
                    "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json")),
                "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json");
            Assert.True(Directory.Exists(Path.Combine(actualUnPackedDirectory, "Export\\SSRSMigrate_AW_Tests\\Reports")), "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Reports");
            
            // Verify report files
            Assert.True(
                File.Exists(Path.Combine(actualUnPackedDirectory,
                    "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl")),
                "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl");
            Assert.True(
                File.Exists(Path.Combine(actualUnPackedDirectory,
                    "Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl")),
                "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl");
            Assert.True(
                File.Exists(Path.Combine(actualUnPackedDirectory,
                    "Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl")),
                "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl");
            Assert.True(
                Directory.Exists(Path.Combine(actualUnPackedDirectory,
                    "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder")),
                "UnpackedDirectory\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder");
        }
        #endregion

        #region ReadExportSummary Tests
        /// <summary>
        /// Test reading the ExportSummary.json from the zip archive successfully, where it contains the expected bundle entries.
        /// </summary>
        [Test]
        public void ReadExportSummary()
        {
            bundleReader.ReadExportSummary();

            Assert.AreEqual(2, bundleReader.Summary.Entries["DataSources"].Count);
            Assert.AreEqual(3, bundleReader.Summary.Entries["Reports"].Count);
            Assert.AreEqual(3, bundleReader.Summary.Entries["Folders"].Count);
        }

        /// <summary>
        /// Tests reading the ExportSummary.json from the zip archive where the ExportSummary.json zip entry does not exist.
        /// </summary>
        [Test]
        public void ReadExportSummary_EntryDoesntExist()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), "EntryDoesntExist.zip");

            var actualReader = new ZipBundleReader(
                       zipFile,
                       unpackDir,
                       zipReader,
                       checkSumGen,
                       logger,
                       fileSystem,
                       serializeWrapper);

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                delegate
                {
                    actualReader.ReadExportSummary();
                });

            Assert.That(ex.Message, Is.EqualTo("ExportSummary.json"));

            zipReader = null;
            checkSumGen = null;
            fileSystem = null;
            serializeWrapper = null;
        }

        /// <summary>
        /// Tests reading the ExportSummary.json from the zip archive where the ExportSummary.json contains no bundle entries.
        /// </summary>
        [Test]
        public void ReadExportSummary_EmptySummary()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), "EmptySummaryEntry.zip");

            var actualReader = new ZipBundleReader(
                       zipFile,
                       unpackDir,
                       zipReader,
                       checkSumGen,
                       logger,
                       fileSystem,
                       serializeWrapper);

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    actualReader.ReadExportSummary();
                });

            Assert.That(ex.Message, Is.EqualTo("No data in export summary."));

            zipReader = null;
            checkSumGen = null;
            fileSystem = null;
            serializeWrapper = null;
        }
        #endregion

        #region Read Tests
        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry successfully, where each bundle entry was extracted and exists on disk.
        /// </summary>
        [Test]
        public void Read()
        {
            bundleReader.Extract();
            bundleReader.ReadExportSummary();
            bundleReader.Read();

            Assert.AreEqual(2, actualDataSources.Count);
            Assert.AreEqual(3, actualFolders.Count);
            Assert.AreEqual(3, actualReports.Count);
        }

        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry, where a single report bundle entry was not extracted, 
        /// so it does not exist on disk.
        /// </summary>
        [Test]
        public void Read_FileDoesntExist()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), zipFilename);

            var actualReader = new ZipBundleReader(
                       zipFile,
                       unpackDir,
                       zipReader,
                       checkSumGen,
                       logger,
                       fileSystem,
                       serializeWrapper);

            string actualUnPackDir = actualReader.Extract();

            // This is the file to delete on disk to simulate a report not extracted
            string expectedFailedReportName = Path.Combine(actualUnPackDir, "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl");

            // Delete this file from the extraction directory, for the purpose of testing for a file that does not exist
            File.Delete(expectedFailedReportName);

            // Set expected values
            int expectedSuccessfulReports = 2;
            int expectedFailedReports = 1;
            int actualSuccessfulReports = 0;
            int actualFailedReports = 0;
            string actualFailedReportName = null;

            actualReader.OnReportRead += delegate(IBundleReader sender, ItemReadEvent e)
            {
                if (e.Success)
                    actualSuccessfulReports++;
                else
                {
                    actualFailedReports++;
                    actualFailedReportName = e.FileName;
                }
            };

            actualReader.ReadExportSummary();
            actualReader.Read();

            Assert.AreEqual(expectedSuccessfulReports, actualSuccessfulReports, "Successful Reports");
            Assert.AreEqual(expectedFailedReports, actualFailedReports, "Failed Reports");
            Assert.AreEqual(expectedFailedReportName, actualFailedReportName, "Report Name");
        }

        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry, where a single folder bundle entry was not extracted, 
        /// so it does not exist on disk.
        /// </summary>
        [Test]
        public void Read_DirectoryDoesntExist()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), zipFilename);

            var actualReader = new ZipBundleReader(
                       zipFile,
                       unpackDir,
                       zipReader,
                       checkSumGen,
                       logger,
                       fileSystem,
                       serializeWrapper);

            string actualUnPackDir = actualReader.Extract();

            // This is the directory to delete on disk to simulate a folder not extracted
            string expectedFailedFolderName = Path.Combine(actualUnPackDir, "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder");

            // Delete this directory from the extraction directory, for the purpose of testing for a folder that does not exist
            Directory.Delete(expectedFailedFolderName, true);

            // Set expected values
            int expectedSuccessfulFolders = 2;
            int expectedFailedFolders = 1;
            int actualSuccessfulFolders = 0;
            int actualFailedFolders = 0;
            string actualFailedFoldersName = null;

            actualReader.OnFolderRead += delegate(IBundleReader sender, ItemReadEvent e)
            {
                if (e.Success)
                    actualSuccessfulFolders++;
                else
                {
                    actualFailedFolders++;
                    actualFailedFoldersName = e.FileName;
                }
            };

            actualReader.ReadExportSummary();
            actualReader.Read();

            Assert.AreEqual(expectedSuccessfulFolders, actualSuccessfulFolders, "Successful Folders");
            Assert.AreEqual(expectedFailedFolders, actualFailedFolders, "Failed Folders");
            Assert.AreEqual(expectedFailedFolderName, actualFailedFoldersName, "Folders Name");
        }

        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry, where a single report bundle entry was extracted,
        /// but the checksum for the report on disk does not match the checksum in EntrySummary.json.
        /// </summary>
        [Test]
        public void Read_Report_ChecksumMismatch()
        {
            var logger = new MockLogger();
            var zipReader = kernel.Get<IZipFileReaderWrapper>();
            var checkSumGen = kernel.Get<ICheckSumGenerator>();
            var fileSystem = kernel.Get<IFileSystem>();
            var serializeWrapper = kernel.Get<ISerializeWrapper>();
            string zipFile = Path.Combine(GetTestDataPath(), "InvalidChecksum.zip");

            var actualReader = new ZipBundleReader(
                       zipFile,
                       unpackDir,
                       zipReader,
                       checkSumGen,
                       logger,
                       fileSystem,
                       serializeWrapper);

            string actualUnPackDir = actualReader.Extract();

            // This is the file on disk that has an invalid checksum
            string expectedFailedFilename = Path.Combine(actualUnPackDir, "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl");
            string expectedFailedChecksum = "BAD CHECKSUM HERE";

            // Set expected values
            int expectedSuccessful = 2;
            int expectedFailed = 1;

            // Actual values
            int actualSuccessful = 0;
            int actualFailed = 0;
            string actualFailedFilename = null;
            string actualFailedChecksum = null;
            string actualFailedPath = null;

            actualReader.OnReportRead += delegate(IBundleReader sender, ItemReadEvent e)
            {
                if (e.Success)
                    actualSuccessful++;
                else
                {
                    actualFailed++;
                    actualFailedPath = e.Path;
                    actualFailedFilename = e.FileName;
                    actualFailedChecksum = e.CheckSum;
                }
            };

            actualReader.ReadExportSummary();
            actualReader.Read();

            Assert.AreEqual(expectedSuccessful, actualSuccessful);
            Assert.AreEqual(expectedFailed, actualFailed);
            Assert.AreEqual(expectedFailedChecksum, actualFailedChecksum);
            Assert.AreEqual(expectedFailedFilename, actualFailedFilename);
        }

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
