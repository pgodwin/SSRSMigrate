using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using Moq;
using SSRSMigrate.SSRS.Repository;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.Tests.SSRS.Writer
{
    [TestFixture]
    class ReportServerWriter_FolderTests
    {
        ReportServerWriter writer = null;

        #region Folder Items
        FolderItem rootFolderItem = null;
        FolderItem reportsFolderItem = null;
        FolderItem reportsSubFolderItem = null;
        FolderItem rootSubFolderItem = null;
        FolderItem alreadyExistsFolderItem = null;
        FolderItem invalidPathFolderItem = null;

        List<FolderItem> folderItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            rootFolderItem = new FolderItem()
            {
                Name = "SSRSMigrate_Tests",
                Path = "/SSRSMigrate_Tests",
            };

            reportsFolderItem = new FolderItem()
            {
                Name = "Reports",
                Path = "/SSRSMigrate_Tests/Reports",
            };

            reportsSubFolderItem = new FolderItem()
            {
                Name = "Sub Folder",
                Path = "/SSRSMigrate_Tests/Reports/Sub Folder",
            };

            rootSubFolderItem = new FolderItem()
            {
                Name = "Test Folder",
                Path = "/SSRSMigrate_Tests/Test Folder",
            };

            alreadyExistsFolderItem = new FolderItem()
            {
                Name = "Already Exists",
                Path = "/SSRSMigrate_Tests/Already Exists"
            };

            invalidPathFolderItem = new FolderItem()
            {
                Name = "SSRSMigrate.Tests",
                Path = "/SSRSMigrate.Tests",
            };

            folderItems = new List<FolderItem>()
            {
                rootFolderItem,
                reportsFolderItem,
                reportsSubFolderItem,
                rootSubFolderItem,
            };

            // Setup IReportServerRepository Mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // IReportServerRepository.CreateFolder Mocks
            reportServerRepositoryMock.Setup(r => r.CreateFolder(null, It.IsAny<string>()))
                .Throws(new ArgumentException("name"));

            reportServerRepositoryMock.Setup(r => r.CreateFolder("", It.IsAny<string>()))
                .Throws(new ArgumentException("name"));

            reportServerRepositoryMock.Setup(r => r.CreateFolder(It.IsAny<string>(), null))
                .Throws(new ArgumentException("parentPath"));

            reportServerRepositoryMock.Setup(r => r.CreateFolder(It.IsAny<string>(), ""))
               .Throws(new ArgumentException("parentPath"));

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Name, TesterUtility.GetParentPath(reportsFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Name, TesterUtility.GetParentPath(reportsSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
               .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(alreadyExistsFolderItem.Name, TesterUtility.GetParentPath(alreadyExistsFolderItem)))
                .Throws(new FolderAlreadyExistsException(string.Format("The folder '{0}' already exists.", alreadyExistsFolderItem.Path)));

            // IReportServerRepository.ValidatePath Mocks
            //TODO Need to fix this so it correctly mocks the ValidatePath with a null or empty parameter.
            reportServerRepositoryMock.Setup(r => r.ValidatePath(null))
               .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.ValidatePath(""))
               .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s, "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s, "[:?;@&=+$,\\*><|.\"]+") == false && !string.IsNullOrEmpty(s))))
               .Returns(() => true);

            writer = new ReportServerWriter(reportServerRepositoryMock.Object);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
            folderItems = new List<FolderItem>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region WriteFolder Tests
        [Test]
        public void WriteFolder()
        {
            string actual = writer.WriteFolder(rootFolderItem);

            Assert.Null(actual);
        }

        [Test]
        public void WriteFolder_AlreadyExists()
        {
            FolderAlreadyExistsException ex = Assert.Throws<FolderAlreadyExistsException>(
                delegate
                {
                    writer.WriteFolder(alreadyExistsFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The folder '{0}' already exists.", alreadyExistsFolderItem.Path)));
        }

        [Test]
        public void WriteFolder_NullFolderItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteFolder(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: folderItem"));
        }

        [Test]
        public void WriteFolder_InvalidPath()
        {
            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolder(invalidPathFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(invalidPathFolderItem.Path));
        }

        [Test]
        public void WriteFolder_FolderItemNullName()
        {
            FolderItem folderItem = new FolderItem()
            {
                Name = null,
                Path = "/SSRSMigrate_Tests",
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteFolder(folderItem);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteFolder_FolderItemEmptyName()
        {
            FolderItem folderItem = new FolderItem()
            {
                Name = "",
                Path = "/SSRSMigrate_Tests",
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteFolder(folderItem);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteFolder_FolderItemNullPath()
        {
            FolderItem folderItem = new FolderItem()
            {
                Name = "SSRSMigrate_Tests",
                Path = null,
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteFolder(folderItem);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void WriteFolder_FolderItemEmptyPath()
        {
            FolderItem folderItem = new FolderItem()
            {
                Name = "SSRSMigrate_Tests",
                Path = "",
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteFolder(folderItem);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }
        #endregion

        #region WriteFolders Tests
        [Test]
        public void WriteFolders()
        {
            string[] actual = writer.WriteFolders(folderItems.ToArray());

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void WriteFolders_OneOrMoreAlreadyExists()
        {

        }

        [Test]
        public void WriteFolders_NullFolderItems()
        {

        }

        [Test]
        public void WriteFolders_OneOrMoreInvalidPaths()
        {

        }

        [Test]
        public void WriteFolders_OneOrMoreNullNames()
        {

        }

        [Test]
        public void WriteFolders_OneOrMoreEmptyNames()
        {

        }

        [Test]
        public void WriteFolders_OneOrMoreNullPaths()
        {

        }

        [Test]
        public void WriteFolders_OneOrMoreEmptyPaths()
        {

        }
        #endregion
    }
}
