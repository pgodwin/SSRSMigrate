using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;
using System.Reflection;
using System.IO;
using SSRSMigrate.Status;
using SSRSMigrate.Utility;
using SSRSMigrate.Exporter.Writer;
using Ninject;
using Ninject.Extensions.Logging.Log4net;

namespace SSRSMigrate.IntegrationTests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class FolderItemExporterTests
    {
        FolderItemExporter exporter = null;
        StandardKernel kernel = null;

        List<FolderItem> folderItems = null;

        string outputPath = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EnvironmentSetup();

            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            kernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new DependencyModule());

            exporter = kernel.Get<FolderItemExporter>();

            folderItems = new List<FolderItem>()
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

            outputPath = GetOutPutPath();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            EnvironmentTearDown();
        }

        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(outputPath).GetDirectories())
                dir.Delete(true);
        }

        #region Environment Setup/TearDown
        private string GetOutPutPath()
        {
            string outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(outputPath, "FolderItemExporter_Output");
        }

        private void EnvironmentSetup()
        {
            Directory.CreateDirectory(GetOutPutPath());
        }

        public void EnvironmentTearDown()
        {
            Directory.Delete(GetOutPutPath(), true);
        }
        #endregion

        [Test]
        public void ExportFolderItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(folderItems[0].Path);

            ExportStatus actualStatus = exporter.SaveItem(folderItems[0], filePath);

            Assert.True(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.Null(actualStatus.Errors);
            Assert.True(Directory.Exists(actualStatus.ToPath));
        }

        [Test]
        public void ExportFolderItem_NullItem()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(folderItems[0].Path);

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    exporter.SaveItem(null, filePath);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
        }

        [Test]
        public void ExportFolderItem_NullPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(folderItems[0], null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportFolderItem_EmptyPath()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(folderItems[0], "");
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void ExportFolderItem_FileDontOverwrite()
        {
            string filePath = outputPath + SSRSUtil.GetServerPathToPhysicalPath(folderItems[0].Path);

            // Create dummy directory
            Directory.CreateDirectory(filePath);

            ExportStatus actualStatus = exporter.SaveItem(folderItems[0], filePath, false);

            Assert.False(actualStatus.Success);
            Assert.AreEqual(filePath, actualStatus.ToPath);
            Assert.NotNull(actualStatus.Errors);
            Assert.True(actualStatus.Errors.Any(e => e.Contains(string.Format("Directory '{0}' already exists.", filePath)))); 
        }
    }
}
