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
            string outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(outputPath, "ZipBundler_Output");
        }

        private static string GetInputPath()
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
    }
}
