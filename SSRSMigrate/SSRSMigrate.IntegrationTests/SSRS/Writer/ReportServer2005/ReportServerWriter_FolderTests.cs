using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.TestHelper;
using System.Net;
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.IntegrationTests.SSRS.Writer.ReportServer2005
{
    /// <summary>
    /// This integration test will write FolderItems to a ReportingService2005 endpoint.
    /// The FolderItem objects used are already 'converted' to contain the destination information.
    /// </summary>
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportServerWriter_FolderTests
    {
        IReportServerWriter writer = null;
        string outputPath = null;

        #region FolderItems
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
            outputPath = Properties.Settings.Default.DestinationPath;

            rootFolderItem = new FolderItem()
            {
                Name =  string.Format("{0}", outputPath.Replace("/", "")),
                Path = string.Format("{0}", outputPath)
            };

            reportsFolderItem = new FolderItem()
            {
                Name = "Reports",
                Path = string.Format("{0}/Reports", outputPath)
            };

            reportsSubFolderItem = new FolderItem()
            {
                Name = "Sub Folder",
                Path = string.Format("{0}/Reports/Sub Folder", outputPath)
            };

            rootSubFolderItem = new FolderItem()
            {
                Name = "Data Sources",
                Path = string.Format("{0}/Data Sources", outputPath)
            };

            alreadyExistsFolderItem = new FolderItem()
            {
                Name = "Folder Already Exists",
                Path = "/SSRSMigrate_AW_Tests_Exists/Folder Already Exists"
            };

            invalidPathFolderItem = new FolderItem()
            {
                Name = "SSRSMigrate_AW_Tests.Tests",
                Path = "/SSRSMigrate_AW.Tests",
            };

            folderItems = new List<FolderItem>()
            {
                rootFolderItem,
                reportsFolderItem,
                reportsSubFolderItem,
                rootSubFolderItem,
            };

            writer = TestKernel.Instance.Get<IReportServerWriter>("2005-DEST");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
            SetupEnvironment();
        }

        [TearDown]
        public void TearDown()
        {
            TeardownEnvironment();
        }

        #region Environment Setup/Teardown
        private void SetupEnvironment()
        {
            ReportingService2005TestEnvironment.SetupFolderWriterEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath,
                new List<FolderItem> 
                { 
                    alreadyExistsFolderItem 
                });
        }

        private void TeardownEnvironment()
        {
            ReportingService2005TestEnvironment.TeardownFolderWriterEnvironment(
                Properties.Settings.Default.ReportServer2008WebServiceUrl,
                CredentialCache.DefaultNetworkCredentials,
                outputPath,
                new List<FolderItem> 
                { 
                    alreadyExistsFolderItem 
                });
        }
        #endregion

        #region WriteFolder Tests
        [Test]
        public void WriteFolder()
        {
            string actual = writer.WriteFolder(rootFolderItem);

            Assert.Null(actual);
        }

        [Test]
        public void WriteFolder_SubFolderParentDoesntExist()
        {
            string actual = writer.WriteFolder(reportsFolderItem);

            Assert.Null(actual);
        }

        [Test]
        public void WriteFolder_AlreadyExists()
        {
            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteFolder(alreadyExistsFolderItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsFolderItem.Path)));
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

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidPathFolderItem.Path)));
        }

        [Test]
        public void WriteFolder_FolderItemNullName()
        {
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

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
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
        public void WriteFolders_NullFolderItems()
        {
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
        #endregion
    }
}
