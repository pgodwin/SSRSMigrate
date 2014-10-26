using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using SSRSMigrate.Importer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.Importer
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class DataSourceItemImporter_Tests
    {
        private DataSourceItemImporter importer = null;
        private Mock<IFileSystem> fileSystemMock = null;
        private MockLogger loggerMock = null;

        #region Expected Data

        private string expectedAWDataSourceFileName =
            "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

        private string expectedAWDataSource = @"{
  ""ConnectString"": ""Data Source=(local);Initial Catalog=AdventureWorks2008"",
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
  ""CreatedBy"": ""nitzer\\jasper"",
  ""CreationDate"": ""2014-09-30T16:18:14.19"",
  ""Description"": null,
  ""ID"": ""7b79a3fc-bbcf-4773-881f-62ca675f1646"",
  ""ModifiedBy"": ""nitzer\\jasper"",
  ""ModifiedDate"": ""2014-09-30T16:18:38.907"",
  ""Size"": 307,
  ""VirtualPath"": null
}";

        private DataSourceItem expectedAWDataSourceItem = new DataSourceItem()
        {

        };

        private string expectedTestDataSourceFileName =
            "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json";
        
        private string expectedTestDataSource = @"{
  ""ConnectString"": ""Data Source=(local);Initial Catalog=AdventureWorks2008"",
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
  ""Path"": ""/SSRSMigrate_AW_Tests/Data Sources/Test Data Source"",
  ""CreatedBy"": ""nitzer\\jasper"",
  ""CreationDate"": ""2014-09-30T16:18:29.093"",
  ""Description"": null,
  ""ID"": ""72946d28-f222-400a-a42c-f1febf0dc49f"",
  ""ModifiedBy"": ""nitzer\\jasper"",
  ""ModifiedDate"": ""2014-09-30T16:18:52.543"",
  ""Size"": 307,
  ""VirtualPath"": null
}";

        private DataSourceItem expectedTestDataSourceItem = new DataSourceItem()
        {
           
        };
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggerMock = new MockLogger();
            fileSystemMock = new Mock<IFileSystem>();

            // TODO ISystemIOWrapper.FileExists Method Mocks
            // ...

            importer = new DataSourceItemImporter(fileSystemMock.Object, loggerMock);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {

        }

        #region ImportItem Tests
        [Test]
        public void ImportItem_AWDataSource()
        {
            ImportStatus status = null;

            DataSourceItem actual = importer.ImportItem(expectedAWDataSourceFileName, out status);

            Assert.NotNull(actual);
        }
        #endregion
    }
}
