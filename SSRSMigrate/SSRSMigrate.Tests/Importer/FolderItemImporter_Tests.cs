using System;
using System.Collections.Generic;
using System.IO;
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
        // Expected successful import item data
        private string expectedFolderItemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports";

        private string expectedFolderItemPath = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests";

        private FolderItem expectedFolderItem = new FolderItem()
                {
                    Name = "Reports",
                    Path = "/SSRSMigrate_AW_Tests/Reports",
                };

        // Expected data for a folder item that does not exist on disk
        private string expectedFolderItem_NotFound_FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\NotFound";

        // Expected data for a folder item that is in a path that does not contain a proper token.
        // These would be items in a path that is not a child of an 'Export' folder.
         private string expectedFolderItem_NoToken_FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\SSRSMigrate_AW_Tests\\NoToken";
        
        private string expectedFolderItemPath_NoToken = "C:\\temp\\SSRSMigrate_AW_Tests\\SSRSMigrate_AW_Tests";

        private FolderItem expectedFolderItem_NoToken = new FolderItem()
        {
            Name = "NoToken",
            Path = "/SSRSMigrate_AW_Tests/NoToken",
        };
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            loggerMock = new MockLogger();
            fileSystemMock = new Mock<IFileSystem>();

            //TODO: Move mock setups to each test that actually uses them.

            // ISystemIOWrapper.Directory.Exists Method Mocks
            fileSystemMock.Setup(f => f.Directory.Exists(expectedFolderItemFileName))
                .Returns(() => true);

            fileSystemMock.Setup(f => f.Directory.Exists(expectedFolderItem_NotFound_FileName))
                .Returns(() => false);

            fileSystemMock.Setup(f => f.Directory.Exists(expectedFolderItem_NoToken_FileName))
                .Returns(() => true);

            // ISystemIOWrapper.Path.GetDirectoryName Method Mocks
            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedFolderItemFileName))
                .Returns(() => expectedFolderItemPath);

            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedFolderItem_NoToken_FileName))
                .Returns(() => expectedFolderItemPath_NoToken);

            // ISystemIOWrapper.Path.GetFileName Method Mocks
            fileSystemMock.Setup(f => f.Path.GetFileName(expectedFolderItemFileName))
                .Returns(() => expectedFolderItem.Name);

            fileSystemMock.Setup(f => f.Path.GetFileName(expectedFolderItem_NoToken_FileName))
                .Returns(() => expectedFolderItem_NoToken.Name);

            importer = new FolderItemImporter(fileSystemMock.Object, loggerMock);
        }

        [OneTimeTearDown]
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
        /// <summary>
        /// Tests importing a 'Reports' folder item that exists on disk and will import successfully.
        /// </summary>
        [Test]
        public void ImportItem_ReportsFolder()
        {
            ImportStatus status = null;

            FolderItem actual = importer.ImportItem(expectedFolderItemFileName, out status);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedFolderItem.Name, actual.Name);
            Assert.AreEqual(expectedFolderItem.Path, actual.Path);
        }

        /// <summary>
        /// Tests importing a folder item that does not exist on disk.
        /// </summary>
        [Test]
        public void ImportItem_NotFound()
        {
            ImportStatus status = null;

            DirectoryNotFoundException ex = Assert.Throws<DirectoryNotFoundException>(
                delegate
                {
                    importer.ImportItem(expectedFolderItem_NotFound_FileName, out status);
                });

            Assert.That(ex.Message, Is.EqualTo(expectedFolderItem_NotFound_FileName));
        }

        /// <summary>
        /// Tests importing a folder item that is in a path that does not contain the 'Export' token in the path. 
        /// Items in these paths cannot be imported because the SSRS path cannot be parsed from it.
        /// </summary>
        [Test]
        public void ImportItem_NoToken()
        {
            ImportStatus status = null;

            FolderItem actual = importer.ImportItem(expectedFolderItem_NoToken_FileName, out status);

            Assert.Null(actual);
            Assert.IsFalse(status.Success);
            Assert.AreEqual(string.Format("Item's filename '{0}' does not contain token '\\Export\\'.",
                        expectedFolderItem_NoToken_FileName), 
                status.Error.Message);
        }
        #endregion
    }
}
