using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;
using SSRSMigrate.Bundler.Events;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Bundler
{
    public class ZipBundleReader : IBundleReader
    {
        private readonly IZipFileWrapper mZipFileWrapper = null;
        private readonly ICheckSumGenerator mCheckSumGenerator = null;
        private Dictionary<string, List<BundleSummaryEntry>> mEntries = null;
        private readonly ILogger mLogger = null;
        private readonly string mFileName = null;

        private string mExportSummaryFilename = "ExportSummary.json";

        #region Public Properties
        public string ExportSummaryFilename
        {
            get { return this.mExportSummaryFilename; }
        }
        #endregion

        #region Events
        public event FolderReadEventHandler OnFolderRead;
        public event DataSourceReadEventHandler OnDataSourceRead;
        public event ReportReadEventHandler OnReportRead;
        #endregion

        public Dictionary<string, List<BundleSummaryEntry>> Entries
        {
            get { return this.mEntries; }
        }

        public ZipBundleReader(
            string fileNameName,
            IZipFileWrapper zipFileWrapper, 
            ICheckSumGenerator checkSumGenerator, 
            ILogger logger)
        {
            if (string.IsNullOrEmpty(fileNameName))
                throw new ArgumentException("fileNameName");

            if (zipFileWrapper == null)
                throw new ArgumentNullException("zipFileWrapper");

            if (checkSumGenerator == null)
                throw new ArgumentNullException("checkSumGenerator");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mFileName = fileNameName;
            this.mZipFileWrapper = zipFileWrapper;
            this.mCheckSumGenerator = checkSumGenerator;
            this.mLogger = logger;

            // Create entries Dictionary with default keys
            this.mEntries = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
			    { "DataSources", new List<BundleSummaryEntry>() },
			    { "Reports", new List<BundleSummaryEntry>() },
			    { "Folders", new List<BundleSummaryEntry>() }
		    };
        }

        ~ZipBundleReader()
        {
            if (this.mZipFileWrapper != null)
                this.mZipFileWrapper.Dispose();
        }
    }
}
