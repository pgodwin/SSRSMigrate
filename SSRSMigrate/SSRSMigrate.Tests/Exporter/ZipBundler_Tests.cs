using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Bundler;
using SSRSMigrate.Enum;
using SSRSMigrate.Exporter;
using Moq;
using SSRSMigrate.TestHelper.Logging;
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
        Mock<ISerializeWrapper> serializeWrapperMock = null;

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
        #endregion

        #region Expected Values
        private BundleSummary expectedBundleSummary = new BundleSummary();
        private string expectedBundleSourceRootPath = "/SSRSMigrate_AW_Tests";
        private SSRSVersion expectedSourceVersion = SSRSVersion.SqlServer2008R2;
        Dictionary<string, List<BundleSummaryEntry>> expectedEntries = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
		        {
		            "DataSources", new List<BundleSummaryEntry>()
		            {
		                new BundleSummaryEntry()
		                {
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                            FileName = "AWDataSource.json",
                            CheckSum = "7b4e44d94590f501ba24cd3904a925c3"
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
		                    Path = "Export\\SSRSMigrate_AW_Tests\\Sub Folder",
                            FileName = "",
                            CheckSum = ""
		                }
		            }
		        }
		    };

        string expectedSummary = @"{
  ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
  ""SourceVersion"": ""SqlServer2008R2"",
  ""Entries"": {
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

        private BundleSummary expectedBundleSummaryNoEntries = new BundleSummary();
        Dictionary<string, List<BundleSummaryEntry>> expectedEntriesNoData = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
			    { "DataSources", new List<BundleSummaryEntry>() },
			    { "Reports", new List<BundleSummaryEntry>() },
			    { "Folders", new List<BundleSummaryEntry>() }
		    };

        string expectedSummaryNoData = @"{
  ""SourceRootPath"": ""/SSRSMigrate_AW_Tests"",
  ""SourceVersion"": ""SqlServer2008R2"",
  ""Entries"": {
    ""DataSources"": [],
    ""Reports"": [],
    ""Folders"": []
  }
}";
        #endregion

        #region Setup Values
        private void SetUpValues()
        {
            expectedBundleSummary.SourceRootPath = expectedBundleSourceRootPath;
            expectedBundleSummary.SourceVersion = expectedSourceVersion;
            expectedBundleSummary.Entries = expectedEntries;

            expectedBundleSummaryNoEntries.SourceRootPath = expectedBundleSourceRootPath;
            expectedBundleSummaryNoEntries.SourceVersion = expectedSourceVersion;
            expectedBundleSummaryNoEntries.Entries = expectedEntriesNoData;
        }
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            this.SetUpValues();

            zipFileMock = new Mock<IZipFileWrapper>();
            checkSumGenMock = new Mock<ICheckSumGenerator>();
            serializeWrapperMock = new Mock<ISerializeWrapper>();

            //TODO: Move mock setups to each test that actually uses them.

            // IZipFileWrapper.AddDirectory Mocks
            zipFileMock.Setup(z => z.AddDirectory(It.IsAny<string>()));
            
            zipFileMock.Setup(z => z.AddDirectory(It.IsAny<string>(), It.IsAny<string>()));
            
            zipFileMock.Setup(z => z.AddDirectory(folderDesNotExistReport.FileName, folderDesNotExistReport.ZipPath))
                .Throws(new DirectoryNotFoundException(folderDesNotExistReport.FileName));
            
            zipFileMock.Setup(z => z.AddDirectory(doesNotExistReport.FileName, doesNotExistReport.ZipPath))
                .Throws(new DirectoryNotFoundException(doesNotExistReport.FileName));
            
            zipFileMock.Setup(z => z.AddDirectory(folderDesNotExistReport.FileName, folderDesNotExistReport.ZipPath))
                .Throws(new FileNotFoundException(folderDesNotExistReport.FileName));
            
            // Mock passing a file to AddDirectory
            zipFileMock.Setup(z => z.AddDirectory(awDataSource.FileName, "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"))
                .Throws(new DirectoryNotFoundException(awDataSource.FileName));

            zipFileMock.Setup(z => z.AddDirectory(companySalesReport.FileName, "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl"))
                .Throws(new DirectoryNotFoundException(companySalesReport.FileName));

            // IZipFileWrapper.AddEntry Mocks
            zipFileMock.Setup(z => z.AddEntry(It.IsAny<string>(), It.IsAny<string>()));

            // IZipFileWrapper.AddFile Mocks
            zipFileMock.Setup(z => z.AddFile(It.IsAny<string>()));
            
            zipFileMock.Setup(z => z.AddFile(It.IsAny<string>(), It.IsAny<string>()));
            
            zipFileMock.Setup(z => z.AddFile(doesNotExistReport.FileName, doesNotExistReport.ZipPath))
                .Throws(new FileNotFoundException(doesNotExistReport.FileName));

            // Mock passing a directory to AddFile
            zipFileMock.Setup(z => z.AddFile(rootFolder.FileName, "Export"))
                .Throws(new FileNotFoundException(rootFolder.FileName));

            // IZipFileWrapper.Dispose Mocks
            zipFileMock.Setup(z => z.Dispose());

            // IZipFileWrapper.Save Mocks
            zipFileMock.Setup(z => z.Save(It.IsAny<string>()));

            // ICheckSumGenerator.CreateCheckSum Mocks
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

            // ISerializeWrapper.Serialize Mocks
            //serializeWrapperMock.Setup(s => s.SerializeObject(It.Is<Dictionary<string, List<BundleSummaryEntry>>>(
            //    p => p["DataSources"].Count > 0 &&
            //            p["Folders"].Count > 0 &&
            //            p["Reports"].Count > 0
            //    )))
            //    .Returns(() => expectedSummary);

            // Mock for ISerializeWrapper.Serialize where the entry being serialized contains no data
            //serializeWrapperMock.Setup(s => s.SerializeObject(It.Is<Dictionary<string, List<BundleSummaryEntry>>>(
            //    p =>    p["DataSources"].Count == 0 &&
            //            p["Folders"].Count == 0 &&
            //            p["Reports"].Count == 0
            //    )))
            //    .Returns(() => expectedSummaryNoData);

            serializeWrapperMock.Setup(s => s.SerializeObject(It.Is<BundleSummary>(
                p => p.Entries["DataSources"].Count > 0 &&
                        p.Entries["Folders"].Count > 0 &&
                        p.Entries["Reports"].Count > 0
                )))
                .Returns(() => expectedSummary);

            // Mock for ISerializeWrapper.Serialize where the entry being serialized contains no data
            serializeWrapperMock.Setup(s => s.SerializeObject(It.Is<BundleSummary>(
                p => p.Entries["DataSources"].Count == 0 &&
                        p.Entries["Folders"].Count == 0 &&
                        p.Entries["Reports"].Count == 0
                )))
                .Returns(() => expectedSummaryNoData);
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            zipBundler = null;
        }

        [SetUp]
        public void SetUp()
        {
            // Recreate ZipBundler for each test, so the CreateSummary tests have fresh data
            MockLogger logger = new MockLogger();

            zipBundler = new ZipBundler(zipFileMock.Object, checkSumGenMock.Object, logger, serializeWrapperMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            zipBundler = null;
        }

        #region Constructor Tests
        [Test]
        public void Constructor()
        {
            MockLogger logger = new MockLogger();

            ZipBundler actual = new ZipBundler(zipFileMock.Object, checkSumGenMock.Object, logger, serializeWrapperMock.Object);

            Assert.NotNull(actual);
        }

        [Test]
        public void Constructor_Null_ZipFile()
        {
            MockLogger logger = new MockLogger();
            Mock<ICheckSumGenerator> checkSumMock = null;
            checkSumMock = new Mock<ICheckSumGenerator>();

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    new ZipBundler(null, checkSumMock.Object, logger, serializeWrapperMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: zipFileWrapper"));
        }

        [Test]
        public void Constructor_Null_CheckSumGenFile()
        {
            MockLogger logger = new MockLogger();
            Mock<IZipFileWrapper> zipMock = null;
            zipMock = new Mock<IZipFileWrapper>();

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    new ZipBundler(zipMock.Object, null, logger, serializeWrapperMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: checkSumGenerator"));
        }
        #endregion

        #region Entries Property Tests
        [Test]
        public void Entries_ContainsKeys()
        {
            Assert.NotNull(zipBundler.Summary.Entries);
            Assert.True(zipBundler.Summary.Entries.ContainsKey("Reports"));
            Assert.True(zipBundler.Summary.Entries.ContainsKey("DataSources"));
            Assert.True(zipBundler.Summary.Entries.ContainsKey("Folders"));
        }
        #endregion

        #region ExportSummaryFilename Tests
        [Test]
        public void ExportSummaryFilename()
        {
            string actual = zipBundler.ExportSummaryFilename;

            Assert.AreEqual("ExportSummary.json", actual);
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
            string expectedZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources";
            string expectedFileName = "AWDataSource.json";
            string expectedCheckSum = awDataSource.CheckSum;

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, expectedZipPath);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedCheckSum, actual.CheckSum);
            Assert.AreEqual(expectedZipPath, actual.Path);
            Assert.AreEqual(expectedFileName, actual.FileName);
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

        #region AddItem DataSource Tests
        [Test]
        public void AddItem_DataSource()
        {
            zipBundler.AddItem("DataSources",
                awDataSource.FileName,
                awDataSource.Path,
                false);

            // Verify that the file was added to the zip
            zipFileMock.Verify(z => z.AddFile(awDataSource.FileName, awDataSource.ZipPath));
        }

        [Test]
        public void AddItem_DataSource_KeyDoesntExist()
        {
            KeyNotFoundException ex = Assert.Throws<KeyNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("DoesntExist",
                        awDataSource.FileName,
                        awDataSource.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("DoesntExist"));
        }

        [Test]
        public void AddItem_DataSource_NullKey()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem(null,
                        awDataSource.FileName,
                        awDataSource.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("key"));
        }

        [Test]
        public void AddItem_DataSource_EmptyKey()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("",
                        awDataSource.FileName,
                        awDataSource.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("key"));
        }

        [Test]
        public void AddItem_DataSource_NullItemFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("DataSources",
                        null,
                        awDataSource.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void AddItem_DataSource_EmptyItemFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("DataSources",
                        "",
                        awDataSource.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void AddItem_DataSource_NullItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("DataSources",
                        awDataSource.FileName,
                        null,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void AddItem_DataSource_EmptyItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("DataSources",
                        awDataSource.FileName,
                        "",
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void AddItem_DataSource_InvalidItemPath()
        {
            string itemFileName = awDataSource.FileName;
            string itemPath = "/SSRSMigrate/Data Sources/AWDataSource"; // This path is not contained within awDataSource.FileName, so it is invalid

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    zipBundler.AddItem("DataSources",
                        itemFileName,
                        itemPath,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Item path '{0}' is invalid.", itemPath)));
        }

        /// <summary>
        /// Tests AddItem by passing it a file but with the isFolder boolean value of True
        /// </summary>
        [Test]
        public void AddItem_DataSource_FileAsDirectory()
        {
            DirectoryNotFoundException ex = Assert.Throws<DirectoryNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("DataSources",
                        awDataSource.FileName,
                        awDataSource.Path,
                        true); // Add to zip as directory
                });

            Assert.That(ex.Message, Is.EqualTo(awDataSource.FileName));
        }
        #endregion

        #region AddItem Folder Tests
        [Test]
        public void AddItem_Folder()
        {
            zipBundler.AddItem("Folders",
                rootFolder.FileName,
                rootFolder.Path,
                true);

            // Verify that the folder was added to the zip
            zipFileMock.Verify(z => z.AddDirectory(rootFolder.FileName, rootFolder.ZipPath));
        }

        [Test]
        public void AddItem_Folder_KeyDoesntExist()
        {
            KeyNotFoundException ex = Assert.Throws<KeyNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("DoesntExist",
                        rootFolder.FileName,
                        rootFolder.Path,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("DoesntExist"));
        }

        [Test]
        public void AddItem_Folder_NullKey()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem(null,
                        rootFolder.FileName,
                        rootFolder.Path,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("key"));
        }

        [Test]
        public void AddItem_Folder_EmptyKey()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("",
                        rootFolder.FileName,
                        rootFolder.Path,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("key"));
        }

        [Test]
        public void AddItem_Folder_NullItemFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        null,
                        rootFolder.Path,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void AddItem_Folder_EmptyItemFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        "",
                        rootFolder.Path,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void AddItem_Folder_NullItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        rootFolder.FileName,
                        null,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void AddItem_Folder_EmptyItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        rootFolder.FileName,
                        "",
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void AddItem_Folder_InvalidItemPath()
        {
            string itemFileName = rootFolder.FileName;
            string itemPath = "/SSRSMigrate_INVALID"; // This path is not contained within rootFolder.FileName, so it is invalid

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        itemFileName,
                        itemPath,
                        true);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Item path '{0}' is invalid.", itemPath)));
        }

        /// <summary>
        /// Tests AddItem by passing it a file but with the isFolder boolean value of True
        /// </summary>
        [Test]
        public void AddItem_Folder_FileAsDirectory()
        {
            DirectoryNotFoundException ex = Assert.Throws<DirectoryNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        awDataSource.FileName,
                        awDataSource.Path,
                        true); // Add to zip as directory
                });

            Assert.That(ex.Message, Is.EqualTo(awDataSource.FileName));
        }
        #endregion

        #region AddItem Report Tests
        [Test]
        public void AddItem_Report()
        {
            zipBundler.AddItem("Reports",
                companySalesReport.FileName,
                companySalesReport.Path,
                false);

            // Verify that the file was added to the zip
            zipFileMock.Verify(z => z.AddFile(companySalesReport.FileName, companySalesReport.ZipPath));
        }

        [Test]
        public void AddItem_Report_KeyDoesntExist()
        {
            KeyNotFoundException ex = Assert.Throws<KeyNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("DoesntExist",
                        companySalesReport.FileName,
                        companySalesReport.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("DoesntExist"));
        }

        [Test]
        public void AddItem_Report_NullKey()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem(null,
                        companySalesReport.FileName,
                        companySalesReport.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("key"));
        }

        [Test]
        public void AddItem_Report_EmptyKey()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("",
                        companySalesReport.FileName,
                        companySalesReport.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("key"));
        }

        [Test]
        public void AddItem_Report_NullItemFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        null,
                        companySalesReport.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void AddItem_Report_EmptyItemFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        "",
                        companySalesReport.Path,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemFileName"));
        }

        [Test]
        public void AddItem_Report_NullItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        companySalesReport.FileName,
                        null,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void AddItem_Report_EmptyItemPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        companySalesReport.FileName,
                        "",
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo("itemPath"));
        }

        [Test]
        public void AddItem_Report_InvalidItemPath()
        {
            string itemFileName = companySalesReport.FileName;
            string itemPath = "/SSRSMigrate/Reports/Company Sales"; // This path is not contained within companySalesReport.FileName, so it is invalid

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        itemFileName,
                        itemPath,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Item path '{0}' is invalid.", itemPath)));
        }

        /// <summary>
        /// Tests AddItem by passing it a file but with the isFolder boolean value of True
        /// </summary>
        [Test]
        public void AddItem_Report_FileAsDirectory()
        {
            DirectoryNotFoundException ex = Assert.Throws<DirectoryNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        companySalesReport.FileName,
                        companySalesReport.Path,
                        true); // Add to zip as directory
                });

            Assert.That(ex.Message, Is.EqualTo(companySalesReport.FileName));
        }
        #endregion

        #region AddItem Directory as File
        /// <summary>
        /// Tests AddItem by passing it a directory but with isFolder boolean value of False
        /// </summary>
        [Test]
        public void AddItem_DirectoryAsFile()
        {
            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                delegate
                {
                    zipBundler.AddItem("Reports",
                        rootFolder.FileName,
                        rootFolder.Path,
                        false); // Add to zip as file
                });

            Assert.That(ex.Message, Is.EqualTo(rootFolder.FileName));
        }
        #endregion

        #region CreateSummary Tests
        [Test]
        public void CreateSummary()
        {
            // Add test data to ZipBundler
            // Add Data Source item to ZipBundler
            zipBundler.AddItem("DataSources",
                awDataSource.FileName,
                awDataSource.Path,
                false);

            // Verify that the data source file was added to the zip
            zipFileMock.Verify(z => z.AddFile(awDataSource.FileName, awDataSource.ZipPath));

            // Add Folder item to ZipBundler
            zipBundler.AddItem("Folders",
                rootFolder.FileName,
                rootFolder.Path,
                true);

            // Verify that the folder was added to the zip
            zipFileMock.Verify(z => z.AddDirectory(rootFolder.FileName, rootFolder.ZipPath));

            // Add Report items to ZipBundler
            // Add Comapny Sales report
            zipBundler.AddItem("Reports",
                companySalesReport.FileName,
                companySalesReport.Path,
                false);

            // Verify that the file was added to the zip
            zipFileMock.Verify(z => z.AddFile(companySalesReport.FileName, companySalesReport.ZipPath));

            // Add Sales Order Detail report
            zipBundler.AddItem("Reports",
                salesOrderDetailReport.FileName,
                salesOrderDetailReport.Path,
                false);

            // Verify that the file was added to the zip
            zipFileMock.Verify(z => z.AddFile(salesOrderDetailReport.FileName, salesOrderDetailReport.ZipPath));

            // Add Store Contacts [sub] report
            zipBundler.AddItem("Reports",
               storeContactsReport.FileName,
               storeContactsReport.Path,
               false);

            // Verify that the file was added to the zip
            zipFileMock.Verify(z => z.AddFile(storeContactsReport.FileName, storeContactsReport.ZipPath));

            string actual = zipBundler.CreateSummary(expectedBundleSourceRootPath, expectedSourceVersion);

            // Verify that the summary was added as an entry to the zip
            zipFileMock.Verify(z => z.AddEntry("ExportSummary.json", expectedSummary));

            Assert.AreEqual(expectedSummary, actual);
        }

        [Test]
        public void CreateSummary_NoData()
        {
            string actual = zipBundler.CreateSummary(expectedBundleSourceRootPath, expectedSourceVersion);

            // Verify that the summary was added as an entry to the zip
            zipFileMock.Verify(z => z.AddEntry("ExportSummary.json", expectedSummaryNoData));

            Assert.AreEqual(expectedSummaryNoData, actual);
        }
        #endregion
        
        #region Save Tests
        [Test]
        public void Save()
        {
            string filename = "C:\\temp\\SSRSMigrate_AW_Tests.zip";

            zipBundler.Save(filename);

            zipFileMock.Verify(z => z.Save(filename, true));
        }

        [Test]
        public void Save_NullFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.Save(null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void Save_EmptyFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    zipBundler.Save("");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }
        #endregion
    }
}
