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
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.IntegrationTests.Importer
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportItemImporter_Tests
    {
        private StandardKernel kernel = null;
        private ReportItemImporter importer = null;

        #region Test Data
        private string reportItemFilename = "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl";
        private string reportItem_NotFound_Filename = "Export\\SSRSMigrate_AW_Tests\\Reports\\NotFound.rdl";
        private string reportItem_NoToken_Filename = "SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl";
        #endregion

        #region Expected Data
        ReportItem expectedReportItem = new ReportItem()
        {
            Name = "Company Sales",
            Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
            Description = null,
            ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
            VirtualPath = null,
            Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile("Test AW Reports\\2005\\Company Sales.rdl"))
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


        [TestFixtureSetUp]
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

            importer = kernel.Get<ReportItemImporter>();
        }

        [TestFixtureTearDown]
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
        /// Tests importing a ReportItem that exists on disk and will import successfully.
        /// </summary>
        [Test]
        public void ImportItem()
        {
            string filename = Path.Combine(this.GetTestDataPath(), reportItemFilename);

            ImportStatus status = null;

            ReportItem actual = importer.ImportItem(filename, out status);

            Assert.NotNull(actual);
            Assert.NotNull(actual.Definition);
            Assert.IsTrue(status.Success);
            Assert.AreEqual(expectedReportItem.Name, actual.Name);
            Assert.AreEqual(expectedReportItem.Path, actual.Path);
        }

        /// <summary>
        /// Tests importing a report item that does not exist on disk.
        /// </summary>
        [Test]
        public void ImportItem_NotFound()
        {
            string filename = Path.Combine(this.GetTestDataPath(), reportItem_NotFound_Filename);

            ImportStatus status = null;

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                delegate
                {
                    importer.ImportItem(filename, out status);
                });

            Assert.That(ex.Message, Is.EqualTo(filename));
        }

        /// <summary>
        /// Tests importing a report item that is in a path that does not contain the 'Export' token in the path.
        /// Items in these paths cannot be imported because the SSRS path cannt be parsed from it.
        /// </summary>
        [Test]
        public void ImportItem_NoToken()
        {
            string filename = Path.Combine(this.GetTestDataPath(), reportItem_NoToken_Filename);

            ImportStatus status = null;

            ReportItem actual = importer.ImportItem(filename, out status);

            Assert.Null(actual);
            Assert.IsFalse(status.Success);
            Assert.AreEqual(string.Format("Item's filename '{0}' does not contain token '\\Export\\'.",
                        filename),
                status.Error.Message);
        }
        #endregion
    }
}
