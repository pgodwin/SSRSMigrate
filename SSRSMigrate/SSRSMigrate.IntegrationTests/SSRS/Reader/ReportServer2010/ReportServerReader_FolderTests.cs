using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.IntegrationTests.SSRS.Reader.ReportServer2010
{
    [TestFixture, Category("ConnectsToSSRS")]
    [CoverageExcludeAttribute]
    class ReportServerReader_FolderTests
    {
        IReportServerReader reader = null;

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

            reader = TestKernel.Instance.Get<IReportServerReader>("2010-SRC");
        }

        [OneTimeTearDown]
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

        #region GetFolder Tests
        [Test]
        public void GetFolder()
        {
            FolderItem actual = reader.GetFolder("/SSRSMigrate_AW_Tests/Reports");

            Assert.NotNull(actual);
            Assert.AreEqual(expectedFolderItem.Path, actual.Path);
            Assert.AreEqual(expectedFolderItem.Name, actual.Name);
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
        public void GetFolder_PathDoesntExist()
        {
            FolderItem actual = reader.GetFolder("/SSRSMigrate_AW_Tests/Doesnt Exist");

            Assert.Null(actual);
        }

        [Test]
        public void GetFolder_InvalidPath()
        {
            string invalidPath = "/SSRSMigrate_AW.Tests";

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
            List<FolderItem> actual = reader.GetFolders("/SSRSMigrate_AW_Tests");

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
        public void GetFolders_PathDoesntExist()
        {
            Assert.That(() => reader.GetFolders("/SSRSMigrate_AW_Tests Doesnt Exist"), Throws.TypeOf<System.Web.Services.Protocols.SoapException>());
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

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
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
        public void GetFolders_UsingDelegate_PathDoesntExist()
        {
            Assert.That(() => reader.GetFolders("/SSRSMigrate_AW_Tests Doesnt Exist", GetFolders_Reporter), Throws.TypeOf<System.Web.Services.Protocols.SoapException>());
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

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPath)));
        }

        private void GetFolders_Reporter(FolderItem folderItem)
        {
            actualFolderItems.Add(folderItem);
        }
        #endregion
    }
}
