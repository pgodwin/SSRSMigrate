using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject.Activation;
using SSRSMigrate.ReportServer2005;
using System.Net;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Exporter;
using SSRSMigrate.Factory;
using SSRSMigrate.DataMapper;
using SSRSMigrate.TestHelper.Logging;
using Ninject.Extensions.Logging;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.IntegrationTests
{
    [CoverageExcludeAttribute]
    public class DependencyModule : NinjectModule
    {
        public override void  Load()
        {
            // Bind source IReportServerRepository
            this.Bind<IReportServerRepository>()
                .ToProvider<SourceReportServer2005RepositoryProvider>()
                .InSingletonScope()
                .Named("2005-SRC");

            this.Bind<IReportServerRepository>()
                .ToProvider<SourceReportServer2010RepositoryProvider>()
                .InSingletonScope()
                .Named("2010-SRC");

            // Bind destination IReportServerRepository
            this.Bind<IReportServerRepository>()
                .ToProvider<DestinationReportServer2005RepositoryProvider>()
                .InSingletonScope()
                .Named("2005-DEST");

            this.Bind<IReportServerRepository>()
                .ToProvider<DestinationReportServer2010RepositoryProvider>()
                .InSingletonScope()
                .Named("2010-DEST");

            // Bind IReportServerRepositoryFactory
            this.Bind<IReportServerRepositoryFactory>()
                .To<ReportServerRepositoryFactory>();

            // Bind IReportServerReaderFactory
            this.Bind<IReportServerReaderFactory>()
                .To<ReportServerReaderFactory>();

            // Bind IReportServerWriterFactory
            this.Bind<IReportServerWriterFactory>()
                .To<ReportServerWriterFactory>();

            // Bind IReportServerReader
            this.Bind<IReportServerReader>()
                .To<ReportServerReader>()
                .InSingletonScope()
                .Named("2005-SRC");

            this.Bind<IReportServerReader>()
                .To<ReportServerReader>()
                .InSingletonScope()
                .Named("2010-SRC");

            // Bind IReportServerWriter
            this.Bind<IReportServerWriter>()
                .To<ReportServerWriter>()
                .InSingletonScope()
                .Named("2005-DEST");

            this.Bind<IReportServerWriter>()
                .To<ReportServerWriter>()
                .InSingletonScope()
                .Named("2010-DEST");

            // Bind IExportWriter
            this.Bind<IExportWriter>().To<FileExportWriter>().WhenInjectedExactlyInto<DataSourceItemExporter>();
            this.Bind<IExportWriter>().To<FileExportWriter>().WhenInjectedExactlyInto<ReportItemExporter>();
            this.Bind<IExportWriter>().To<FolderExportWriter>().WhenInjectedExactlyInto<FolderItemExporter>();

            // Bind IItemExporter
            this.Bind(typeof(IItemExporter<>)).To(typeof(ReportItemExporter));
            this.Bind(typeof(IItemExporter<>)).To(typeof(FolderItemExporter));
            this.Bind(typeof(IItemExporter<>)).To(typeof(DataSourceItemExporter));

            // Bind IBundler
            this.Bind<IBundler>().To<ZipBundler>();

            // Bind IZipFileWrapper
            this.Bind<IZipFileWrapper>().To<ZipFileWrapper>();

            // Bind ICheckSumGenerator
            this.Bind<ICheckSumGenerator>().To<MD5CheckSumGenerator>();
        }
    }

    [CoverageExcludeAttribute]
    public class SourceReportServer2005RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.ReportServer2008WebServiceUrl;
            string path = Properties.Settings.Default.SourcePath;

            if (!url.EndsWith("reportservice2005.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2005.asmx", url);
            }

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2005Repository(path, service, new ReportingService2005DataMapper());
        }
    }

    [CoverageExcludeAttribute]
    public class SourceReportServer2010RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.ReportServer2008R2WebServiceUrl;
            string path = Properties.Settings.Default.SourcePath;

            if (!url.EndsWith("reportservice2010.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2010.asmx", url);
            }

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2010Repository(path, service,  new ReportingService2010DataMapper());
        }
    }

    [CoverageExcludeAttribute]
    public class DestinationReportServer2005RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.ReportServer2008WebServiceUrl;
            string path = Properties.Settings.Default.DestinationPath;

            if (!url.EndsWith("reportservice2005.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2005.asmx", url);
            }

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2005Repository(path, service, new ReportingService2005DataMapper());
        }
    }

    [CoverageExcludeAttribute]
    public class DestinationReportServer2010RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.ReportServer2008R2WebServiceUrl;
            string path = Properties.Settings.Default.DestinationPath;

            if (!url.EndsWith("reportservice2010.asmx"))
            {
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

                url = string.Format("{0}/reportservice2010.asmx", url);
            }

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            service.Credentials = CredentialCache.DefaultNetworkCredentials;
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = true;

            return new ReportServer2010Repository(path, service, new ReportingService2010DataMapper());
        }
    }
}
