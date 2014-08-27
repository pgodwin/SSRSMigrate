using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.SSRS.Errors;
using System.Text.RegularExpressions;

namespace SSRSMigrate.Tests.SSRS.Reader
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_FolderTests
    {
        ReportServerReader reader = null;

        #region GetFolders - Expected FolderItems
        List<FolderItem> expectedFolderItems = null;
        #endregion

        #region GetFolders - Actual FolderItems
        List<FolderItem> actualFolderItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Setup expected FolderItems
            expectedFolderItems = new List<FolderItem>()
            {
                new FolderItem()
                {
                    Name = "Reports",
                    Path = "/SSRSMigrate_AW_Tests/Reports",
                },
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

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // IReportServerRepository.GetFolders Mocks
            reportServerRepositoryMock.Setup(r => r.GetFolders(null))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetFolders(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetFolders("/SSRSMigrate_AW_Tests"))
                .Returns(() => expectedFolderItems);

            reportServerRepositoryMock.Setup(r => r.GetFolders("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<FolderItem>());

            // IReportServerRepository.GetFolderList Mocks
            reportServerRepositoryMock.Setup(r => r.GetFolderList(null))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetFolderList(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetFolderList("/SSRSMigrate_AW_Tests"))
                .Returns(() => expectedFolderItems);

            reportServerRepositoryMock.Setup(r => r.GetFolderList("/SSRSMigrate_AW_Tests Doesnt Exist"))
                .Returns(() => new List<FolderItem>());

            // Setup IReportServerRepository.ValidatePath Mocks
            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests"))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests Doesnt Exist"))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests/Reports"))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests/Reports/Sub Folder"))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests/Data Sources"))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(null))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(""))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s, "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            reader = new ReportServerReader(reportServerRepositoryMock.Object);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            reader = null;
        }

        [SetUp]
        public void SetUp()
        {
            actualFolderItems = new List<FolderItem>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region GetFolders Tests
        [Test]
        public void GetFolders()
        {
            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_AW_Tests");

            Assert.AreEqual(expectedFolderItems.Count(), actual.Count());
        }

        [Test]
        public void GetFolders_PathDoesntExist()
        {
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

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetFolders(invalidPath);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }
        #endregion

        #region GetFolders Using Action<FolderItem> Tests
        [Test]
        public void GetFolders_UsingDelegate()
        {
            reader.GetFolders("/SSRSMigrate_AW_Tests", GetFolders_Reporter);

            Assert.AreEqual(expectedFolderItems.Count(), actualFolderItems.Count());
        }

        [Test]
        public void GetFolders_UsingDelegate_PathDoesntExist()
        {
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

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    reader.GetFolders(invalidPath, GetFolders_Reporter);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion
    }
}
