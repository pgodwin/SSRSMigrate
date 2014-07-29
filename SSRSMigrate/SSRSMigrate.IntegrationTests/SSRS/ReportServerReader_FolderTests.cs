using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS;

namespace SSRSMigrate.IntegrationTests.SSRS
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerReader_FolderTests
    {
        ReportServerReader reader = null;

        List<FolderItem> actualFolderItems = null;
        List<FolderItem> expectedFolderItems = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Setup expected Folder Items
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

            reader = DependencySingleton.Instance.Get<ReportServerReader>();
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
            actualFolderItems = null;
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
                   reader.GetFolders("");
               });

            Assert.That(ex.Message, Is.EqualTo("path"));
        }

        [Test]
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException), 
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found", 
            MatchType = MessageMatch.Contains)]
        public void GetFolders_PathDoesntExist()
        {
            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_Tests Doesnt Exist");
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

        [Test]
        [ExpectedException(typeof(System.Web.Services.Protocols.SoapException),
            ExpectedMessage = "The item '/SSRSMigrate_Tests Doesnt Exist' cannot be found",
            MatchType = MessageMatch.Contains)]
        public void GetFolders_UsingDelegate_PathDoesntExist()
        {
            reader.GetFolders("/SSRSMigrate_Tests Doesnt Exist", GetFolders_Reporter);

            Assert.AreEqual(expectedFolderItems.Count(), actualFolderItems.Count());
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion
    }
}
