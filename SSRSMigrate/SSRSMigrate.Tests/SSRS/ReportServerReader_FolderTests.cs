using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;
using Moq;

namespace SSRSMigrate.Tests.SSRS
{
    [TestFixture]
    class ReportServerReader_FolderTests
    {
        ReportServerReader reader = null;

        #region GetFolders - Expected FolderItems
        List<FolderItem> expectedFolderItems = null;
        #endregion

        #region GetFolders - Actual FolderItems
        List<FolderItem> actualFolderItems = null;
        #endregion

        [SetUp]
        public void SetUp()
        {
            // Setup expected FolderItems
            expectedFolderItems = new List<FolderItem>()
            {
                new FolderItem()
                {
                    Name = "Reports",
                    Path = "/SSRSMigrate_Tests/Reports",
                },
                new FolderItem()
                {
                    Name = "Sub Folder",
                    Path = "/SSRSMigrate_Tests/Reports/Sub Folder",
                },
                new FolderItem()
                {
                    Name = "Test Folder",
                    Path = "/SSRSMigrate_Tests/Test Folder",
                }
            };

            actualFolderItems = new List<FolderItem>();

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup method mocks

            // IReportServerRepository.GetFolders
            reportServerRepositoryMock.Setup(r => r.GetFolders(null))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetFolders(""))
                .Throws(new ArgumentException("path"));

            reportServerRepositoryMock.Setup(r => r.GetFolders("/SSRSMigrate_Tests"))
                .Returns(() => expectedFolderItems);

            reportServerRepositoryMock.Setup(r => r.GetFolderList("/SSRSMigrate_Tests"))
                .Returns(() => expectedFolderItems);

            reader = new ReportServerReader(reportServerRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region GetFolders Tests
        [Test]
        public void GetFolders()
        {
            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_Tests");

            Assert.AreEqual(expectedFolderItems.Count(), actual.Count());
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
        #endregion

        #region GetFolders Using Action<FolderItem> Tests
        [Test]
        public void GetFolders_UsingDelegate()
        {
            reader.GetFolders("/SSRSMigrate_Tests", GetFolders_Reporter);

            Assert.AreEqual(expectedFolderItems.Count(), actualFolderItems.Count());
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
                    reader.GetFolders("/SSRSMigrate_Tests", null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: progressReporter"));
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion

        #region Report Tests
        #endregion

        #region Data Source Tests
        #endregion
    }
}
