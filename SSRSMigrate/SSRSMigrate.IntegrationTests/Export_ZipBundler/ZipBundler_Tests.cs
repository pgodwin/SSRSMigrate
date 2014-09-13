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
            zipFileWrapper = new ZipFileWrapper();
            checkSumGenerator = new MD5CheckSumGenerator();
            zipBundler = new ZipBundler(zipFileWrapper, checkSumGenerator);
        }

        [TearDown]
        public void TearDown()
        {
            zipBundler = null;
        }

        #region GetZipPath Tests
        [Test]
        public void GetZipPath_File()
        {
            string itemFileName = awDataSource.FileName;
            string itemPath = awDataSource.Path;

            string expectedZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

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
            string expectedZipPath = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
            string expectedCheckSum = "7b4e44d94590f501ba24cd3904a925c3";

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, expectedZipPath);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedCheckSum, actual.CheckSum, itemFileName);
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
            string zipPath = "Export\\SSRSMigrate_AW_Tests\\Reports\\File Doesnt Exist.rdl";

            BundleSummaryEntry actual = zipBundler.CreateEntrySummary(itemFileName, zipPath);

            Assert.NotNull(actual);
            Assert.AreEqual("", actual.CheckSum);
            Assert.AreEqual(zipPath, actual.Path);
        }
        #endregion

        #region AddItem DataSource Tests
        //[Test]
        //public void AddItem_DataSource()
        //{
        //    zipBundler.AddItem("DataSources",
        //        awDataSource.FileName,
        //        awDataSource.Path,
        //        false);

        //    Assert.NotNull(zipBundler.Entries["DataSources"][0]);

        //    Assert.AreEqual(
        //        "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
        //        zipBundler.Entries["DataSources"][0].Path);

        //    Assert.AreEqual(
        //        "7b4e44d94590f501ba24cd3904a925c3",
        //        zipBundler.Entries["DataSources"][0].CheckSum);
        //}
        #endregion

        #region AddItem Folder Tests
        #endregion

        #region AddItem Report Tests
        #endregion

        #region AddItem Directory as File
        #endregion

        #region CreateSummary Tests
        #endregion

        #region Save Tests
        #endregion
    }
}
