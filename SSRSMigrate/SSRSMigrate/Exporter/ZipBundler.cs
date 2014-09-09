using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using SSRSMigrate.Wrappers;
using System.Security.Cryptography;
using System.IO;
using SSRSMigrate.SSRS.Item;
using Newtonsoft.Json;

namespace SSRSMigrate.Exporter
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
    //    "DataSources": [
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json",
    //        "CheckSum": "7b4e44d94590f501ba24cd3904a925c3"
    //    },
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Data Sources\\Test Data Source.json",
    //        "CheckSum": "c0815114c3ce9dde35eca314bbfe4bc9"
    //    }
    //    ],
    //    "Folders": [
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests",
    //        "CheckSum": ""
    //    },
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Data Sources",
    //        "CheckSum": ""
    //    },
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports",
    //        "CheckSum": ""
    //    },
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Sub Folder",
    //        "CheckSum": ""
    //    }
    //    ],
    //    "Reports": [
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports\\Company Sales.rdl",
    //        "CheckSum": "1adde7720ca2f0af49550fc676f70804"
    //    },
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports\\Sales Order Detail.rdl",
    //        "CheckSum": "640a2f60207f03779fdedfed71d8101d"
    //    },
    //    {
    //        "Path": "Export\\SSRSMigrate_AW_Tests\\Reports\\Store Contacts.rdl",
    //        "CheckSum": "a225b92ed8475e6bc5b59f5b2cc396fa"
    //    }
    //    ]
    //}

    /// <summary>
    /// Bundles exported Report Server data in a zip archive that can be imported later.
    /// </summary>
    public class ZipBundler : IBundler
    {
        private readonly IZipFileWrapper mZipFileWrapper = null;
        private readonly ICheckSumGenerator mCheckSumGenerator = null;
        private Dictionary<string, List<BundleSummaryEntry>> mEntries = null;

        private string mExportSummaryFilename = "ExportSummary.json";

        public string ExportSummaryFilename
        {
            get { return this.mExportSummaryFilename; }
        }

        public Dictionary<string, List<BundleSummaryEntry>> Entries
        {
            get { return this.mEntries; }
        }

        public ZipBundler(IZipFileWrapper zipFileWrapper, ICheckSumGenerator checkSumGenerator)
        {
            if (zipFileWrapper == null)
                throw new ArgumentNullException("zipFileWrapper");

            if (checkSumGenerator == null)
                throw new ArgumentNullException("checkSumGenerator");

            this.mZipFileWrapper = zipFileWrapper;
            this.mCheckSumGenerator = checkSumGenerator;

            // Create entries Dictionary with default keys
            this.mEntries = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
			    { "DataSources", new List<BundleSummaryEntry>() },
			    { "Reports", new List<BundleSummaryEntry>() },
			    { "Folders", new List<BundleSummaryEntry>() }
		    };
        }

        ~ZipBundler()
        {
            if (this.mZipFileWrapper != null)
                this.mZipFileWrapper.Dispose();
        }

        /// <summary>
        /// Gets the path to the file/folder as it will be stored in the zip archive.
        /// </summary>
        /// <param name="itemFileName">Name of the item file.</param>
        /// <param name="itemPath">The item path.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// itemFileName
        /// or
        /// itemPath
        /// </exception>
        public string GetZipPath(string itemFileName, string itemPath)
        {
            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("itemFileName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            // Replace / with \ in the item's path so we can get the path for the file in the summary
            string summaryPathPart = itemPath.Replace("/", "\\");

            // If the itemFileName does not contain the itemPath, throw an exception
            //  because we can't parse the path for the summary
            if (!itemFileName.Contains(summaryPathPart))
                throw new Exception(string.Format("Item path '{0}' is invalid.", itemPath));

            summaryPathPart = itemFileName.Substring(itemFileName.LastIndexOf(summaryPathPart));

            string summaryFullPath = string.Format("Export{0}", summaryPathPart);

            return summaryFullPath;
        }

        public BundleSummaryEntry CreateEntrySummary(string itemFileName, string zipPath)
        {
            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("itemFileName");

            if (string.IsNullOrEmpty(zipPath))
                throw new ArgumentException("zipPath");

            BundleSummaryEntry entry = new BundleSummaryEntry()
            {
                CheckSum = this.mCheckSumGenerator.CreateCheckSum(itemFileName),
                Path = zipPath
            };

            return entry;
        }

        public void AddItem(string key, string itemFileName, string itemPath, bool isFolder)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");

            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            // Get the path for inside the zip archive
            string zipPath = this.GetZipPath(itemFileName, itemPath);

            // If it is a folder, add the folder to the archive, otherwise add file
            if (isFolder)
                this.mZipFileWrapper.AddDirectory(itemFileName, zipPath);
            else
                this.mZipFileWrapper.AddFile(itemFileName, zipPath);

            BundleSummaryEntry entry = this.CreateEntrySummary(itemFileName, zipPath);

            this.mEntries[key].Add(entry);
        }

        public string CreateSummary()
        {
            //string summary = JsonConvert.SerializeObject(this.mEntries, Formatting.Indented);

            //this.mZipFileWrapper.AddEntry(this.mExportSummaryFilename, summary);

            //return summary;

            throw new NotImplementedException();
        }

        public string Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            this.mZipFileWrapper.Save(fileName, true);

            return fileName;
        }
    }
}
