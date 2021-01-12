using NUnit.Framework;
using SSRSMigrate.SSRS.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.Tests.SSRS.Item
{

    [TestFixture, Category("good")]
    [CoverageExcludeAttribute]
    class ReportServerItem_GetParentPathTests
    {
        #region SSRSUtil.GetParentPath Tests
        [Test]
        public void GetParentPath()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Sub Folder",
                Path = "/SSRSMigrate/Reports/Sub Folder"
            };

            string expected = "/SSRSMigrate/Reports";

            string actual = item.ParentPath;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetParentPath_ParentContainsName()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Reports",
                Path = "/SSRSMigrate/Reports/Reports"
            };

            string expected = "/SSRSMigrate/Reports";

            string actual = item.ParentPath;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetParentPath_PathIsSlash()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Sub Folder",
                Path = "/"
            };

            string expected = "/";

            string actual = item.ParentPath;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetParentPath_PathEndsWithSlash()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Sub Folder",
                Path = "/SSRSMigrate/Reports/Sub Folder/"
            };

            string expected = "/SSRSMigrate/Reports";

            string actual = item.ParentPath;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetParentPath_PathMissingFirstSlash()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Sub Folder",
                Path = "SSRSMigrate/Reports/Sub Folder"
            };

            string expected = "/SSRSMigrate/Reports";

            string actual = item.ParentPath;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetParentPath_NullName()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = null,
                Path = "/SSRSMigrate/Reports/Sub Folder"
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    string actual = item.ParentPath;
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void GetParentPath_EmptyName()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "",
                Path = "/SSRSMigrate/Reports/Sub Folder"
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    string actual = item.ParentPath;
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void GetParentPath_NullPath()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Sub Folder",
                Path = null
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    string actual = item.ParentPath;
                });

            Assert.That(ex.Message, Is.EqualTo("item.Path"));
        }

        [Test]
        public void GetParentPath_EmptyPath()
        {
            ReportServerItem item = new ReportServerItem()
            {
                Name = "Sub Folder",
                Path = ""
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    string actual = item.ParentPath;
                });

            Assert.That(ex.Message, Is.EqualTo("item.Path"));
        }
        #endregion
    }
}
