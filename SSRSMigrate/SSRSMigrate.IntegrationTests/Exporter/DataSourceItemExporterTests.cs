using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Utility;
using SSRSMigrate.Exporter.Writer;
using Ninject;

namespace SSRSMigrate.IntegrationTests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class DataSourceItemExporterTests
    {
        StandardKernel kernel = null;
        DataSourceItemExporter exporter = null;

        DataSourceItem dataSourceItem = null;

        #region Expected DataSourceItem serialzed as JSON
        string expectedDataSourceJson = @"{
  ""ConnectString"": ""Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008"",
  ""CredentialsRetrieval"": ""Integrated"",
  ""Enabled"": true,
  ""EnabledSpecified"": true,
  ""Extension"": ""SQL"",
  ""ImpersonateUser"": false,
  ""ImpersonateUserSpecified"": true,
  ""OriginalConnectStringExpressionBased"": false,
  ""Password"": null,
  ""Prompt"": ""Enter a user name and password to access the data source:"",
  ""UseOriginalConnectString"": false,
  ""UserName"": null,
  ""WindowsCredentials"": false,
  ""Name"": ""AWDataSource"",
  ""Path"": ""/SSRSMigrate_AW_Tests/Data Sources/AWDataSource"",
  ""CreatedBy"": null,
  ""CreationDate"": ""0001-01-01T00:00:00"",
  ""Description"": null,
  ""ID"": null,
  ""ModifiedBy"": null,
  ""ModifiedDate"": ""0001-01-01T00:00:00"",
  ""Size"": 0,
  ""VirtualPath"": null
}";
        #endregion

        string outputPath = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EnvironmentSetup();

            kernel = new StandardKernel(new DependencyModule());

            exporter = kernel.Get<DataSourceItemExporter>();

            dataSourceItem = new DataSourceItem()
            {
                Description = null,
                VirtualPath = null,
                Name = "AWDataSource",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource",
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
                CredentialsRetrieval = "Integrated",
                Enabled = true,
                EnabledSpecified = true,
                Extension = "SQL",
                ImpersonateUser = false,
                ImpersonateUserSpecified = true,
                OriginalConnectStringExpressionBased = false,
                Password = null,
                Prompt = "Enter a user name and password to access the data source:",
                UseOriginalConnectString = false,
                UserName = null,
                WindowsCredentials = false
            };

            outputPath = GetOutPutPath();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EnvironmentTearDown();
        }

        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(outputPath).GetDirectories())
                dir.Delete(true);
        }

        #region Environment Setup/TearDown
        private string GetOutPutPath()
        {
            string outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(outputPath, "DataSourceItemExporter_Output");
        }

        private void EnvironmentSetup()
        {
            Directory.CreateDirectory(GetOutPutPath());
        }

        public void EnvironmentTearDown()
        {
            Directory.Delete(GetOutPutPath(), true);
        }
        #endregion

        [Test]
        public void ExportDataSourceItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(dataSourceItem.Path, "json");
 
            ExportStatus actualStatus = exporter.SaveItem(dataSourceItem, filePath);

            Assert.True(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.Null(actualStatus.Errors);
            Assert.True(File.Exists(filePath));
            Assert.AreEqual(expectedDataSourceJson, File.ReadAllText(filePath));
        }

        [Test]
        public void ExportDataSourceItem_NullItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(dataSourceItem.Path, "json");

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    exporter.SaveItem(null, filePath);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
        }
        
        [Test]
        public void ExportDataSourceItem_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(dataSourceItem, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportDataSourceItem_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(dataSourceItem, "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportDatasourceItem_FileDontOverwrite()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(dataSourceItem.Path, "json");
            
            // Create dummy file, so the output file already exists, causing the SaveItem to fail
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, "DUMMY FILE");

            ExportStatus actualStatus = exporter.SaveItem(dataSourceItem, filePath, false);

            Assert.False(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.NotNull(actualStatus.Errors);
            Assert.True(actualStatus.Errors.Any(e => e.Contains(string.Format("File '{0}' already exists.", filePath))));
        }
    }

}
