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
    class FolderItemImporter_Tests
    {
        private FolderItemImporter importer = null;
        private Mock<IFileSystem> fileSystemMock = null;
        private MockLogger loggerMock = null;

        #region Expected Data
        string expectedFolderItemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports";

        string expectedFolderItemPath = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests";
        

        FolderItem expectedFolderItem = new FolderItem()
                {
                    Name = "Reports",
                    Path = "/SSRSMigrate_AW_Tests/Reports",
                };

        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            loggerMock = new MockLogger();
            fileSystemMock = new Mock<IFileSystem>();

            // ISystemIOWrapper.Directory.Exists Method Mocks
            fileSystemMock.Setup(f => f.Directory.Exists(expectedFolderItemFileName))
                .Returns(() => true);

            // ISystemIOWrapper.Path.GetDirectoryName Method Mocks
            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedFolderItemFileName))
                .Returns(() => expectedFolderItemPath);

            fileSystemMock.Setup(f => f.Path.GetFileName(expectedFolderItemFileName))
                .Returns(() => expectedFolderItem.Name);

            importer = new FolderItemImporter(fileSystemMock.Object, loggerMock);
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
        public void ImportItem()
        {
            ImportStatus status = null;

            FolderItem actual = importer.ImportItem(expectedFolderItemFileName, out status);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedFolderItem.Name, actual.Name);
            Assert.AreEqual(expectedFolderItem.Path, actual.Path);
        }
        #endregion
    }
}
