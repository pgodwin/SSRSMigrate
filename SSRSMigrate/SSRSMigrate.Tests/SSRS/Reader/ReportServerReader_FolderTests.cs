using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Reader
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_FolderTests
    {
        private ReportServerReader reader = null;
        private Mock<IReportServerPathValidator> pathValidatorMock = null;
        private Mock<IReportServerRepository> reportServerRepositoryMock = null;

        #region GetFolders - Expected FolderItems
        FolderItem expectedFolderItem = null;
        List<FolderItem> expectedFolderItems = null;
        #endregion

        #region GetFolders - Actual FolderItems
        List<FolderItem> actualFolderItems = null;
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            // Setup expected FolderItems
            expectedFolderItem = new FolderItem()
            {
                Name = "Reports",
                Path = "/SSRSMigrate_AW_Tests/Reports",
            };

            expectedFolderItems = new List<FolderItem>()
            {
                expectedFolderItem,
                new FolderItem()
                {
                    Name = "Sub Folder",
                    Path = "/SSRSMigrate_AW_Tests/Reports/Sub Folder",
                },
                new FolderItem()
                {
                    Name = "Data Sources",
                    Path = "/SSRSMigrate_AW_Tests/Data Sources",
                }
            };
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            actualFolderItems = new List<FolderItem>();

            // Setup IReportServerRepository mock
            reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerPathValidator mock
            pathValidatorMock = new Mock<IReportServerPathValidator>();

            MockLogger logger = new MockLogger();

            reader = new ReportServerReader(reportServerRepositoryMock.Object, logger, pathValidatorMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            reader = null;
        }

        #region GetFolder Tests

        [Test]
        public void GetFolder()
        {
            reportServerRepositoryMock.Setup(r => r.GetFolder("/SSRSMigrate_AW_Tests/Reports"))
                .Returns(() => expectedFolderItem);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Reports"))
                .Returns(() => true);

            FolderItem actual = reader.GetFolder("/SSRSMigrate_AW_Tests/Reports");

            Assert.NotNull(actual);
            Assert.AreEqual(expectedFolderItem.Path, actual.Path);
            Assert.AreEqual(expectedFolderItem.Name, actual.Name);
        }

        [Test]
        public void GetFolder_PathDoesntExist()
        {
            reportServerRepositoryMock.Setup(r => r.GetFolder("/SSRSMigrate_AW_Tests/Doesnt Exist"))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Doesnt Exist"))
                .Returns(() => true);

            FolderItem actual = reader.GetFolder("/SSRSMigrate_AW_Tests/Doesnt Exist");

            Assert.Null(actual);
        }

        [Test]
        public void GetFolder_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetFolder(null);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetFolder_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetFolder("");
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetFolder_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            pathValidatorMock.Setup(r => r.Validate(invalidPath))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetFolder(invalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }
        #endregion

        #region GetFolders Tests
        [Test]
        public void GetFolders()
        {
            reportServerRepositoryMock.Setup(r => r.GetFolders("/SSRSMigrate_AW_Tests"))
                .Returns(() => expectedFolderItems);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_AW_Tests");

            Assert.AreEqual(expectedFolderItems.Count(), actual.Count());
        }

        [Test]
        public void GetFolders_PathDoesntExist()
        {
            reportServerRepositoryMock.Setup(r => r.GetFolders("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<FolderItem>());

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => true);

            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_AW_Tests Doesnt Exist");

            Assert.AreEqual(0, actual.Count());
        }

        [Test]
        public void GetFolders_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetFolders(null);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetFolders_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetFolders(null);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetFolders_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            pathValidatorMock.Setup(r => r.Validate(invalidPath))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetFolders(invalidPath);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }
        #endregion

        #region GetFolders Using Action<FolderItem> Tests
        [Test]
        public void GetFolders_UsingDelegate()
        {
            reportServerRepositoryMock.Setup(r => r.GetFolderList("/SSRSMigrate_AW_Tests"))
                .Returns(() => expectedFolderItems);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            reader.GetFolders("/SSRSMigrate_AW_Tests", GetFolders_Reporter);

            Assert.AreEqual(expectedFolderItems.Count(), actualFolderItems.Count());
        }

        [Test]
        public void GetFolders_UsingDelegate_PathDoesntExist()
        {
            reportServerRepositoryMock.Setup(r => r.GetFolder("/SSRSMigrate_AW_Tests/Doesnt Exist"))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => true);

            reader.GetFolders("/SSRSMigrate_AW_Tests Doesnt Exist", GetFolders_Reporter);

            Assert.AreEqual(0, actualFolderItems.Count());
        }

        [Test]
        public void GetFolders_UsingDelegate_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetFolders(null, GetFolders_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetFolders_UsingDelegate_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    reader.GetFolders("", GetFolders_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        public void GetFolders_UsingDelegate_NullDelegate()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    reader.GetFolders("/SSRSMigrate_AW_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));
        }

        [Test]
        public void GetFolders_UsingDelegate_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

            pathValidatorMock.Setup(r => r.Validate(invalidPath))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetFolders(invalidPath, GetFolders_Reporter);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion
    }
}
