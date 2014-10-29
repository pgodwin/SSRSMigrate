using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    class DataSourceItemImporter_Tests
    {
        private StandardKernel kernel = null;
        private DataSourceItemImporter importer = null;

        #region Test Data
        private string dataSourceItemFilename = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json";
        private string dataSourceItem_NotFound_FileName = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\NotFound.json";
        private string dataSourceItemFilename_DeserializeError_Filename = "Export\\SSRSMigrate_AW_Tests\\Data Sources\\SerializationError.json";
        #endregion

        #region Expected Data
        DataSourceItem expectedDataSourceItem = new DataSourceItem()
        {
            Description = null,
            VirtualPath = null,
            Name = "AWDataSource",
            Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource",
            ConnectString = "Data Source=(local);Initial Catalog=AdventureWorks2008",
            CredentialsRetrieval = "Integrated",
            Enabled = true,
            EnabledSpecified = true,
            Extension = "SQL",
            ImpersonateUser = false,
            ImpersonateUserSpecified = true,
            OriginalConnectStringExpressionBased = false,
            Password = null,
            Prompt = "Enter a user name and password to access the data source:",
            UseOriginalConnectString = false,
            UserName = null,
            WindowsCredentials = false
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

            importer = kernel.Get<DataSourceItemImporter>();
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
        /// Tests importing a data source item that exists on disk and will import successfully.
        /// </summary>
        [Test]
        public void ImportItem()
        {
            string filename = Path.Combine(this.GetTestDataPath(), dataSourceItemFilename);

            ImportStatus status = null;

            DataSourceItem actual = importer.ImportItem(filename, out status);

            Assert.NotNull(actual);
            Assert.NotNull(status);
            Assert.AreEqual(actual.Name, expectedDataSourceItem.Name);
            Assert.AreEqual(actual.Path, expectedDataSourceItem.Path);
            Assert.True(status.Success);
        }

        /// <summary>
        /// Tests importing a data source item that does not exist on disk.
        /// </summary>
        [Test]
        public void ImportItem_NotFound()
        {
            string filename = Path.Combine(this.GetTestDataPath(), dataSourceItem_NotFound_FileName);

            ImportStatus status = null;

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
               delegate
               {
                   importer.ImportItem(filename, out status);
               });

            Assert.That(ex.Message, Is.EqualTo(filename));
        }

        /// <summary>
        /// Tests importing a data source item where the item's content cannot be deserialized to an DataSourceItem object.
        /// </summary>
        [Test]
        public void ImportItem_DeserializeException()
        {
            string filename = Path.Combine(this.GetTestDataPath(), dataSourceItemFilename_DeserializeError_Filename);

            ImportStatus status = null;

            DataSourceItem actual = importer.ImportItem(filename, out status);

            Assert.IsNull(actual);
            Assert.IsFalse(status.Success);
            Assert.AreEqual("Unexpected end when deserializing object. Path 'Password', line 10, position 20.",
                status.Error.Message);
        }
        #endregion
    }
}
