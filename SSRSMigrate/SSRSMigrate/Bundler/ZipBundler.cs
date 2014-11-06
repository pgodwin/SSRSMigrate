using System;
using System.Collections.Generic;
using Ninject.Extensions.Logging;
using SSRSMigrate.Enum;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Bundler
{
    //Sample Export Zip Archive Structure:
    //
    //  SSRSMigrate_AW_Tests.zip
    //  |-- Export\
    //  |   |-- SSRSMigrate_AW_Tests\
    //  |       |-- Data Sources\
    //  |       |   |-- AWDataSource.json
    //  |	    |   |-- Test Data Source.json
    //  |       |
    //  |	    |-- Reports\
    //  |	    |   |-- Company Sales.rdl
    //  |	    |   |-- Sales Order Details.rdl
    //  |	    |   |-- Store Contacts.rdl
    //  |	    |
    //  |	    |-- Sub Folder\
    //  |
    //  |-- ExportSummary.json
    //
    //ExportSummary.json will contain a list of everything exported and the MD5 checksum of that file:
    //
    //{
    //    "SourceRootPath": "/SSRSMigrate_AW_Tests",
    //    "SourceVersion": "SqlServer2008R2",
    //    "Entries": {
    //    "DataSources": [
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Data Sources",
    //        "FileName": "AWDataSource.json",
    //        "CheckSum": "7b4e44d94590f501ba24cd3904a925c3"
    //        }
    //    ],
    //    "Reports": [
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports",
    //        "FileName": "Company Sales.rdl",
    //        "CheckSum": "1adde7720ca2f0af49550fc676f70804"
    //        },
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports",
    //        "FileName": "Sales Order Detail.rdl",
    //        "CheckSum": "640a2f60207f03779fdedfed71d8101d"
    //        },
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports",
    //        "FileName": "Store Contacts.rdl",
    //        "CheckSum": "a225b92ed8475e6bc5b59f5b2cc396fa"
    //        }
    //    ],
    //    "Folders": [
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Data Sources",
    //        "FileName": "",
    //        "CheckSum": ""
    //        },
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports",
    //        "FileName": "",
    //        "CheckSum": ""
    //        },
    //        {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Sub Folder",
    //        "FileName": "",
    //        "CheckSum": ""
    //        }
    //    ]
    //    }
    //}

    /// <summary>
    /// Bundles exported Report Server data in a zip archive that can be imported later.
    /// </summary>
    public class ZipBundler : IBundler
    {
        private readonly IZipFileWrapper mZipFileWrapper = null;
        private readonly ICheckSumGenerator mCheckSumGenerator = null;
        private readonly ISerializeWrapper mSerializeWrapper = null;
        private BundleSummary mSummary = null;
        private readonly ILogger mLogger = null;

        private string mExportSummaryFilename = "ExportSummary.json";

        public string ExportSummaryFilename
        {
            get { return this.mExportSummaryFilename; }
        }

        public BundleSummary Summary
        {
            get { return this.mSummary;  }
        }

        public ZipBundler(
            IZipFileWrapper zipFileWrapper, 
            ICheckSumGenerator checkSumGenerator, 
            ILogger logger,
            ISerializeWrapper serializeWrapper)
        {
            if (zipFileWrapper == null)
                throw new ArgumentNullException("zipFileWrapper");

            if (checkSumGenerator == null)
                throw new ArgumentNullException("checkSumGenerator");

            if (logger == null)
                throw new ArgumentNullException("logger");

            if (serializeWrapper == null)
                throw new ArgumentNullException("serializeWrapper");

            this.mZipFileWrapper = zipFileWrapper;
            this.mCheckSumGenerator = checkSumGenerator;
            this.mLogger = logger;
            this.mSerializeWrapper = serializeWrapper;

            this.mSummary = new BundleSummary();
        }

        ~ZipBundler()
        {
            if (this.mZipFileWrapper != null)
                this.mZipFileWrapper.Dispose();
        }

        /// <summary>
        /// Gets the path to the file/folder as it will be stored in the zip archive, without filename.
        /// </summary>
        /// <param name="itemFileName">Name of the item file.</param>
        /// <param name="itemPath">The item path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// itemFileName
        /// or
        /// itemPath
        /// </exception>
        public string GetZipPath(string itemFileName, string itemPath, bool isFolder = false)
        {
            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("itemFileName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            this.mLogger.Trace("GetZipPath - itemFileName = {0}; itemPath = {1}; isFolder = {2}",
                itemFileName,
                itemPath,
                isFolder);

            // Replace / with \ in the item's path so we can get the path for the file in the summary
            string summaryPathPart = itemPath.Replace("/", "\\");

            // If the itemFileName does not contain the itemPath, throw an exception
            //  because we can't parse the path for the summary
            if (!itemFileName.Contains(summaryPathPart))
                throw new Exception(string.Format("Item path '{0}' is invalid.", itemPath));

            summaryPathPart = itemFileName.Substring(itemFileName.LastIndexOf(summaryPathPart));

            // If the item is not a folder, parse up until the last \ in order to get the full path
            if (!isFolder)
                summaryPathPart = summaryPathPart.Substring(0, summaryPathPart.LastIndexOf("\\"));

            string summaryFullPath = string.Format("Export{0}", summaryPathPart);

            this.mLogger.Trace("GetZipPath - Returns = {0}", summaryFullPath);

            return summaryFullPath;
        }

        public BundleSummaryEntry CreateEntrySummary(string itemFileName, string zipPath, bool isFolder = false)
        {
            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("itemFileName");

            if (string.IsNullOrEmpty(zipPath))
                throw new ArgumentException("zipPath");

            this.mLogger.Trace("CreateEntrySummary - itemFileName = {0}; zipPath = {1}; isFolder",
                itemFileName,
                zipPath,
                isFolder);

            string fileName = "";

            // If the item is not a Folder, parse the fileName for the BundleSummaryEntry
            if (!isFolder)
                fileName = itemFileName.Substring(itemFileName.LastIndexOf("\\") + 1);

            BundleSummaryEntry entry = new BundleSummaryEntry()
            {
                CheckSum = this.mCheckSumGenerator.CreateCheckSum(itemFileName),
                FileName = fileName,
                Path = zipPath
            };

            this.mLogger.Trace("CreateEntrySummary - FileName = {0}; Path = {1}; CheckSum = {2}", 
                entry.FileName,
                entry.Path,
                entry.CheckSum);

            return entry;
        }

        public void AddItem(string key, string itemFileName, string itemPath, bool isFolder)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");

            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("itemFileName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            if (!this.mSummary.Entries.ContainsKey(key))
                throw new KeyNotFoundException(key);

            this.mLogger.Debug("AddItem - key = {0}; itemFileName = {1}; itemPath = {2}; isFolder = {3}",
                key,
                itemFileName,
                itemPath,
                isFolder);

            // Get the path for inside the zip archive
            string zipPath = this.GetZipPath(itemFileName, itemPath, isFolder);

            // If it is a folder, add the folder to the archive, otherwise add file
            if (isFolder)
                this.mZipFileWrapper.AddDirectory(itemFileName, zipPath);
            else
                this.mZipFileWrapper.AddFile(itemFileName, zipPath);

            BundleSummaryEntry entry = this.CreateEntrySummary(itemFileName, zipPath, isFolder);

            this.mSummary.Entries[key].Add(entry);      
        }

        /// <summary>
        /// Creates a JSON string that contains a summary of the data created by the ZipBundler.
        /// </summary>
        /// <param name="rootPath">The root path of what is being exported.</param>
        /// <param name="version">The version of SQL Server where the items are being exported from.</param>
        /// <returns>String in JSON format containing the summary of data contained in the zip archive.</returns>
        public string CreateSummary(string rootPath, SSRSVersion version)
        {
            this.mSummary.SourceRootPath = rootPath;
            this.mSummary.SourceVersion = version;

            // Serialize mEntries Dictionary to JSON format
            string summary = this.mSerializeWrapper.SerializeObject(this.mSummary);

            this.mLogger.Trace("CreateSummary - JSON = {0}", summary);
            this.mLogger.Trace("CreateSummary - Saving summary as '{0}'...", this.mExportSummaryFilename);

            // Add JSON serialized summary string as an entry to the zip using the value from mExportSummaryFilename 
            this.mZipFileWrapper.AddEntry(this.mExportSummaryFilename, summary);

            return summary;
        }

        public string Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            this.mLogger.Debug("Save - fileName = {0}", fileName);

            this.mZipFileWrapper.Save(fileName, true);

            return fileName;
        }

        /// <summary>
        /// Resets this instance by resetting the underlying IZipFileWrapper object.
        /// </summary>
        public void Reset()
        {
            // This is done to prevent entries from being duplicated in the archive 
            //  if an export is ran multiple times in the same form instance.
            this.mZipFileWrapper.Reset();
        }
    }
}
