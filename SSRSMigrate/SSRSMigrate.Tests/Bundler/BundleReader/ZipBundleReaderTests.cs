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
using SSRSMigrate.Enum;
using SSRSMigrate.Errors;
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
        private Mock<ISerializeWrapper> serializeWrapperMock = null;

        // The path to the zip archive to be read by the tests. This is passed to the ZipBundleReader constructor
        private string zipFileName = "C:\\temp\\SSRSMigrate_AW_Tests.zip";

        // The path to where the zip archive would be extracted to. This is passed to the ZipBundleReader constructor
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
            ZipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder"
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

        BundleSummary bundleSummary = new BundleSummary();
        string bundleSourceRootPath = "/SSRSMigrate_AW_Tests";
        SSRSVersion bundleSourceVersion = SSRSVersion.SqlServer2008R2;

        Dictionary<string, List<BundleSummaryEntry>> entries = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
		        {
		            "DataSources", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "AWDataSource.json",
                            CheckSum = "7b4e44d94590f501ba24cd3904a925c3"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "Test Data Source.json",
                            CheckSum = "81a151f4b17aba7e1516d0801cadf4ee"
		                }
		            }
		        },
		        {
		            "Reports", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Company Sales.rdl",
                            CheckSum = "1adde7720ca2f0af49550fc676f70804"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Sales Order Detail.rdl",
                            CheckSum = "640a2f60207f03779fdedfed71d8101d"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Store Contacts.rdl",
                            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa"
		                },
		            }
		        },
		        {
		            "Folders", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
                            FileName = "",
                            CheckSum = ""
		                }
		            }
		        }
		    };

        string exportSummary = @"{
  ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
  ""SourceVersion"": ""SqlServer2008R2"",
  ""Entries"": {
    ""DataSources"": [
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
        ""FileName"": ""AWDataSource.json"",
        ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
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
        ""Path"": ""Export\\SSRSMigrate_AW_Tests"",
        ""FileName"": """",
        ""CheckSum"": """"
      },
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
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
        ""FileName"": """",
        ""CheckSum"": """"
      }
    ]
  }
}";

        // For testing when a file in the export summary does not exist on disk
        BundleSummary bundleSummaryFileDoesntExist = new BundleSummary();

        Dictionary<string, List<BundleSummaryEntry>> entriesFileDoesntExist = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
		        {
		            "DataSources", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "AWDataSource.json",
                            CheckSum = "7b4e44d94590f501ba24cd3904a925c3"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "Test Data Source.json",
                            CheckSum = "81a151f4b17aba7e1516d0801cadf4ee"
		                }
		            }
		        },
		        {
		            "Reports", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Company Sales.rdl",
                            CheckSum = "1adde7720ca2f0af49550fc676f70804"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Sales Order Detail.rdl",
                            CheckSum = "640a2f60207f03779fdedfed71d8101d"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Store Contacts.rdl",
                            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa"
		                },
                        new BundleSummaryEntry()
                        {
                            Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "File Doesnt Exist.rdl",
                            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa"
                        }
		            }
		        },
		        {
		            "Folders", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
                            FileName = "",
                            CheckSum = ""
		                }
		            }
		        }
		    };

        string exportSummaryFileDoesntExist = @"{
  ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
  ""SourceVersion"": ""SqlServer2008R2"",
  ""Entries"": {
    ""DataSources"": [
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
        ""FileName"": ""AWDataSource.json"",
        ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
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
      }
    ],
    ""Folders"": [
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests"",
        ""FileName"": """",
        ""CheckSum"": """"
      },
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
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
        ""FileName"": """",
        ""CheckSum"": """"
      }
    ]
  }
}";
        
        // For testing when a directory in the export summary does not exist on disk
        BundleSummary bundleSummaryDirectoryDoesntExist = new BundleSummary();

        Dictionary<string, List<BundleSummaryEntry>> entriesDirectoryDoesntExist = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
		        {
		            "DataSources", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "AWDataSource.json",
                            CheckSum = "7b4e44d94590f501ba24cd3904a925c3"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "Test Data Source.json",
                            CheckSum = "81a151f4b17aba7e1516d0801cadf4ee"
		                }
		            }
		        },
		        {
		            "Reports", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Company Sales.rdl",
                            CheckSum = "1adde7720ca2f0af49550fc676f70804"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Sales Order Detail.rdl",
                            CheckSum = "640a2f60207f03779fdedfed71d8101d"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Store Contacts.rdl",
                            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa"
		                },
		            }
		        },
		        {
		            "Folders", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
                        {
                            Path = "Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist",
                            FileName = "",
                            CheckSum = ""
                        }
		            }
		        }
		    };
        string exportSummaryDirectoryDoesntExist = @"{
  ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
  ""SourceVersion"": ""SqlServer2008R2"",
  ""Entries"": {
    ""DataSources"": [
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
        ""FileName"": ""AWDataSource.json"",
        ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
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
        ""Path"": ""Export\\SSRSMigrate_AW_Tests"",
        ""FileName"": """",
        ""CheckSum"": """"
      },
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
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
        ""FileName"": """",
        ""CheckSum"": """"
      },
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist"",
        ""FileName"": """",
        ""CheckSum"": """"
      }
    ]
  }
}";

        // For testing when a checksum does not match
        BundleSummary bundleSummaryChecksumMismatch = new BundleSummary();

        Dictionary<string, List<BundleSummaryEntry>> entriesChecksumMismatch = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
		        {
		            "DataSources", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "AWDataSource.json",
                            CheckSum = "7b4e44d94590f501ba24cd3904a925c3"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "Test Data Source.json",
                            CheckSum = "81a151f4b17aba7e1516d0801cadf4ee"
		                }
		            }
		        },
		        {
		            "Reports", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Company Sales.rdl",
                            CheckSum = "BAD CHECKSUM HERE"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Sales Order Detail.rdl",
                            CheckSum = "640a2f60207f03779fdedfed71d8101d"
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "Store Contacts.rdl",
                            CheckSum = "a225b92ed8475e6bc5b59f5b2cc396fa"
		                },
		            }
		        },
		        {
		            "Folders", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports",
                            FileName = "",
                            CheckSum = ""
		                },
                        new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Reports\\Sub Folder",
                            FileName = "",
                            CheckSum = ""
		                }
		            }
		        }
		    };

        string exportSummaryChecksumMismatch = @"{
  ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
  ""SourceVersion"": ""SqlServer2008R2"",
  ""Entries"": {
    ""DataSources"": [
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Data Sources"",
        ""FileName"": ""AWDataSource.json"",
        ""CheckSum"": ""7b4e44d94590f501ba24cd3904a925c3""
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
      }
    ],
    ""Folders"": [
      {
        ""Path"": ""Export\\SSRSMigrate_AW_Tests"",
        ""FileName"": """",
        ""CheckSum"": """"
      },
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
        ""Path"": ""Export\\SSRSMigrate_AW_Tests\\Sub Folder"",
        ""FileName"": """",
        ""CheckSum"": """"
      }
    ]
  }
}";
        #endregion

        #region Actual Values
        // These hold the paths to the items read from the ExportSummary
        private List<string> actualReports = null;
        private List<string> actualFolders = null;
        private List<string> actualDataSources = null; 
        #endregion

        #region Value Setup
        /// <summary>
        /// Setups the test values used in the OnEntryExtracted event
        /// </summary>
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

            bundleSummary.SourceRootPath = bundleSourceRootPath;
            bundleSummary.SourceVersion = bundleSourceVersion;
            bundleSummary.Entries = entries;

            bundleSummaryFileDoesntExist.SourceRootPath = bundleSourceRootPath;
            bundleSummaryFileDoesntExist.SourceVersion = bundleSourceVersion;
            bundleSummaryFileDoesntExist.Entries = entriesFileDoesntExist;

            bundleSummaryDirectoryDoesntExist.SourceRootPath = bundleSourceRootPath;
            bundleSummaryDirectoryDoesntExist.SourceVersion = bundleSourceVersion;
            bundleSummaryDirectoryDoesntExist.Entries = entriesDirectoryDoesntExist;

            bundleSummaryChecksumMismatch.SourceRootPath = bundleSourceRootPath;
            bundleSummaryChecksumMismatch.SourceVersion = bundleSourceVersion;
            bundleSummaryChecksumMismatch.Entries = entriesChecksumMismatch;
        }
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            this.SetupTestValues();

            zipReaderMock = new Mock<IZipFileReaderWrapper>();
            checkSumGenMock = new Mock<ICheckSumGenerator>();
            fileSystemMock = new Mock<IFileSystem>();
            serializeWrapperMock = new Mock<ISerializeWrapper>();

            //TODO: Move mock setups to each test that actually uses them.

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
                .Throws(new InvalidFileArchiveException("NotAZip.txt"));

            // IZipFileReaderWrapper.ReadEntry Method Mocks
            zipReaderMock.Setup(z => z.ReadEntry("ExportSummary.json"))
                .Returns(() => exportSummary);
            
            // ICheckSumGenerator.CreateCheckSum Method Mocks
            checkSumGenMock.Setup(c => c.CreateCheckSum(dataSourceAW.ExtractedTo))
                .Returns(() => "7b4e44d94590f501ba24cd3904a925c3");

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

            // ISerializeWrapper.DeserializeObject Mocks
            serializeWrapperMock.Setup(s => s.DeserializeObject<BundleSummary>(exportSummary))
                .Returns(() => bundleSummary);
        }

        [OneTimeTearDown]
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
                fileSystemMock.Object,
                serializeWrapperMock.Object
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

        #region Constructor Tests
        /// <summary>
        /// Test the contructor when passed all valid parameters.
        /// </summary>
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
                fileSystemMock.Object,
                serializeWrapperMock.Object);

            Assert.NotNull(actual);
        }

        /// <summary>
        /// Test the constructor when passed a filename parameter that is not a zip archive.
        /// </summary>
        [Test]
        public void Constructor_InvalidFileName()
        {
            InvalidFileArchiveException ex = Assert.Throws<InvalidFileArchiveException>(
                delegate
                {
                    var logger = new MockLogger();

                    var reader = new ZipBundleReader(
                                "NotAZip.txt",
                                unPackDirectory,
                                zipReaderMock.Object,
                                checkSumGenMock.Object,
                                logger,
                                fileSystemMock.Object,
                                serializeWrapperMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("'NotAZip.txt' is not a valid archive."));
        }
        #endregion

        #region Extract Tests
        /// <summary>
        /// Test extracting the zip archive successfully.
        /// </summary>
        [Test]
        public void Extract()
        {
            string actualUnPackedDirectory = zipBundleReader.Extract();

            zipReaderMock.Verify(z => z.UnPack());

            Assert.AreEqual(unPackDirectory, actualUnPackedDirectory);
        }
        #endregion
        
        #region ReadExportSummary Tests
        /// <summary>
        /// Test reading the ExportSummary.json from the zip archive successfully, where it contains the expected bundle entries.
        /// </summary>
        [Test]
        public void ReadExportSummary()
        {
            zipBundleReader.ReadExportSummary();

            zipReaderMock.Verify(z => z.ReadEntry("ExportSummary.json"));

            Assert.AreEqual(2, zipBundleReader.Summary.Entries["DataSources"].Count);
            Assert.AreEqual(3, zipBundleReader.Summary.Entries["Reports"].Count);
            Assert.AreEqual(4, zipBundleReader.Summary.Entries["Folders"].Count);
        }

        /// <summary>
        /// Tests reading the ExportSummary.json from the zip archive where the ExportSummary.json zip entry does not exist.
        /// </summary>
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
                        fileSystemMock.Object,
                        serializeWrapperMock.Object);

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                delegate
                {
                    reader.ReadExportSummary();
                });

            Assert.That(ex.Message, Is.EqualTo(entryName));
        }

        /// <summary>
        /// Tests reading the ExportSummary.json from the zip archive where the ExportSummary.json contains no bundle entries.
        /// </summary>
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
                        fileSystemMock.Object,
                        serializeWrapperMock.Object);

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    reader.ReadExportSummary();
                });

            Assert.That(ex.Message, Is.EqualTo("No data in export summary."));
            Assert.AreEqual(0, reader.Summary.Entries["DataSources"].Count);
            Assert.AreEqual(0, reader.Summary.Entries["Reports"].Count);
            Assert.AreEqual(0, reader.Summary.Entries["Folders"].Count);
        }
        #endregion

        #region Read Tests
        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry successfully, where each bundle entry was extracted and exists on disk.
        /// </summary>
        [Test]
        public void Read()
        {
            zipBundleReader.ReadExportSummary();

            zipBundleReader.Read();

            Assert.AreEqual(2, actualDataSources.Count);
            Assert.AreEqual(4, actualFolders.Count);
            Assert.AreEqual(3, actualReports.Count);
        }

        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry, where a single report bundle entry was not extracted, 
        /// so it does not exist on disk.
        /// </summary>
        [Test]
        public void Read_FileDoesntExist()
        {
            string entryName = "ExportSummary.json";
            int expectedSuccessfulReports = 3;
            int expectedFailedReports = 1;
            string expectedFailedReportName = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl";
            int actualSuccessfulReports = 0;
            int actualFailedReports = 0;
            string actualFailedReportName = null;

            var readerMock = new Mock<IZipFileReaderWrapper>();
            var serializeMock = new Mock<ISerializeWrapper>();

            readerMock.Setup(z => z.ReadEntry(entryName))
                .Returns(() => exportSummaryFileDoesntExist);

            serializeMock.Setup(
                s => s.DeserializeObject<BundleSummary>(exportSummaryFileDoesntExist))
                .Returns(() => bundleSummaryFileDoesntExist);

            var logger = new MockLogger();

            var reader = new ZipBundleReader(
                        zipFileName,
                        unPackDirectory,
                        readerMock.Object,
                        checkSumGenMock.Object,
                        logger,
                        fileSystemMock.Object,
                        serializeMock.Object);

            reader.ReadExportSummary();

            reader.OnReportRead += delegate(IBundleReader sender, ItemReadEvent e)
            {
                if (e.Success)
                    actualSuccessfulReports++;
                else
                {
                    actualFailedReports++;
                    actualFailedReportName = e.FileName;
                }
            };

            reader.Read();

            Assert.AreEqual(expectedSuccessfulReports, actualSuccessfulReports);
            Assert.AreEqual(expectedFailedReports, actualFailedReports);
            Assert.AreEqual(expectedFailedReportName, actualFailedReportName);
        }

        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry, where a single folder bundle entry was not extracted, 
        /// so it does not exist on disk.
        /// </summary>
        [Test]
        public void Read_DirectoryDoesntExist()
        {
            string entryName = "ExportSummary.json";
            int expectedSuccessfulDirectories = 4;
            int expectedFailedDirectories = 1;
            string expectedFailedDirectoryName = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Folder Doesnt Exist";
            int actualSuccessfulDirectories = 0;
            int actualFailedDirectories = 0;
            string actualFailedDirectoryName = null;

            var readerMock = new Mock<IZipFileReaderWrapper>();
            var serializeMock = new Mock<ISerializeWrapper>();

            readerMock.Setup(z => z.ReadEntry(entryName))
                .Returns(() => exportSummaryDirectoryDoesntExist);

            serializeMock.Setup(
                s => s.DeserializeObject<BundleSummary>(exportSummaryDirectoryDoesntExist))
                .Returns(() => bundleSummaryDirectoryDoesntExist);

            var logger = new MockLogger();

            var reader = new ZipBundleReader(
                        zipFileName,
                        unPackDirectory,
                        readerMock.Object,
                        checkSumGenMock.Object,
                        logger,
                        fileSystemMock.Object,
                        serializeMock.Object);

            reader.ReadExportSummary();

            reader.OnFolderRead += delegate(IBundleReader sender, ItemReadEvent e)
            {
                if (e.Success)
                    actualSuccessfulDirectories++;
                else
                {
                    actualFailedDirectories++;
                    actualFailedDirectoryName = e.FileName;
                }
            };

            reader.Read();

            Assert.AreEqual(expectedSuccessfulDirectories, actualSuccessfulDirectories);
            Assert.AreEqual(expectedFailedDirectories, actualFailedDirectories);
            Assert.AreEqual(expectedFailedDirectoryName, actualFailedDirectoryName);
        }

        /// <summary>
        /// Tests reading the ExportSummary.json and reading each bundle entry, where a single report bundle entry was extracted,
        /// but the checksum for the report on disk does not match the checksum in EntrySummary.json.
        /// </summary>
        [Test]
        public void Read_Report_ChecksumMismatch()
        {
            string entryName = "ExportSummary.json";
            int expectedSuccessful = 2;
            int expectedFailed = 1;
            string expectedFailedFilename = "C:\\temp\\Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl";
            string expectedFailedPath = "Export\\SSRSMigrate_AW_Tests\\Reports";
            string expectedFailedChecksum = "BAD CHECKSUM HERE";
            int actualSuccessful = 0;
            int actualFailed = 0;
            string actualFailedChecksum = null;
            string actualFailedFilename = null;
            string actualFailedPath = null;

            var readerMock = new Mock<IZipFileReaderWrapper>();
            var serializeMock = new Mock<ISerializeWrapper>();

            readerMock.Setup(z => z.ReadEntry(entryName))
                .Returns(() => exportSummaryChecksumMismatch);

            serializeMock.Setup(
                s => s.DeserializeObject<BundleSummary>(exportSummaryChecksumMismatch))
                .Returns(() => bundleSummaryChecksumMismatch);

            var logger = new MockLogger();

            var reader = new ZipBundleReader(
                        zipFileName,
                        unPackDirectory,
                        readerMock.Object,
                        checkSumGenMock.Object,
                        logger,
                        fileSystemMock.Object,
                        serializeMock.Object);

            reader.ReadExportSummary();

            reader.OnReportRead += delegate(IBundleReader sender, ItemReadEvent e)
            {
                if (e.Success)
                    actualSuccessful++;
                else
                {
                    actualFailed++;
                    actualFailedPath = e.Path;
                    actualFailedChecksum = e.CheckSum;
                    actualFailedFilename = e.FileName;
                }
            };

            reader.Read();

            Assert.AreEqual(expectedSuccessful, actualSuccessful);
            Assert.AreEqual(expectedFailed, actualFailed);
            Assert.AreEqual(expectedFailedChecksum, actualFailedChecksum);
            Assert.AreEqual(expectedFailedFilename, actualFailedFilename);
            Assert.AreEqual(expectedFailedPath, actualFailedPath);
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
