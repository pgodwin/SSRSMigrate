using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Repository;
using Moq;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.SSRS.Validators;
using SSRSMigrate.TestHelper;
using SSRSMigrate.TestHelper.Logging;

namespace SSRSMigrate.Tests.SSRS.Writer
{
    [TestFixture]
    class ReportServerWriter_DataSourceTests
    {
        private ReportServerWriter writer = null;
        private Mock<IReportServerPathValidator> pathValidatorMock = null;
        private Mock<IReportServerRepository> reportServerRepositoryMock = null;

        #region Data Source Items
        DataSourceItem dataSourceItem = null;
        DataSourceItem dataSourceTwoItem = null;
        DataSourceItem invalidDataSourcePathItem = null;
        DataSourceItem alreadyExistsDataSourceItem = null;
        DataSourceItem errorDataSourceItem = null;
        List<DataSourceItem> dataSourceItems = null;
        #endregion

        [OneTimeSetUp]
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

            // Setup IReportServerRepository.WriteDataSource
            //reportServerRepositoryMock.Setup(r => r.WriteDataSource(null, It.IsAny<DataSourceItem>(), It.IsAny<bool>()))
            //    .Throws(new ArgumentException("dataSourcePath"));

            //reportServerRepositoryMock.Setup(r => r.WriteDataSource("", It.IsAny<DataSourceItem>(), It.IsAny<bool>()))
            //    .Throws(new ArgumentException("dataSourcePath"));

            //reportServerRepositoryMock.Setup(r => r.WriteDataSource(It.IsAny<string>(), null, It.IsAny<bool>()))
            //    .Throws(new ArgumentException("dataSource"));

            //reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceItem), dataSourceItem, It.IsAny<bool>()))
            //    .Returns(() => null);

            //reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceTwoItem), dataSourceTwoItem, It.IsAny<bool>()))
            //    .Returns(() => null);

            //reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(errorDataSourceItem), errorDataSourceItem, It.IsAny<bool>()))
            //    .Returns(() => string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path));

            //reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(alreadyExistsDataSourceItem), alreadyExistsDataSourceItem, true))
            //    .Returns(() => null);

            //// Setup IReportServerRepository.ItemExists Mocks
            //reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsDataSourceItem.Path, "DataSource"))
            //    .Returns(() => true);

            //// Setup IReportserverValidator.Validate Mocks
            //pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
            //   .Returns(() => true);

            //pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
            //   .Returns(() => true);

            //pathValidatorMock.Setup(r => r.Validate(alreadyExistsDataSourceItem.Path))
            //   .Returns(() => true);

            //pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests"))
            //    .Returns(() => true);

            //pathValidatorMock.Setup(r => r.Validate(errorDataSourceItem.Path))
            //    .Returns(() => true);

            //pathValidatorMock.Setup(r => r.Validate(null))
            //   .Returns(() => false);

            //pathValidatorMock.Setup(r => r.Validate(""))
            //   .Returns(() => false);

            //pathValidatorMock.Setup(r => r.Validate(It.Is<string>(s => Regex.IsMatch(s ?? "", "[:?;@&=+$,\\*><|.\"]+") == true)))
            //   .Returns(() => false);
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            writer = null;
        }

        [SetUp]
        public void SetUp()
        {
            // Setup IReportServerRepository mock
            reportServerRepositoryMock = new Mock<IReportServerRepository>();

            // Setup IReportServerPathValidator mock
            pathValidatorMock = new Mock<IReportServerPathValidator>();

            MockLogger logger = new MockLogger();

            writer = new ReportServerWriter(reportServerRepositoryMock.Object, logger, pathValidatorMock.Object);

            writer.Overwrite = false; // Reset overwrite property
        }

