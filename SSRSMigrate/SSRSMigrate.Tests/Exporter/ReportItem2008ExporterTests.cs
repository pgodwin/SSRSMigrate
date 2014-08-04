using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.Exporter;
using SSRSMigrate.Exporter.Writer;
using Moq;
using SSRSMigrate.SSRS.Item;
using System.IO;
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.Tests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportItem2008ExporterTests
    {
        ReportItemExporter exporter = null;
        Mock<IExportWriter> exportWriterMock = null;
        ReportItem reportItem;
        string testReportPath = "Test Reports\\2008\\Inquiry.rdl";

        #region Expected Values
        string expectedReportItemFileName = "C:\\temp\\SSRSMigrate_Tests\\Reports\\Inquiry.rdl";
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            reportItem = new ReportItem()
            {
                Name = "Inquiry",
                Path = "/SSRSMigrate_Tests/Reports/Inquiry",
                Description = null,
                ID = "5921480a-1b24-4a6e-abbc-f8db116cd24e",
                VirtualPath = null,
                Definition = TesterUtility.StringToByteArray(TesterUtility.LoadRDLFile(testReportPath))
            };

            exportWriterMock = new Mock<IExportWriter>();

            exportWriterMock.Setup(e => e.Save(It.IsAny<string>(), It.IsAny<byte[]>(), true));

            // Mock IExporter.Save where the filename exists but overwrite = false
            exportWriterMock.Setup(e => e.Save(expectedReportItemFileName, It.IsAny<byte[]>(), false))
                .Throws(new IOException(string.Format("File '{0}' already exists.", expectedReportItemFileName)));

            exporter = new ReportItemExporter(exportWriterMock.Object);
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

        #region SaveItem Tests
        [Test]
        public void SaveItem()
        {
            ExportStatus actualStatus = exporter.SaveItem(reportItem, expectedReportItemFileName, true);

            exportWriterMock.Verify(e => e.Save(expectedReportItemFileName, reportItem.Definition, true));

            Assert.True(actualStatus.Success);
            Assert.AreEqual(expectedReportItemFileName, actualStatus.ToPath);
            Assert.AreEqual(reportItem.Path, actualStatus.FromPath);
        }

        [Test]
        public void SaveItem_FileExists_DontOverwrite()
        {
            ExportStatus actualStatus = exporter.SaveItem(reportItem, expectedReportItemFileName, false);

            Assert.False(actualStatus.Success);
            Assert.NotNull(actualStatus.Errors);
            Assert.AreEqual(actualStatus.Errors[0], string.Format("File '{0}' already exists.", expectedReportItemFileName));
        }

        [Test]
        public void SaveItem_NullItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
               delegate
               {
                   exporter.SaveItem(null, expectedReportItemFileName);
               });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: item"));
        }

        [Test]
        public void SaveItem_NullFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    exporter.SaveItem(reportItem, null);
                });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }

        [Test]
        public void SaveItem_EmptyFileName()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(
               delegate
               {
                   exporter.SaveItem(reportItem, "");
               });

            Assert.That(ex.Message, Is.EqualTo("fileName"));
        }


        #endregion
    }
}
