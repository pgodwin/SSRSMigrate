using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Extensions.Logging.Log4net;
using NUnit.Framework;
using SSRSMigrate.Bundler;
using SSRSMigrate.Bundler.Events;

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

        #region Expected Values
        TestData dataSourceAW = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
            CheckSum = "7b4e44d94590f501ba24cd3904a925c3",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"
        };

        TestData dataSourceTest = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
            CheckSum = "c0815114c3ce9dde35eca314bbfe4bc9",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"
        };

        TestData folderRoot = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests"
        };

        TestData folderDataSources = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources"
        };

        TestData reportsFolder = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData subFolder = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Sub Folder"
        };

        TestData reportCompanySales = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl",
            CheckSum = "1adde7720ca2f0af49550fc676f70804",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData reportSalesOrderDetail = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
            CheckSum = "640a2f60207f03779fdedfed71d8101d",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData reportStoreContacts = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData reportDoesNotExist = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports"
        };

        TestData folderDoesNotExist = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist",
            CheckSum = "",
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"
        };

        TestData summaryFile = new TestData()
        {
            ExtractedTo = "C:\\temp\\SSRSMigrate_AW_Tests\\ExportSummary.json",
            CheckSum = "",
            ZipPath = "ExportSummary.json"
        };

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
        // For testing when a file in the export summary does not exist on disk
        string exportSummaryFileDoesntExist = @"{
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
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Reports"",
      ""FileName"": ""File Doesnt Exist.rdl"",
      ""CheckSum"": ""a225b92ed8475e6bc5b59f5b2cc396fa""
    },
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
    },
  ]
}";

        // For testing when a directory in the export summary does not exist on disk
        string exportSummaryDirectoryDoesntExist = @"{
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
    },
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
    },
    {
      ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"",
      ""FileName"": """",
      ""CheckSum"": """"
    }
  ]
}";
        // For testing when a checksum does not match
        string exportSummaryChecksumMismatch = @"{
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
      ""CheckSum"": ""BAD CHECKSUM HERE""
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
    },
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
    },
  ]
}";
        #endregion

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

        [TestFixtureSetUp]
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

        [TestFixtureTearDown]
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
        //TODO Constructor tests
        #endregion

        #region Extract Tests
        //TODO Extract tests
        [Test]
        public void Extract()
        {
            //string actualUnPackedDirectory = bundleReader.Extract();
        }

        //TODO Extract_NotFound Tests
        #endregion

        #region ReadExportSummary Tests
        //TODO ReadExportSummart tests
        //TODO ReadExportSummary_EntryDoesntExist
        //TODO ReadExportSummary_EmptySummary
        #endregion

        #region Read Tests
        //TODO Read
        //TODO Read_FileDoesntExist
        //TODO Read_DirectoryDoesntExist
        //TODO Read_Report_ChecksumMismatch

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