        #region WriteDataSource Tests
        [Test]
        public void WriteDataSource()
        {
            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceItem), dataSourceItem, It.IsAny<bool>()))
                .Returns(() => null);

            string actual = writer.WriteDataSource(dataSourceItem);

            Assert.Null(actual);
        }

        [Test]
        public void WriteDataSource_AlreadyExists_OverwriteDisallowed()
        {
            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsDataSourceItem.Path, "DataSource"))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsDataSourceItem.Path))
                .Returns(() => true);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSource(alreadyExistsDataSourceItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsDataSourceItem.Path)));
        }

        [Test]
        public void WriteDataSource_AlreadyExists_OverwriteAllowed()
        {
            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsDataSourceItem.Path, "DataSource"))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsDataSourceItem.Path))
                .Returns(() => true);

            writer.Overwrite = true; // Allow overwriting of data source

            string actual = writer.WriteDataSource(alreadyExistsDataSourceItem);

            Assert.Null(actual);
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
            pathValidatorMock.Setup(r => r.Validate(invalidDataSourcePathItem.Path))
                .Returns(() => false);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSource(invalidDataSourcePathItem);
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidDataSourcePathItem.Path)));
        }

        [Test]
        public void WriteDatasource_DataSourceItemNullName()
        {
            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Data Sources/Test Data Source"))
                .Returns(() => true);

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
            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Data Sources/Test Data Source"))
                .Returns(() => true);

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
            pathValidatorMock.Setup(r => r.Validate(null))
                .Returns(() => false);

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

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", dataSource.Path)));
        }

        [Test]
        public void WriteDataSource_DataSourceItemEmptyPath()
        {
            pathValidatorMock.Setup(r => r.Validate(""))
                .Returns(() => false);

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

            Assert.That(ex.Message, Does.Contain("Invalid path"));
        }

        [Test]
        public void WriteDataSource_DataSourceItemError()
        {
            pathValidatorMock.Setup(r => r.Validate(errorDataSourceItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(errorDataSourceItem), errorDataSourceItem, It.IsAny<bool>()))
                .Returns(() => string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path));

            string actual = writer.WriteDataSource(errorDataSourceItem);

            Assert.AreEqual(string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path), actual);
        }
        #endregion

        #region WriteDataSources Tests
        [Test]
        public void WriteDataSources()
        {
            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceTwoItem), dataSourceTwoItem, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceItem), dataSourceItem, It.IsAny<bool>()))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
                .Returns(() => true);

            string[] actual = writer.WriteDataSources(dataSourceItems.ToArray());

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void WriteDataSources_OneOrMoreAlreadyExists()
        {
            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceTwoItem), dataSourceTwoItem, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceItem), dataSourceItem, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(alreadyExistsDataSourceItem), alreadyExistsDataSourceItem, true))
                .Returns(() => null);

            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(alreadyExistsDataSourceItem.Path))
                .Returns(() => true);

            reportServerRepositoryMock.Setup(r => r.ItemExists(alreadyExistsDataSourceItem.Path, "DataSource"))
                .Returns(() => true);

            List<DataSourceItem> items = new List<DataSourceItem>();

            items.AddRange(dataSourceItems);
            items.Add(alreadyExistsDataSourceItem);

            ItemAlreadyExistsException ex = Assert.Throws<ItemAlreadyExistsException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("An item at '{0}' already exists.", alreadyExistsDataSourceItem.Path)));
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
            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceTwoItem), dataSourceTwoItem, It.IsAny<bool>()))
                .Returns(() => null);

            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(dataSourceItem), dataSourceItem, It.IsAny<bool>()))
                .Returns(() => null);
            
            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(invalidDataSourcePathItem.Path))
                .Returns(() => false);

            List<DataSourceItem> items = new List<DataSourceItem>();
            items.AddRange(dataSourceItems);
            items.Add(invalidDataSourcePathItem);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", invalidDataSourcePathItem.Path)));
        
        }

        [Test]
        public void WriteDatasources_OneOrMoreDataSourceItemNullName()
        {
            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Data Sources/Test Data Source"))
                .Returns(() => true);

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
            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate("/SSRSMigrate_AW_Tests/Data Sources/Test Data Source"))
                .Returns(() => true);

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
            pathValidatorMock.Setup(r => r.Validate(null))
                .Returns(() => false);

            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = null,
            };

            List<DataSourceItem> items = new List<DataSourceItem>()
            {
                dataSource
            };

            items.AddRange(dataSourceItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", dataSource.Path)));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemEmptyPath()
        {
            pathValidatorMock.Setup(r => r.Validate(""))
                .Returns(() => false);

            DataSourceItem dataSource = new DataSourceItem()
            {
                Name = "Test Data Source",
                Path = "",
            };

            List<DataSourceItem> items = new List<DataSourceItem>()
            {
                dataSource
            };

            items.AddRange(dataSourceItems);

            InvalidPathException ex = Assert.Throws<InvalidPathException>(
                delegate
                {
                    writer.WriteDataSources(items.ToArray());
                });

            Assert.That(ex.Message, Is.EqualTo(string.Format("Invalid path '{0}'.", dataSource.Path)));
        }

        [Test]
        public void WriteDataSources_OneOrMoreDataSourceItemError()
        {
            reportServerRepositoryMock.Setup(r => r.WriteDataSource(TesterUtility.GetParentPath(errorDataSourceItem), errorDataSourceItem, It.IsAny<bool>()))
                .Returns(() => string.Format("Error writing data source '{0}': Error!", errorDataSourceItem.Path));

            pathValidatorMock.Setup(r => r.Validate(errorDataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceItem.Path))
                .Returns(() => true);

            pathValidatorMock.Setup(r => r.Validate(dataSourceTwoItem.Path))
                .Returns(() => true);

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
