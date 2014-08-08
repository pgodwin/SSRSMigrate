using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;
using System.IO;

namespace SSRSMigrate.Tests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class FolderItemExporterTests
    {
        FolderItemExporter exporter = null;
        Mock<IExportWriter> exportWriterMock = null;

        #region Expected Values
        FolderItem expectedFolderItem = null;
        string expectedFolderItemFileName = "C:\\temp\\SSRSMigrate_Tests\\Reports";
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            expectedFolderItem = new FolderItem()
                {
                    Name = "Reports",
                    Path = "/SSRSMigrate_Tests/Reports",
                };

            exportWriterMock = new Mock<IExportWriter>();

            exportWriterMock.Setup(e => e.Save(It.IsAny<string>(), It.IsAny<string>(), true));

            // Mock IExporter.Save where the directory exists but overwrite = false
            exportWriterMock.Setup(e => e.Save(expectedFolderItemFileName, false))
                .Throws(new IOException(string.Format("Directory '{0}' already exists.", expectedFolderItemFileName)));

            exporter = new FolderItemExporter(exportWriterMock.Object);
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
            FolderItemExporter dataSourceExporter = new FolderItemExporter(exportWriterMock.Object);

            Assert.NotNull(dataSourceExporter);
        }

        [Test]
        public void Constructor_NullIExportWriter()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    FolderItemExporter dataSourceExporter = new FolderItemExporter(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: exportWriter"));

        }
        #endregion

        #region FolderItemExporter.SaveItem Tests
        [Test]
        public void SaveItem()
        {
            ExportStatus actualStatus = exporter.SaveItem(expectedFolderItem, expectedFolderItemFileName, true);

            exportWriterMock.Verify(e => e.Save(expectedFolderItemFileName, true));

            Assert.True(actualStatus.Success);
            Assert.AreEqual(expectedFolderItemFileName, actualStatus.ToPath);
            Assert.AreEqual(expectedFolderItem.Path, actualStatus.FromPath);
        }

        [Test]
        public void SaveItem_FileExists_DontOverwrite()
        {
            ExportStatus actualStatus = exporter.SaveItem(expectedFolderItem, expectedFolderItemFileName, false);

            Assert.False(actualStatus.Success);
            Assert.NotNull(actualStatus.Errors);
            Assert.AreEqual(actualStatus.Errors[0], string.Format("Directory '{0}' already exists.", expectedFolderItemFileName));
        }

        [Test]
        public void SaveItem_NullItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    exporter.SaveItem(null, expectedFolderItemFileName);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
        }

        [Test]
        public void SaveItem_NullFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(expectedFolderItem, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void SaveItem_EmptyFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(expectedFolderItem, "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }
        #endregion
    }
}
