using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;
using Moq;
using SSRSMigrate.Exporter.Writer;
using Newtonsoft.Json;
using System.IO;
using SSRSMigrate.Status;
using SSRSMigrate.TestHelper.Logging;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Tests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class DataSourceItemExporterTests
    {
        DataSourceItemExporter exporter = null;
        Mock<IExportWriter> exportWriterMock = null;
        Mock<ISerializeWrapper> serializeWrapperMock = null;

        #region Test Data
        DataSourceItem dataSourceItem = new DataSourceItem()
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
        #endregion

        #region Expected Values
        string expectedDataSourceItemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\AWDataSource.json";

        // The expected json string to get when serializing dataSourceItem
        string expectedDataSourceItemJson = @"{
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
  ""CreatedBy"": ""nitzer\\jasper"",
  ""CreationDate"": ""2014-09-30T16:18:14.19"",
  ""Description"": null,
  ""ID"": ""7b79a3fc-bbcf-4773-881f-62ca675f1646"",
  ""ModifiedBy"": ""nitzer\\jasper"",
  ""ModifiedDate"": ""2014-09-30T16:18:38.907"",
  ""Size"": 307,
  ""VirtualPath"": null
}";
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            expectedDataSourceItemJson = JsonConvert.SerializeObject(dataSourceItem, Formatting.Indented);
            
            exportWriterMock = new Mock<IExportWriter>();
            serializeWrapperMock = new Mock<ISerializeWrapper>();
            var logger = new MockLogger();

            // Mock IExporter.Save any argument with overwrite = True
            exportWriterMock.Setup(e => e.Save(It.IsAny<string>(), It.IsAny<string>(), true));
            
            // Mock IExporter.Save where the filename exists but overwrite = false
            exportWriterMock.Setup(e => e.Save(expectedDataSourceItemFileName, It.IsAny<string>(), false))
                .Throws(new IOException(string.Format("File '{0}' already exists.", expectedDataSourceItemFileName)));

            // Mock ISerializeWrapper.Serialize
            serializeWrapperMock.Setup(s => s.SerializeObject(dataSourceItem))
                .Returns(() => expectedDataSourceItemJson);

            exporter = new DataSourceItemExporter(exportWriterMock.Object, serializeWrapperMock.Object, logger);
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

        #region Constructor Tests
        /// <summary>
        /// Test the constructor successfully.
        /// </summary>
        [Test]
        public void Constructor_Succeed()
        {
            var logger = new MockLogger();

            DataSourceItemExporter dataSourceExporter = new DataSourceItemExporter(exportWriterMock.Object, serializeWrapperMock.Object, logger);

            Assert.NotNull(dataSourceExporter);
        }

        /// <summary>
        /// Test the constuctor when passed a null IItemExporter parameter.
        /// </summary>
        [Test]
        public void Constructor_NullIExportWriter()
        {
            var logger = new MockLogger();

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    DataSourceItemExporter dataSourceExporter = new DataSourceItemExporter(null, serializeWrapperMock.Object, logger);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: exportWriter"));
        
        }

        /// <summary>
        /// Test the constuctor when passed a null ISerializeWrapper parameter.
        /// </summary>
        [Test]
        public void Constructor_NullISerializeWrapper()
        {
            var logger = new MockLogger();

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    DataSourceItemExporter dataSourceExporter = new DataSourceItemExporter(exportWriterMock.Object, null, logger);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: serializeWrapper"));

        }
        #endregion

        #region DataSourceItemExporeter.SaveItem Tests
        /// <summary>
        /// Test saving a DataSourcItem successfully.
        /// </summary>
        [Test]
        public void SaveItem()
        {
            ExportStatus actualStatus = exporter.SaveItem(dataSourceItem, expectedDataSourceItemFileName, true);

            exportWriterMock.Verify(e => e.Save(expectedDataSourceItemFileName, expectedDataSourceItemJson, true));

            Assert.True(actualStatus.Success);
            Assert.AreEqual(expectedDataSourceItemFileName, actualStatus.ToPath);
            Assert.AreEqual(dataSourceItem.Path, actualStatus.FromPath);
        }

        /// <summary>
        /// Test saving a DataSourceItem where the filename already exists on disk and overwrite is false.
        /// </summary>
        [Test]
        public void SaveItem_FileExists_DontOverwrite()
        {
            ExportStatus actualStatus = exporter.SaveItem(dataSourceItem, expectedDataSourceItemFileName, false);

            Assert.False(actualStatus.Success);
            Assert.NotNull(actualStatus.Errors);
            Assert.AreEqual(actualStatus.Errors[0], string.Format("File '{0}' already exists.", expectedDataSourceItemFileName));
        }

        /// <summary>
        /// Test saving a DataSourceItem where the item parameter is null.
        /// </summary>
        [Test]
        public void SaveItem_NullItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    exporter.SaveItem(null, expectedDataSourceItemFileName);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
        }

        /// <summary>
        /// Test saving a DataSourceItem where the the filename parameter is null.
        /// </summary>
        [Test]
        public void SaveItem_NullFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(dataSourceItem, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        /// <summary>
        /// Test saving a DataSourceItem where the filename parameter is empty.
        /// </summary>
        [Test]
        public void SaveItem_EmptyFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(dataSourceItem, "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }
        #endregion
    }
}
