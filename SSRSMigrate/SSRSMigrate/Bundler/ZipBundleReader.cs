using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using SSRSMigrate.Bundler.Events;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Bundler
{
    public class ZipBundleReader : IBundleReader
    {
        private IZipFileReaderWrapper mZipFileReaderWrapper = null;
        private readonly ICheckSumGenerator mCheckSumGenerator = null;
        private Dictionary<string, List<BundleSummaryEntry>> mEntries = null;
        private readonly ILogger mLogger = null;
        private readonly IFileSystem mFileSystem = null;
        private readonly string mFileName = null;
        private readonly string mUnpackDirectory = null;

        private string mExportSummaryFilename = "ExportSummary.json";

        #region Public Properties
        public string ExportSummaryFilename
        {
            get { return this.mExportSummaryFilename; }
        }

        public Dictionary<string, List<BundleSummaryEntry>> Entries
        {
            get { return this.mEntries; }
        }
        #endregion

        #region Events
        public event FolderReadEventHandler OnFolderRead;
        public event DataSourceReadEventHandler OnDataSourceRead;
        public event ReportReadEventHandler OnReportRead;
        #endregion

        public ZipBundleReader(
            string fileName,
            string unpackDirectory,
            IZipFileReaderWrapper zipFileReaderWrapper, 
            ICheckSumGenerator checkSumGenerator, 
            ILogger logger,
            IFileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(unpackDirectory))
                throw new ArgumentException("unpackDirectory");

            if (zipFileReaderWrapper == null)
                throw new ArgumentNullException("zipFileReaderWrapper");

            if (checkSumGenerator == null)
                throw new ArgumentNullException("checkSumGenerator");

            if (logger == null)
                throw new ArgumentNullException("logger");

            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            this.mFileName = fileName;
            this.mUnpackDirectory = unpackDirectory;
            this.mZipFileReaderWrapper = zipFileReaderWrapper;
            this.mZipFileReaderWrapper.FileName = fileName;
            this.mCheckSumGenerator = checkSumGenerator;
            this.mLogger = logger;
            this.mFileSystem = fileSystem;
            this.mZipFileReaderWrapper.UnPackDirectory = unpackDirectory;
            this.mZipFileReaderWrapper.FileName = fileName;

            // Register event for when entries are extracted using the IZipFileReaderWrapper
            this.mZipFileReaderWrapper.OnEntryExtracted += EntryExtractedEventHandler;

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
            if (this.mZipFileReaderWrapper != null)
            {
                this.mZipFileReaderWrapper.OnEntryExtracted -= EntryExtractedEventHandler;
                this.mZipFileReaderWrapper.Dispose();
            }
        }

        public string Extract()
        {
            return this.mZipFileReaderWrapper.UnPack();
        }

        public void ReadExportSummary()
        {
            this.mLogger.Debug("ReadExportSummary - Reading summary from entry '{0}' in bundle '{1}'...", this.mExportSummaryFilename, this.mFileName);

            string exportSummary = this.mZipFileReaderWrapper.ReadEntry(this.mExportSummaryFilename);
 
            if (string.IsNullOrEmpty(exportSummary))
                throw new Exception("No data in export summary.");

            this.mLogger.Debug("ReadExportSummary - Summary = {0}", exportSummary);

            this.mEntries = JsonConvert.DeserializeObject<Dictionary<string, List<BundleSummaryEntry>>>(exportSummary);
        }

        public void Read()
        {
            // Go through each folder entry
            this.mLogger.Debug("Read - Started reading folder entries...");
            foreach (BundleSummaryEntry folder in this.mEntries["Folders"])
            {
                string fileName = Path.Combine(this.mUnpackDirectory, folder.Path);

                this.mLogger.Trace("Read - Reading folder... Filename = {0}; Path = {1}; Checksum = {2}",
                    fileName,
                    folder.Path,
                    folder.CheckSum);

                bool success = true;
                List<string> errors = new List<string>();
                string checkSum = this.mCheckSumGenerator.CreateCheckSum(fileName);

                // Check if the folder exists on disk
                if (!this.mFileSystem.Directory.Exists(fileName))
                {
                    success = false;
                    errors.Add(string.Format("Directory does not exist '{0}'.", fileName));
                }

                // Check if the checksums match (folder.CheckSum should be "")
                if (!folder.CheckSum.Equals(checkSum) || folder.CheckSum != "")
                {
                    success = false;
                    errors.Add(string.Format("Source checksum of '{0}' does not match checksum of extracted entry '{1}'.",
                        folder.CheckSum,
                        checkSum));
                }

                if (errors.Any())
                    this.mLogger.Error("Read - Errors reading folder... {0}",
                        string.Join("; ", errors.ToArray()));

                ItemReadEvent readEvent = new ItemReadEvent(
                    fileName,
                    folder.Path,
                    true,
                    folder.CheckSum,
                    success,
                    errors.ToArray());

                if (this.OnFolderRead != null)
                {
                    this.mLogger.Debug("Read - Triggering OnFolderRead event for entry '{0}'...", fileName);

                    this.OnFolderRead(this, readEvent);
                }
            }
            this.mLogger.Debug("Read - Finished reading folder entries.");

            // Go through each data source entry
            this.mLogger.Debug("Read - Started reading data source entries...");
            foreach (BundleSummaryEntry dataSource in this.mEntries["DataSources"])
            {
                string fileName = Path.Combine(Path.Combine(this.mUnpackDirectory, dataSource.Path), dataSource.FileName);

                this.mLogger.Trace("Read - Reading data source... Filename = {0}; Path = {1}; Checksum = {2}",
                    dataSource.FileName,
                    dataSource.Path,
                    dataSource.CheckSum);

                bool success = true;
                List<string> errors = new List<string>();
                string checkSum = this.mCheckSumGenerator.CreateCheckSum(fileName);

                // Check if the file exists on disk
                if (!this.mFileSystem.File.Exists(fileName))
                {
                    success = false;
                    errors.Add(string.Format("File does not exist '{0}'.", fileName));
                }

                // Check if the checksums match
                if (!dataSource.CheckSum.Equals(checkSum))
                {
                    success = false;
                    errors.Add(string.Format("Source checksum of '{0}' does not match checksum of extracted entry '{1}'.",
                        dataSource.CheckSum,
                        checkSum));
                }

                if (errors.Any())
                    this.mLogger.Error("Read - Errors reading data source... {0}",
                        string.Join("; ", errors.ToArray()));

                ItemReadEvent readEvent = new ItemReadEvent(
                    fileName,
                    dataSource.Path,
                    false,
                    dataSource.CheckSum,
                    success,
                    errors.ToArray());

                if (this.OnDataSourceRead != null)
                {
                    this.mLogger.Debug("Read - Triggering OnDataSourceRead event for entry '{0}'...", fileName);

                    this.OnDataSourceRead(this, readEvent);
                }
            }
            this.mLogger.Debug("Read - Finished reading folder entries.");

            // Go through each report entry
            foreach (BundleSummaryEntry report in this.mEntries["Reports"])
            {
                string fileName = Path.Combine(Path.Combine(this.mUnpackDirectory, report.Path), report.FileName);

                this.mLogger.Trace("Read - Reading report... Filename = {0}; Path = {1}; Checksum = {2}",
                    fileName,
                    report.Path,
                    report.CheckSum);

                bool success = true;
                List<string> errors = new List<string>();
                string checkSum = this.mCheckSumGenerator.CreateCheckSum(fileName);

                // Check if the file exists on disk
                if (!this.mFileSystem.File.Exists(fileName))
                {
                    success = false;
                    errors.Add(string.Format("File does not exist '{0}'.", fileName));
                }

                // Check if the checksums match
                if (!report.CheckSum.Equals(checkSum))
                {
                    success = false;
                    errors.Add(string.Format("Source checksum of '{0}' does not match checksum of extracted entry '{1}'.",
                        report.CheckSum,
                        checkSum));
                }

                if (errors.Any())
                    this.mLogger.Error("Read - Errors reading report... {0}",
                        string.Join("; ", errors.ToArray()));

                ItemReadEvent readEvent = new ItemReadEvent(
                    fileName,
                    report.Path,
                    false,
                    report.CheckSum,
                    success,
                    errors.ToArray());

                if (this.OnReportRead != null)
                {
                    this.mLogger.Debug("Read - Triggering OnReportRead event for entry '{0}'...", fileName);

                    this.OnReportRead(this, readEvent);
                }
            }
        }

        private void EntryExtractedEventHandler(IZipFileReaderWrapper sender, ZipEntryReadEvent e)
        {
            this.mLogger.Debug("EntryExtractedEventHandler - Entry '{0}' extracted to '{1}'...", e.FileName, e.ExtractedTo);
        }
    }
}
