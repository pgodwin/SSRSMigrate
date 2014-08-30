using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using Moq;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.TestHelper;

namespace SSRSMigrate.Tests.SSRS.Writer
{
    [TestFixture]
    class ReportServerWriter_DataSourceTests
    {
        ReportServerWriter writer = null;

        #region Data Source Items
        DataSourceItem dataSourceItem = null;
        DataSourceItem dataSourceTwoItem = null;
        DataSourceItem invalidDataSourcePathItem = null;
        DataSourceItem alreadyExistsDataSourceItem = null;
        DataSourceItem errorDataSourceItem = null;
        List<DataSourceItem> dataSourceItems = null;
        #endregion

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            dataSourceItem = new DataSourceItem()
            {
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
                Description = null,
                ID = Guid.NewGuid().ToString(),
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
                Size = 414,
                VirtualPath = null,
                Name = "AWDataSource",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/AWDataSource",
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
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

            dataSourceTwoItem = new DataSourceItem()
            {
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                Description = null,
                ID = Guid.NewGuid().ToString(),
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                Size = 414,
                VirtualPath = null,
                Name = "Test Data Source",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
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

            alreadyExistsDataSourceItem = new DataSourceItem()
            {
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                Description = null,
                ID = Guid.NewGuid().ToString(),
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                Size = 414,
                VirtualPath = null,
                Name = "Already Exists Data Source",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Already Exists Data Source",
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
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

            invalidDataSourcePathItem = new DataSourceItem()
            {
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                Description = null,
                ID = Guid.NewGuid().ToString(),
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/23/2014 8:29:25 AM"),
                Size = 414,
                VirtualPath = null,
                Name = "Test.Data Source",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Test.Data Source",
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
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

            errorDataSourceItem = new DataSourceItem()
            {
                CreatedBy = "DOMAIN\\user",
                CreationDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
                Description = null,
                ID = Guid.NewGuid().ToString(),
                ModifiedBy = "DOMAIN\\user",
                ModifiedDate = DateTime.Parse("7/23/2014 8:28:43 AM"),
                Size = 414,
                VirtualPath = null,
                Name = "ErrorDataSource",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/ErrorDataSource",
                ConnectString = "Data Source=(local)\\SQL2008;Initial Catalog=AdventureWorks2008",
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

            dataSourceItems = new List<DataSourceItem>()
            {
                dataSourceItem,
                dataSourceTwoItem
            };

            // Setup IReportServerRepository mock
            var reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerRepository.WriteDataSource
            reportServerRepositoryMock.Setup(r => r.WriteDataSource(null, It.IsAny<DataSourceItem>()))
                .Throws(new ArgumentException("dataSourcePath"));

            reportServerRepositoryMock.Setup(r => r.WriteDataSource("", It.IsAny<DataSourceItem>()))
                .Throws(new ArgumentException("dataSourcePath"));

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(It.IsAny<string>(), null))
                .Throws(new ArgumentException("dataSource"));

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceItem), dataSourceItem))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceTwoItem), dataSourceTwoItem))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(alreadyExistsDataSourceItem), alreadyExistsDataSourceItem))
                .Throws(new ItemAlreadyExistsException(string.Format("The data source '{0}' already exists.", alreadyExistsDataSourceItem.Path)));

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(errorDataSourceItem), errorDataSourceItem))
                .Returns(() => string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path));

            // Setup IReportServerRepository.ValidatePath Mocks
            reportServerRepositoryMock.Setup(r => r.ValidatePath(dataSourceItem.Path))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(dataSourceTwoItem.Path))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(alreadyExistsDataSourceItem.Path))
               .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath("/SSRSMigrate_AW_Tests"))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(errorDataSourceItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(null))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(""))
               .Returns(() => false);

            reportServerRepositoryMock.Setup(r => r.ValidatePath(It.Is<string>(s => Regex.IsMatch(s ?? "", "[:?;@&=+$,\\*><|.\"]+") == true)))
               .Returns(() => false);

            writer = new ReportServerWriter(reportServerRepositoryMock.Object);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        #region WriteDataSource Tests
        [Test]
        public void WriteDataSource()
        {
            string actual = writer.WriteDataSource(dataSourceItem);

            Assert.Null(actual);
        }

        [Test]
        public void WriteDataSource_AlreadyExists()
        {
            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSource(alreadyExistsDataSourceItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The data source '{0}' already exists.", alreadyExistsDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSource_NullDataSourceItem()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteDataSource(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: dataSourceItem"));
        }

        [Test]
        public void WriteDataSource_InvalidDataSourcePath()
        {
            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(invalidDataSourcePathItem);
                });

            Assert.That(ex.Message, Is.StringContaining(string.Format("Invalid path '{0}'.", invalidDataSourcePathItem.Path)));
        }

        [Test]
        public void WriteDatasource_DataSourceItemNullName()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = null,
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemEmptyName()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
            };

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemNullPath()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = null,
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemEmptyPath()
        {
            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = "",
            };

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(dataSource);
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemError()
        {
            string actual = writer.WriteDataSource(errorDataSourceItem);

            Assert.AreEqual(string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path), actual);
        }
        #endregion

        #region WriteDataSources Tests
        [Test]
        public void WriteDataSources()
        {
            string[] actual = writer.WriteDataSources(dataSourceItems.ToArray());

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void WriteDataSources_OneOrMoreAlreadyExists()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(alreadyExistsDataSourceItem);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("The data source '{0}' already exists.", alreadyExistsDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSources_NullDataSourceItems()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
                delegate
                {
                    writer.WriteDataSources(null);
                });

            Assert.That(ex.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: dataSourceItems"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreInvalidDataSourcePath()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();
            items.AddRange(dataSourceItems);
            items.Add(invalidDataSourcePathItem);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.StringContaining(string.Format("Invalid path '{0}'.", invalidDataSourcePathItem.Path)));
        
        }

        [Test]
        public void WriteDatasources_OneOrMoreDataSourceItemNullName()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(new DataSourceItem()
            {
                Name = null,
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
            });

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyName()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(new DataSourceItem()
            {
                Name = "",
                Path = "/SSRSMigrate_AW_Tests/Data Sources/Test Data Source",
            });

            ArgumentException ex = Assert.Throws<ArgumentException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo("item.Name"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemNullPath()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = null,
            });

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyPath()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = "",
            });

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.StringContaining("Invalid path"));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemError()
        {
            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(errorDataSourceItem);

            string[] actual = writer.WriteDataSources(items.ToArray());

            Assert.NotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path), actual[0]);
        }
        #endregion
    }
}
