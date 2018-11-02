using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Extensions.Logging.Log4net;
using NUnit.Framework;
using SSRSMigrate.Importer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.IntegrationTests.Importer
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class FolderItemImporter_Tests
    {
        private StandardKernel kernel = null;
        private FolderItemImporter importer = null;

        #region Test Data
        private string folderItemFilename = "Export\\SSRSMigrate_AW_Tests\\Reports";
        private string folderItem_NotFound_Filename = "Export\\SSRSMigrate_AW_Tests\\NotFound";
        private string folderItem_NoToken_Filename = "SSRSMigrate_AW_Tests\\Reports";
        #endregion

        #region Expected Data
        FolderItem expectedFolderItem = new FolderItem()
        {
            Name = "Reports",
            Path = "/SSRSMigrate_AW_Tests/Reports",
        };
        #endregion

        #region Environment Methods
        private string GetTestDataPath()
        {
            string outputPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            outputPath = outputPath.Replace("file:\\", "");

            return Path.Combine(outputPath, "Test AW Data\\Imports\\ExtractedArchive\\");
        }
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            kernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new DependencyModule());

            importer = kernel.Get<FolderItemImporter>();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void TearDown()
        {
        }

        #region ImportItem Tests
        /// <summary>
        /// Tests importing a 'Reports' folder item that exists on disk and will import successfully.
        /// </summary>
        [Test]
        public void ImportItem()
        {
            string filename = Path.Combine(this.GetTestDataPath(), folderItemFilename);

            ImportStatus status = null;

            FolderItem actual = importer.ImportItem(filename, out status);

            Assert.NotNull(actual);
            Assert.AreEqual(expectedFolderItem.Name, actual.Name);
            Assert.AreEqual(expectedFolderItem.Path, actual.Path);
        }

        /// <summary>
        /// Tests importing a folder item that does not exist on disk.
        /// </summary>
        [Test]
        public void ImportItem_NotFound()
        {
            string filename = Path.Combine(this.GetTestDataPath(), folderItem_NotFound_Filename);

            ImportStatus status = null;

            DirectoryNotFoundException ex = Assert.Throws<DirectoryNotFoundException>(
                delegate
                {
                    importer.ImportItem(filename, out status);
                });

            Assert.That(ex.Message, Is.EqualTo(filename)); 
        }

        /// <summary>
        /// Tests importing a folder item that is in a path that does not contain the 'Export' token in the path. 
        /// Items in these paths cannot be imported because the SSRS path cannot be parsed from it.
        /// </summary>
        [Test]
        public void ImportItem_NoToken()
        {
            string filename = Path.Combine(this.GetTestDataPath(), folderItem_NoToken_Filename);

            ImportStatus status = null;

            FolderItem actual = importer.ImportItem(filename, out status);

            Assert.Null(actual);
            Assert.IsFalse(status.Success);
            Assert.AreEqual(string.Format("Item's filename '{0}' does not contain token '\\Export\\'.",
                        filename),
                status.Error.Message);
        }
        #endregion
    }
}
