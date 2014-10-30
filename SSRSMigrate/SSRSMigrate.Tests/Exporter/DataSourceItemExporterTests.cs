﻿using System;
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

        #region Expected Values
        DataSourceItem expectedDataSourceItem = null;

        string expectedDataSourceItemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\AWDataSource.json";

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
            expectedDataSourceItem = new DataSourceItem()
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

            expectedDataSourceItemJson = JsonConvert.SerializeObject(expectedDataSourceItem, Formatting.Indented);
            
            exportWriterMock = new Mock<IExportWriter>();
            serializeWrapperMock = new Mock<ISerializeWrapper>();

            // Mock IExporter.Save any argument with overwrite = True
            exportWriterMock.Setup(e => e.Save(It.IsAny<string>(), It.IsAny<string>(), true));
            
            // Mock IExporter.Save where the filename exists but overwrite = false
            exportWriterMock.Setup(e => e.Save(expectedDataSourceItemFileName, It.IsAny<string>(), false))
                .Throws(new IOException(string.Format("File '{0}' already exists.", expectedDataSourceItemFileName)));

            // Mock ISerializeWrapper.Serialize
            serializeWrapperMock.Setup(s => s.SerializeObject(expectedDataSourceItem))
                .Returns(() => expectedDataSourceItemJson);

            exporter = new DataSourceItemExporter(exportWriterMock.Object, serializeWrapperMock.Object);
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
        [Test]
        public void Constructor_Succeed()
        {
            DataSourceItemExporter dataSourceExporter = new DataSourceItemExporter(exportWriterMock.Object, serializeWrapperMock.Object);

            Assert.NotNull(dataSourceExporter);
        }

        [Test]
        public void Constructor_NullIExportWriter()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    DataSourceItemExporter dataSourceExporter = new DataSourceItemExporter(null, serializeWrapperMock.Object);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: exportWriter"));
        
        }

        [Test]
        public void Constructor_NullISerializeWrapper()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    DataSourceItemExporter dataSourceExporter = new DataSourceItemExporter(exportWriterMock.Object, null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: serializeWrapper"));

        }
        #endregion

        #region DataSourceItemExporeter.SaveItem Tests
        [Test]
        public void SaveItem()
        {
            ExportStatus actualStatus = exporter.SaveItem(expectedDataSourceItem, expectedDataSourceItemFileName, true);

            exportWriterMock.Verify(e => e.Save(expectedDataSourceItemFileName, expectedDataSourceItemJson, true));

            Assert.True(actualStatus.Success);
            Assert.AreEqual(expectedDataSourceItemFileName, actualStatus.ToPath);
            Assert.AreEqual(expectedDataSourceItem.Path, actualStatus.FromPath);
        }

        [Test]
        public void SaveItem_FileExists_DontOverwrite()
        {
            ExportStatus actualStatus = exporter.SaveItem(expectedDataSourceItem, expectedDataSourceItemFileName, false);

            Assert.False(actualStatus.Success);
            Assert.NotNull(actualStatus.Errors);
            Assert.AreEqual(actualStatus.Errors[0], string.Format("File '{0}' already exists.", expectedDataSourceItemFileName));
        }

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

        [Test]
        public void SaveItem_NullFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(expectedDataSourceItem, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void SaveItem_EmptyFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(expectedDataSourceItem, "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }
        #endregion
    }
}
