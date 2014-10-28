using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SSRSMigrate.Importer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;
using SSRSMigrate.TestHelper.Logging;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Tests.Importer
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class DataSourceItemImporter_Tests
    {
        private DataSourceItemImporter importer = null;
        private Mock<IFileSystem> fileSystemMock = null;
        private Mock<ISerializeWrapper> serializeWrapperMock = null;
        private MockLogger loggerMock = null;

        #region Expected Data
        private string expectedAWDataSourceFileName =
            "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";

        private string expectedAWDataSourcePath =
            "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources";

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
            CreatedBy = "DOMAIN\\user",
            CreationDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
            Description = null,
            ID = Guid.NewGuid().ToString(),
            ModifiedBy = "DOMAIN\\user",
            ModifiedDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
            Size = 414,
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

        private string expectedDataSourceItem_NotFound_Filename = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\NotFound.json";

        private string expectedAWDataSource_DeserializeException_FileName =
            "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources\\Exception.json";

        private string expectedAWDataSourcePath_DeserializeException =
            "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Data Sources";

        private string expectedAWDataSource_DeserializeException = @"{
		""ConnectString"": ""Data Source=(local);Initial Catalog=AdventureWorks2008"",
		""CredentialsRetrieval"": ""Integrated"",
		""Enabled"": true,
		""EnabledSpecified"": true,
		""Extension"": ""SQL"",
		""ImpersonateUser"": false,
		""ImpersonateUserSpecified"": true,
		""OriginalConnectStringExpressionBased"": false,
		""Password"": null,";
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggerMock = new MockLogger();
            fileSystemMock = new Mock<IFileSystem>();
            serializeWrapperMock = new Mock<ISerializeWrapper>();

            // ISystemIOWrapper.File.ReadAllText Method Mocks
            fileSystemMock.Setup(f => f.File.ReadAllText(expectedAWDataSourceFileName))
                .Returns(() => expectedAWDataSource);

            fileSystemMock.Setup(f => f.File.ReadAllText(expectedAWDataSource_DeserializeException_FileName))
                .Returns(() => expectedAWDataSource_DeserializeException);

            // ISystemIOWrapper.File.Exists Method Mocks
            fileSystemMock.Setup(f => f.File.Exists(expectedAWDataSourceFileName))
                .Returns(() => true);

            fileSystemMock.Setup(f => f.File.Exists(expectedAWDataSource_DeserializeException_FileName))
                .Returns(() => true);

            fileSystemMock.Setup(f => f.File.Exists(expectedDataSourceItem_NotFound_Filename))
                .Returns(() => false);

            // ISystemIOWrapper.Path.GetDirectoryName Method Mocks
            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedAWDataSourceFileName))
                .Returns(() => expectedAWDataSourcePath);

            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedAWDataSource_DeserializeException_FileName))
               .Returns(() => expectedAWDataSourcePath_DeserializeException);

            // ISerializeWrapper.DeserializeObject<DataSourceItem> Method Mocks
            serializeWrapperMock.Setup(s => s.DeserializeObject<DataSourceItem>(expectedAWDataSource))
                .Returns(() => expectedAWDataSourceItem);

            serializeWrapperMock.Setup(
                s => s.DeserializeObject<DataSourceItem>(expectedAWDataSource_DeserializeException))
                .Throws(
                    new JsonSerializationException(
                        "Unexpected end when deserializing object. Path 'Password', line 10, position 20."));

            importer = new DataSourceItemImporter(serializeWrapperMock.Object, fileSystemMock.Object, loggerMock);
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
            Assert.NotNull(status);
            Assert.AreEqual(actual.Name, expectedAWDataSourceItem.Name);
            Assert.AreEqual(actual.Path, expectedAWDataSourceItem.Path);
            Assert.True(status.Success);
        }

        [Test]
        public void ImportItem_NotFound()
        {
            ImportStatus status = null;

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
               delegate
               {
                   importer.ImportItem(expectedDataSourceItem_NotFound_Filename, out status);
               });

            Assert.That(ex.Message, Is.EqualTo(expectedDataSourceItem_NotFound_Filename));
        }

        [Test]
        public void ImportItem_DeserializeException()
        {
            ImportStatus status = null;

            DataSourceItem actual = importer.ImportItem(expectedAWDataSource_DeserializeException_FileName, out status);

            Assert.IsNull(actual);
            Assert.IsFalse(status.Success);
            Assert.AreEqual("Unexpected end when deserializing object. Path 'Password', line 10, position 20.",
                status.Error.Message);
        }
        #endregion
    }
}
