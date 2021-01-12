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
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Writer
{
    [TestFixture]
    class ReportServerWriter_FolderTests
    {
        private ReportServerWriter writer = null;
        private Mock<IReportServerRepository> reportServerRepositoryMock = null;
        private Mock<IReportServerPathValidator> pathValidatorMock = null;

        #region Folder Items
        FolderItem rootFolderItem = null;
        FolderItem reportsFolderItem = null;
        FolderItem reportsSubFolderItem = null;
        FolderItem rootSubFolderItem = null;
        FolderItem alreadyExistsFolderItem = null;
        FolderItem invalidPathFolderItem = null;
        FolderItem errorFolderItem = null;

        List<FolderItem> folderItems = null;
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            rootFolderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests",
                Path = "/SSRSMigrate_AW_Tests",
            };

            reportsFolderItem = new FolderItem()
            {
                Name = "Reports",
                Path = "/SSRSMigrate_AW_Tests/Reports",
            };

            reportsSubFolderItem = new FolderItem()
            {
                Name = "Sub Folder",
                Path = "/SSRSMigrate_AW_Tests/Reports/Sub Folder",
            };

            rootSubFolderItem = new FolderItem()
            {
                Name = "Data Sources",
                Path = "/SSRSMigrate_AW_Tests/Data Sources",
            };

            alreadyExistsFolderItem = new FolderItem()
            {
                Name = "Already Exists",
                Path = "/SSRSMigrate_AW_Tests/Already Exists"
            };

            invalidPathFolderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests.Tests",
                Path = "/SSRSMigrate_AW.Tests",
            };

            errorFolderItem = new FolderItem()
            {
                Name = "ERROR",
                Path = "/SSRSMigrate_AW_Tests/ERROR"
            };

            folderItems = new List<FolderItem>()
            {
                rootFolderItem,
                reportsFolderItem,
                reportsSubFolderItem,
                rootSubFolderItem,
            };
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
            // Setup IReportServerRepository Mock
            reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerPathValidator mock
            pathValidatorMock = new Mock<IReportServerPathValidator>();

            MockLogger logger = new MockLogger();

            writer = new ReportServerWriter(reportServerRepositoryMock.Object, logger, pathValidatorMock.Object);

            writer.Overwrite = false; // Reset allow overwrite before each test
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region WriteFolder Tests
        [Test]
        public void WriteFolder()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            string actual = writer.WriteFolder(rootFolderItem);

            Assert.Null(actual);
        }

        [Test]
        public void WriteFolder_AlreadyExists()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(alreadyExistsFolderItem.Name, TesterUtility.GetParentPath(alreadyExistsFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsFolderItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsFolderItem.Path, "Folder"))
                .Returns(() => true);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteFolder(alreadyExistsFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsFolderItem.Path)));
        }

        [Test]
        public void WriteFolder_AlreadyExists_AllowOverwrite()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(alreadyExistsFolderItem.Name, TesterUtility.GetParentPath(alreadyExistsFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsFolderItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsFolderItem.Path, "Folder"))
                .Returns(() => true);

            writer.Overwrite = true;

            string actual = writer.WriteFolder(alreadyExistsFolderItem);

            Assert.Null(actual);

            reportServerRepositoryMock.Verify(r => r.DeleteItem(alreadyExistsFolderItem.Path));
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
            pathValidatorMock.Setup(r => r.Validate(invalidPathFolderItem.Path))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolder(invalidPathFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPathFolderItem.Path)));
        }

        [Test]
        public void WriteFolder_FolderItemNullName()
        {
            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            FolderItem folderItem = new FolderItem()
            {
                Name = null,
                Path = "/SSRSMigrate_AW_Tests",
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
            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            FolderItem folderItem = new FolderItem()
            {
                Name = "",
                Path = "/SSRSMigrate_AW_Tests",
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
            pathValidatorMock.Setup(r => r.Validate(null))
                .Returns(() => false);

            FolderItem folderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests",
                Path = null,
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolder(folderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", folderItem.Path)));
        }

        [Test]
        public void WriteFolder_FolderItemEmptyPath()
        {
            pathValidatorMock.Setup(r => r.Validate(null))
                .Returns(() => false);

            FolderItem folderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests",
                Path = "",
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolder(folderItem);
                });

            Assert.That(ex.Message, Does.Contain("Invalid path"));
        }

        [Test]
        public void WriteFolder_FolderItemError()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(errorFolderItem.Name, TesterUtility.GetParentPath(errorFolderItem)))
                .Returns(() => string.Format("Error writing folder '{0}': Error!", errorFolderItem.Path));

            pathValidatorMock.Setup(r => r.Validate(errorFolderItem.Path))
                .Returns(() => true);

            string actual = writer.WriteFolder(errorFolderItem);

            Assert.NotNull(actual);
            Assert.AreEqual(string.Format("Error writing folder '{0}': Error!", errorFolderItem.Path), actual);
        }
        #endregion

        #region WriteFolders Tests
        [Test]
        public void WriteFolders()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Name, TesterUtility.GetParentPath(reportsSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);


            string[] actual = writer.WriteFolders(folderItems.ToArray());

            Assert.IsEmpty(actual);
        }

        [Test]
        public void WriteFolders_OneOrMoreAlreadyExists()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Name, TesterUtility.GetParentPath(reportsSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsFolderItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsFolderItem.Path, "Folder"))
                .Returns(() => true);

            List<FolderItem> items = new List<FolderItem>();

            items.AddRange(folderItems);
            items.Add(alreadyExistsFolderItem);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteFolders(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsFolderItem.Path)));
        }

        [Test]
        public void WriteFolders_OneOrMoreAlreadyExists_AllowOverwrite()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Name, TesterUtility.GetParentPath(reportsSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Name, TesterUtility.GetParentPath(reportsFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(alreadyExistsFolderItem.Name, TesterUtility.GetParentPath(alreadyExistsFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsFolderItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsFolderItem.Path, "Folder"))
                .Returns(() => true);

            List<FolderItem> items = new List<FolderItem>();

            items.AddRange(folderItems);
            items.Add(alreadyExistsFolderItem);

            writer.Overwrite = true;

            string[] actual = writer.WriteFolders(items.ToArray());

            Assert.IsEmpty(actual);

            reportServerRepositoryMock.Verify(r => r.DeleteItem(alreadyExistsFolderItem.Path));
        }

        [Test]
        public void WriteFolders_NullFolderItems()
        {
            pathValidatorMock.Setup(r => r.Validate(null))
                .Returns(() => false);

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteFolders(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: folderItems"));
        }

        [Test]
        public void WriteFolders_OneOrMoreInvalidPaths()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Name, TesterUtility.GetParentPath(reportsSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(errorFolderItem.Name, TesterUtility.GetParentPath(errorFolderItem)))
                .Returns(() => string.Format("Error writing folder '{0}': Error!", errorFolderItem.Path));

            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(errorFolderItem.Path))
                .Returns(() => true);


            List<FolderItem> items = new List<FolderItem>();

            items.AddRange(folderItems);
            items.Add(invalidPathFolderItem);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolders(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPathFolderItem.Path)));
        }

        [Test]
        public void WriteFolders_OneOrMoreNullNames()
        {
            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            List<FolderItem> items = new List<FolderItem>();

            items.AddRange(folderItems);
            items.Add(new FolderItem()
            {
                Name = null,
                Path = "/SSRSMigrate_AW_Tests",
            });

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteFolders(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteFolders_OneOrMoreEmptyNames()
        {
            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            List<FolderItem> items = new List<FolderItem>();

            items.AddRange(folderItems);
            items.Add(new FolderItem()
            {
                Name = "",
                Path = "/SSRSMigrate_AW_Tests",
            });

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteFolders(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteFolders_OneOrMoreNullPaths()
        {
            pathValidatorMock.Setup(r => r.Validate(null))
                .Returns(() => false);

            FolderItem folderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests",
                Path = null,
            };

            List<FolderItem> items = new List<FolderItem>()
            {
                folderItem
            };

            items.AddRange(folderItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolders(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", folderItem.Path)));
        }

        [Test]
        public void WriteFolders_OneOrMoreEmptyPaths()
        {
            pathValidatorMock.Setup(r => r.Validate(""))
                .Returns(() => false);

            FolderItem folderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests",
                Path = "",
            };

            List<FolderItem> items = new List<FolderItem>()
            {
                folderItem
            };

            items.AddRange(folderItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteFolders(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", folderItem.Path)));
        }

        [Test]
        public void WriteFolders_OneOrMoreFolderItemError()
        {
            reportServerRepositoryMock.Setup(r => r.CreateFolder(errorFolderItem.Name, TesterUtility.GetParentPath(errorFolderItem)))
                .Returns(() => string.Format("Error writing folder '{0}': Error!", errorFolderItem.Path));

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootFolderItem.Name, TesterUtility.GetParentPath(rootFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsSubFolderItem.Name, TesterUtility.GetParentPath(reportsSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(rootSubFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.CreateFolder(reportsFolderItem.Name, TesterUtility.GetParentPath(rootSubFolderItem)))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(reportsFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(reportsSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(rootSubFolderItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(errorFolderItem.Path))
                .Returns(() => true);

            List<FolderItem> items = new List<FolderItem>()
            {
                errorFolderItem
            };

            items.AddRange(folderItems);

            string[] actual = writer.WriteFolders(items.ToArray());

            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(string.Format("Error writing folder '{0}': Error!", errorFolderItem.Path), actual[0]);
        }
        #endregion
    }
}
