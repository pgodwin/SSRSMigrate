using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using SSRSMigrate.Wrappers;
using System.Security.Cryptography;
using System.IO;
using SSRSMigrate.SSRS.Item;

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

    public class ZipBundler : IBundler
    {
        private readonly IZipFileWrapper mZipFileWrapper = null;
        private readonly ICheckSumGenerator mCheckSumGenerator = null;
        private Dictionary<string, List<BundleSummaryEntry>> mEntries = null;

        public ZipBundler(IZipFileWrapper zipFileWrapper, ICheckSumGenerator checkSumGenerator)
        {
            if (zipFileWrapper == null)
                throw new ArgumentException("zipFileWrapper");

            if (checkSumGenerator == null)
                throw new ArgumentException("checkSumGenerator");

            this.mZipFileWrapper = zipFileWrapper;
            this.mCheckSumGenerator = checkSumGenerator;
            this.mEntries = new Dictionary<string, List<BundleSummaryEntry>>();
        }

        ~ZipBundler()
        {
            this.mZipFileWrapper.Dispose();
        }

        public BundleSummaryEntry CreateEntrySummary(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentException("item");

            throw new NotImplementedException();
        }

        public void AddItem(string key, string itemFileName, string itemPath)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");

            if (string.IsNullOrEmpty(itemFileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            //TODO Take itemFileName and convert it to a path like this "Export\\SSRSMigrate_AW_Tests\\Data Sources\\AWDataSource.json"
        }

        public string CreateSummary()
        {
            throw new NotImplementedException();
        }

        public string Save(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            throw new NotImplementedException();
        }
    }
}
