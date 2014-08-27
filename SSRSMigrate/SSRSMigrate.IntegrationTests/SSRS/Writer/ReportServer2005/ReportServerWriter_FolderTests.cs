using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Writer;

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
        ReportServerWriter writer = null;

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

        #region WriteFolder Tests
        #endregion

        #region WriteFolders Tests
        #endregion
    }
}
