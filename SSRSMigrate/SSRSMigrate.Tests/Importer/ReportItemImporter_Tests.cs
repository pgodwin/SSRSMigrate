using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Moq;
using NUnit.Framework;
using SSRSMigrate.Importer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;
using SSRSMigrate.TestHelper;
using SSRSMigrate.TestHelper.Logging;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Tests.Importer
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportItemImporter_Tests
    {
        private ReportItemImporter importer = null;
        private Mock<IFileSystem> fileSystemMock = null;
        private Mock<ISerializeWrapper> serializeWrapperMock = null;
        private MockLogger loggerMock = null;

        #region Expected Data
        // Expected successfull import item data
        private string expectedReportItemFileName = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl";
        private string expectedReportItemPath = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports";

        private ReportItem expectedReportItem = new ReportItem()
            {
                Name = "Company Sales",
                Path = "/SSRSMigrate_AW_Tests/Reports/Company Sales",
                Description = null,
                ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2005\\Company Sales.rdl")))
            };

        // Expected data for a Report Item that does not exist on disk
        private string expectedReportItem_NotFound_Filename = "C:\\temp\\SSRSMigrate_AW_Tests\\Export\\SSRSMigrate_AW_Tests\\Reports\\NotFound.rdl";

        // Expected data for a Report item that is in a path that does not contain a proper token.
        // These would be items in a path that is not a child of an 'Export' folder.
        private string expectedReportItem_NoToken_FileName = "C:\\temp\\SSRSMigrate_AW_Tests\\SSRSMigrate_AW_Tests\\Reports\\NoToken.rdl";
        private string expectedReportItemPath_NoToken = "C:\\temp\\SSRSMigrate_AW_Tests\\SSRSMigrate_AW_Tests\\Reports";

        private ReportItem expectedReportItem_NoToken = new ReportItem()
        {
            Name = "NoToken",
            Path = "/SSRSMigrate_AW_Tests/Reports/NoToken",
            Description = null,
            ID = "16d599e6-9c87-4ebc-b45b-5a47e3c73746",
            VirtualPath = null,
            Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "Test AW Reports\\2005\\Company Sales.rdl")))
        };
        #endregion

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            loggerMock = new MockLogger();
            fileSystemMock = new Mock<IFileSystem>();
            serializeWrapperMock = new Mock<ISerializeWrapper>();

            // ISystemIOWrapper.File.ReadAllBytes Method Mocks
            fileSystemMock.Setup(f => f.File.ReadAllBytes(expectedReportItemFileName))
                .Returns(() => expectedReportItem.Definition);

            fileSystemMock.Setup(f => f.File.ReadAllBytes(expectedReportItem_NoToken_FileName))
                .Returns(() => expectedReportItem_NoToken.Definition);

            // ISystemIOWrapper.File.Exists Method Mocks
            fileSystemMock.Setup(f => f.File.Exists(expectedReportItemFileName))
                .Returns(() => true);

            fileSystemMock.Setup(f => f.File.Exists(expectedReportItem_NotFound_Filename))
                .Returns(() => false);

            fileSystemMock.Setup(f => f.File.Exists(expectedReportItem_NoToken_FileName))
                .Returns(() => true);

            // ISystemIOWrapper.Path.GetDirectoryName Method Mocks
            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedReportItemFileName))
                .Returns(() => expectedReportItemPath);

            fileSystemMock.Setup(f => f.Path.GetDirectoryName(expectedReportItem_NoToken_FileName))
                .Returns(() => expectedReportItemPath_NoToken);

            // ISystemIOWrapper.Path.GetFileName Method Mocks
            fileSystemMock.Setup(f => f.Path.GetFileNameWithoutExtension(expectedReportItemFileName))
                .Returns(() => expectedReportItem.Name);

            fileSystemMock.Setup(f => f.Path.GetFileNameWithoutExtension(expectedReportItem_NoToken_FileName))
                .Returns(() => expectedReportItem_NoToken.Name);

            importer = new ReportItemImporter(fileSystemMock.Object, loggerMock);
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
        /// Tests importing a ReportItem that exists on disk and will import successfully.
        /// </summary>
        [Test]
        public void ImportItem()
        {
            ImportStatus status = null;

            ReportItem actual = importer.ImportItem(expectedReportItemFileName, out status);

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
            ImportStatus status = null;

            FileNotFoundException ex = Assert.Throws<FileNotFoundException>(
                delegate
                {
                    importer.ImportItem(expectedReportItem_NotFound_Filename, out status);
                });

            Assert.That(ex.Message, Is.EqualTo(expectedReportItem_NotFound_Filename));
        }

        /// <summary>
        /// Tests importing a report item that is in a path that does not contain the 'Export' token in the path.
        /// Items in these paths cannot be imported because the SSRS path cannt be parsed from it.
        /// </summary>
        [Test]
        public void ImportItem_NoToken()
        {
            ImportStatus status = null;

            ReportItem actual = importer.ImportItem(expectedReportItem_NoToken_FileName, out status);

            Assert.Null(actual);
            Assert.IsFalse(status.Success);
            Assert.AreEqual(string.Format("Item's filename '{0}' does not contain token '\\Export\\'.",
                        expectedReportItem_NoToken_FileName),
                status.Error.Message);
        }
        #endregion
    }
}
