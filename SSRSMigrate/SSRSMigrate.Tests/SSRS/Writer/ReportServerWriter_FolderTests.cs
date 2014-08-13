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
            reportServerRepositoryMock.Setup(r => r.CreateFolder(null))
                .Throws(new ArgumentException("folderPath"));

            reportServerRepositoryMock.Setup(r => r.CreateFolder(""))
                .Throws(new ArgumentException("folderPath"));

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Path))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Path))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Path))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Path))
               .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(alreadyExistsFolderItem.Path))
                .Throws(new FolderAlreadyExistsException(string.Format("The folder '{0}' already exists.", alreadyExistsFolderItem.Path)));

            // IReportServerRepository.ValidatePath Mocks
            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s, "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s, "[:?;@&=+$,\\*><|.\"]+") == false)))
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
        public void CreateFolder()
        {
            string actual = writer.WriteFolder(rootFolderItem);

            Assert.Null(actual);
        }

        [Test]
        public void CreateFolder_AlreadyExists()
        {
            FolderAlreadyExistsException ex = Assert.Throws<FolderAlreadyExistsException>(
                delegate
                {
                    writer.WriteFolder(alreadyExistsFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The folder '{0}' already exists.", alreadyExistsFolderItem.Path)));
        }

        [Test]
        public void CreateFolder_NullFolderPath()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteFolder(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: folderItem"));
        }

        [Test]
        public void CreateFolder_InvalidPath()
        {
            InvalidPathCharsException ex = Assert.Throws<InvalidPathCharsException>(
                delegate
                {
                    writer.WriteFolder(invalidPathFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(invalidPathFolderItem.Path));
        }
        #endregion

        #region WriteFolders Tests
        #endregion
    }
}
