using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Activation;
using SSRSMigrate.Bundler;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.ReportServer2010;
using SSRSMigrate.ReportServer2005;
using Ninject.Modules;
using System.Net;
using Ninject.Parameters;
using SSRSMigrate.Factory;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Exporter;
using SSRSMigrate.DataMapper;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate
{
    public class ReportServerRepositoryModule : NinjectModule
    {
        public override void Load()
        {
            // Bind source IReportServerRepository
            this.Bind<IReportServerRepository>()
               .ToProvider<SourceReportServer2005RepositoryProvider>()
               .Named("2005-SRC");

            this.Bind<IReportServerRepository>()
               .ToProvider<SourceReportServer2010RepositoryProvider>()
               .Named("2010-SRC");

            // Bind destination IReportServerRepository
            this.Bind<IReportServerRepository>()
               .ToProvider<DestinationReportServer2005RepositoryProvider>()
               .Named("2005-DEST");

            this.Bind<IReportServerRepository>()
               .ToProvider<DestinationReportServer2010RepositoryProvider>()
               .Named("2010-DEST");

            // Bind IReportServerRepositoryFactory
            this.Bind<IReportServerRepositoryFactory>().To<ReportServerRepositoryFactory>();

            // Bind IReportServerReaderFactory
            this.Bind<IReportServerReaderFactory>()
                .To<ReportServerReaderFactory>();

            // Bind IReportServerWriterFactory
            this.Bind<IReportServerWriterFactory>()
                .To<ReportServerWriterFactory>();

            // Bind IReportServerReader
            this.Bind<IReportServerReader>()
                .To<ReportServerReader>()
                .Named("2005-SRC");

            this.Bind<IReportServerReader>()
                .To<ReportServerReader>()
                .Named("2010-SRC");

            // Bind IReportServerWriter
            this.Bind<IReportServerWriter>()
                .To<ReportServerWriter>()
                .Named("2005-DEST");

            this.Bind<IReportServerWriter>()
                .To<ReportServerWriter>()
                .Named("2010-DEST");

            this.Bind<IFileSystem>().To<FileSystem>();

            this.Bind<ISerializeWrapper>().To<JsonConvertWrapper>();

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

            // Bind IZipFileReaderWrapper
            this.Bind<IZipFileReaderWrapper>().To<ZipFileReaderWrapper>();

            // Bind ICheckSumGenerator
            this.Bind<ICheckSumGenerator>().To<MD5CheckSumGenerator>();
            
            // Bind IBundleReader
            this.Bind<IBundleReader>().To<ZipBundleReader>()
                .WithConstructorArgument("fileName", GetImportZipFileName)
                .WithConstructorArgument("unpackDirectory", GetImportZipUnpackDirectory);
        }

        private string GetImportZipFileName(IContext context)
        {
            return Properties.Settings.Default.ImportZipFileName;
        }

        private string GetImportZipUnpackDirectory(IContext context)
        {
            return Properties.Settings.Default.ImportZipUnpackDir;
        }
    }

    public class SourceReportServer2005RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.SrcWebServiceUrl;
            string version = Properties.Settings.Default.SrcVersion;
            bool defaultCred = Properties.Settings.Default.SrcDefaultCred;
            string username = Properties.Settings.Default.SrcUsername;
            string password = Properties.Settings.Default.SrcPassword;
            string domain = Properties.Settings.Default.SrcDomain;
            string path = Properties.Settings.Default.SrcPath;

            if (!url.EndsWith("reportservice2005.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2005.asmx", url);

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                //service.UseDefaultCredentials = false;
            }

            return new ReportServer2005Repository(path, service, new ReportingService2005DataMapper());
        }
    }

    public class SourceReportServer2010RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.SrcWebServiceUrl;
            string version = Properties.Settings.Default.SrcVersion;
            bool defaultCred = Properties.Settings.Default.SrcDefaultCred;
            string username = Properties.Settings.Default.SrcUsername;
            string password = Properties.Settings.Default.SrcPassword;
            string domain = Properties.Settings.Default.SrcDomain;
            string path = Properties.Settings.Default.SrcPath;

            if (!url.EndsWith("reportservice2010.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2010.asmx", url);

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                //service.UseDefaultCredentials = false;
            }

            return new ReportServer2010Repository(path, service, new ReportingService2010DataMapper());
        }
    }

    public class DestinationReportServer2005RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.DestWebServiceUrl;
            string version = Properties.Settings.Default.DestVersion;
            bool defaultCred = Properties.Settings.Default.DestDefaultCred;
            string username = Properties.Settings.Default.DestUsername;
            string password = Properties.Settings.Default.DestPassword;
            string domain = Properties.Settings.Default.DestDomain;
            string path = Properties.Settings.Default.DestPath;

            if (!url.EndsWith("reportservice2005.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2005.asmx", url);

            ReportingService2005 service = new ReportingService2005();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                //service.UseDefaultCredentials = false;
            }

            return new ReportServer2005Repository(path, service, new ReportingService2005DataMapper());
        }
    }

    public class DestinationReportServer2010RepositoryProvider : Provider<IReportServerRepository>
    {
        protected override IReportServerRepository CreateInstance(IContext context)
        {
            string url = Properties.Settings.Default.DestWebServiceUrl;
            string version = Properties.Settings.Default.DestVersion;
            bool defaultCred = Properties.Settings.Default.DestDefaultCred;
            string username = Properties.Settings.Default.DestUsername;
            string password = Properties.Settings.Default.DestPassword;
            string domain = Properties.Settings.Default.DestDomain;
            string path = Properties.Settings.Default.DestPath;

            if (!url.EndsWith("reportservice2010.asmx"))
                if (url.EndsWith("/"))
                    url = url.Substring(0, url.Length - 1);

            url = string.Format("{0}/reportservice2010.asmx", url);

            ReportingService2010 service = new ReportingService2010();
            service.Url = url;

            if (defaultCred)
            {
                service.Credentials = CredentialCache.DefaultNetworkCredentials;
                service.PreAuthenticate = true;
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(
                    username,
                    password,
                    domain);

                //service.UseDefaultCredentials = false;
            }

            return new ReportServer2010Repository(path, service, new ReportingService2010DataMapper());
        }
    }

}
