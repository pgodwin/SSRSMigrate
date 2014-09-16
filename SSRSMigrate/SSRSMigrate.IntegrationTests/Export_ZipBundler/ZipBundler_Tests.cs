using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;
using SSRSMigrate.Wrappers;
using SSRSMigrate.Exporter;

namespace SSRSMigrate.IntegrationTests.Export_ZipBundler
{
    #region Test Structures
    // Holds data for test methods and mocks
    public struct TestData
    {
        public string FileName { get; set; }
        public string Path { get; set; }
    };
    #endregion

    [TestFixture]
    [CoverageExcludeAttribute]
    class ZipBundler_Tests
    {
        private ZipFileWrapper zipFileWrapper = null;
        private MD5CheckSumGenerator checkSumGenerator = null;
        private ZipBundler zipBundler = null;

        private string zipArchiveFilename = Path.Combine(GetOutPutPath(), "SSRSMigrate_AW_Tests.zip");

        #region Test Data
        TestData awDataSource = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"),
            Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource"
        };

        TestData testDataSource = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json"),
            Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source"
        };

        TestData rootFolder = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests"),
            Path = "/SSRSMigrate_AW_Tests"
        };

        TestData dataSourcesFolder = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Data Sources"),
            Path = "/SSRSMigrate_AW_Tests/Data Sources"
        };

        TestData reportsFolder = new TestData()
        {
            FileName =Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Reports"),
            Path = "/SSRSMigrate_AW_Tests/Reports"
        };

        TestData subFolder = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Sub Folder"),
            Path = "/SSRSMigrate_AW_Tests/Sub Folder"
        };

        TestData companySalesReport = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl"),
            Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales"
        };

        TestData salesOrderDetailReport = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl"),
            Path = "/SSRSMigrate_AW_Tests/Reports/Sales Order Detail"
        };

        TestData storeContactsReport = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl"),
            Path = "/SSRSMigrate_AW_Tests/Reports/Store Contacts"
        };

        TestData doesNotExistReport = new TestData()
        {
            FileName =Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl"),
            Path = "/SSRSMigrate_AW_Tests/Reports/File Doesnt Exist"
        };

        TestData folderDesNotExistReport = new TestData()
        {
            FileName = Path.Combine(GetInputPath(), "SSRSMigrate_AW_Tests\\Folder Doesnt Exist"),
            Path = "/SSRSMigrate_AW_Tests/Folder Doesnt Exist"
        };
        #endregion

        #region Environment Methods
        // Static so they can be used in field initializers
        private static string GetOutPutPath()
        {
            // Use the test assembly's directory instead of where nunit runs the test
            string outputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            outputPath = outputPath.Replace("file:\\", "");

            return Path.Combine(outputPath, "ZipBundler_Output");
        }

        private static string GetInputPath()
        {
            // Use the test assembly's directory instead of where nunit runs the test
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            dir = dir.Replace("file:\\", "");

            return Path.Combine(dir, "Test AW Data\\ZipBundler");
        }

        private void SetUpEnvironment()
        {
            if (Directory.Exists(GetOutPutPath()))
                this.TearDownEnvironment();

            // Create output directory
            Directory.CreateDirectory(GetOutPutPath());
        }

        private void TearDownEnvironment()
        {
            // Delete output directory if it exists
            if (Directory.Exists(GetOutPutPath()))
                Directory.Delete(GetOutPutPath());
        }
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {

        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {

        }

        [SetUp]
        public void SetUp()
        {
            // Each test will add files to ZipFileWrapper using ZipBundler,
            //  so they need to get recreated for each test so the zip is in a clean state
            //  for each test.
            zipFileWrapper = new ZipFileWrapper();
            checkSumGenerator = new MD5CheckSumGenerator();
            zipBundler = new ZipBundler(zipFileWrapper, checkSumGenerator);

            this.SetUpEnvironment();
        }

        [TearDown]
        public void TearDown()
        {
            zipBundler = null;

            this.TearDownEnvironment();
        }

        #region GetZipPath Tests
        [Test]
        public void GetZipPath_File()
        {
            string itemFileName = awDataSource.FileName;
            string itemPath = awDataSource.Path;

            string expectedZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources";

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
            string expectedCheckSum = "7b4e44d94590f501ba24cd3904a925c3";

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, expectedZipPath);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedCheckSum, actual.CheckSum, itemFileName);
            Assert.AreEqual(expectedZipPath, actual.Path);
            Assert.AreEqual(expectedFileName, actual.FileName);
        }

        /// <summary>
        /// Test CreateEntrySummary for a file by passing in null value for itemFileName
        /// </summary>
        [Test]
        public void CreateEntrySummary_File_NullFileName()
        {
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources";

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
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources";

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
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Reports";
            string expectedFileName = "File Doesnt Exist.rdl";

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, zipPath);

            Assert.NotNull(actual);
            Assert.AreEqual("", actual.CheckSum);
            Assert.AreEqual(zipPath, actual.Path);
            Assert.AreEqual(expectedFileName, actual.FileName);
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

            // Check that the ZipBundler has the entry we added to DataSources
            Assert.NotNull(zipBundler.Entries["DataSources"][0]);

            // Check that the proper ZipPath exists in the DataSource entry we added
            Assert.AreEqual(
                "Export\\SSRSMigrate_AW_Tests\\Data Sources",
                zipBundler.Entries["DataSources"][0].Path);

            // Check that the checksum is correct for the DataSource entry we added
            Assert.AreEqual(
                "7b4e44d94590f501ba24cd3904a925c3",
                zipBundler.Entries["DataSources"][0].CheckSum);

            // Check that the filename is correct for the DataSource entry we added
            Assert.AreEqual(
                "AWDataSource.json",
                zipBundler.Entries["DataSources"][0].FileName);

            // Check that the DataSource file exists in ZipFileWrapper
            Assert.True(zipFileWrapper.FileExists("Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"));
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
                reportsFolder.FileName,
                reportsFolder.Path,
                true);

            // Check that the ZipBunlder has the entry we added to Folders
            Assert.NotNull(zipBundler.Entries["Folders"][0]);

            // Check that the proper ZipPath exists in the Folder entry we added
            Assert.AreEqual(
                "Export\\SSRSMigrate_AW_Tests\\Reports",
                zipBundler.Entries["Folders"][0].Path);

            // Check that the checksum is correct for the Folder entry we added
            Assert.AreEqual(
                "",
                zipBundler.Entries["Folders"][0].CheckSum);

            // Check that the filename is correct for the Folder entry we added
            Assert.AreEqual(
                "",
                zipBundler.Entries["Folders"][0].FileName);

            // Check that the Folder exists in ZipFileWrapper
            Assert.True(zipFileWrapper.FileExists("Export\\SSRSMigrate_AW_Tests\\Reports"));
        }

        [Test]
        public void AddItem_Folder_InvalidItemPath()
        {
            string itemFileName = reportsFolder.FileName;
            string itemPath = "/SSRSMigrate/Reports"; // This path is not contained within reportsFolder.FileName, so it is invalid

            Exception ex = Assert.Throws<Exception>(
                delegate
                {
                    zipBundler.AddItem("Folders",
                        itemFileName,
                        itemPath,
                        false);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Item path '{0}' is invalid.", itemPath)));
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

            // Check that ZipBundler has the entry we added to Reports
            Assert.NotNull(zipBundler.Entries["Reports"][0]);

            // Check that the proper ZipPath exists in the Reports entry we added
            Assert.AreEqual(
                "Export\\SSRSMigrate_AW_Tests\\Reports",
                zipBundler.Entries["Reports"][0].Path);

            // Check that the checksum is correct fot the Report entry we added
            Assert.AreEqual(
                "1adde7720ca2f0af49550fc676f70804",
                zipBundler.Entries["Reports"][0].CheckSum);

            // Check that the filename is correct for the Report entry we added
            Assert.AreEqual(
                "Company Sales.rdl",
                zipBundler.Entries["Reports"][0].FileName);

            // Check that the Report exists in ZipFileWrapper
            Assert.True(zipFileWrapper.FileExists("Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl"));
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
        #endregion

        #region AddItem Directory as File
        #endregion

        #region CreateSummary Tests
        #endregion

        #region Save Tests
        #endregion
    }
}
