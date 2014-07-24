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
    class ReportServerReader_Tests
    {
        ReportServerReader reader = null;

        #region Expected FolderItems
        List<FolderItem> expectedFolderItems = null;
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

            reader = new ReportServerReader(reportServerRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }

        #region Folder Tests
        [Test]
        public void GetFolders()
        {
            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_Tests");

            Assert.AreEqual(expectedFolderItems.Count(), actual.Count());
        }

        [Test]
        public void GetFolders_NullPath()
        {
            List<FolderItem> actual = reader.GetFolders(null);
        }

        [Test]
        public void GetFolders_EmptyPath()
        {
            List<FolderItem> actual = reader.GetFolders("");
        }
        #endregion

        #region Report Tests
        #endregion

        #region Data Source Tests
        #endregion
    }
}
