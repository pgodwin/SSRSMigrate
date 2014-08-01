using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.IO;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.IntegrationTests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class DataSourceItemExporterTests
    {
        DataSourceItemExporter exporter = null;

        DataSourceItem dataSourceItem = null;

        string expected = @"{
  ""ConnectString"": ""Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest"",
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
  ""Name"": ""Test Data Source"",
  ""Path"": ""/SSRSMigrate_Tests/Test Data Source"",
  ""CreatedBy"": null,
  ""CreationDate"": ""0001-01-01T00:00:00"",
  ""Description"": null,
  ""ID"": null,
  ""ModifiedBy"": null,
  ""ModifiedDate"": ""0001-01-01T00:00:00"",
  ""Size"": 0,
  ""VirtualPath"": null
}";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EnvironmentSetup();

            exporter = new DataSourceItemExporter();

            dataSourceItem = new DataSourceItem()
            {
                Description = null,
                Name = "Test Data Source",
                Path = "/SSRSMigrate_Tests/Test Data Source",
                ConnectString = "Data Source=(local);Initial Catalog=TestDatabase;Application Name=SSRSMigrate_IntegrationTest",
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
        public void ExportDataSource()
        {
            string filePath = Path.Combine(GetOutPutPath(), "SSRSMigrate_Tests")  + "\\Test Data Souce.json";
 
            ExportStatus actualStatus = exporter.SaveItem(dataSourceItem, filePath);

            Assert.True(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.Null(actualStatus.Errors);
        }
    }
}
