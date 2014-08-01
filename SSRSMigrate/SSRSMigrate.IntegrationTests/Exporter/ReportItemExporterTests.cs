using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using SSRSMigrate.Exporter;

namespace SSRSMigrate.IntegrationTests.Exporter
{
    [TestFixture]
    [CoverageExcludeAttribute]
    class ReportItemExporterTests
    {
        ReportItemExporter exporter = null;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            EnvironmentSetup();

            exporter = new ReportItemExporter();
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

        }

        #region Environment Setup/TearDown
        private string GetOutPutPath()
        {
            string outputPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(outputPath, "ReportItemExporter_Output");
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
    }
}
