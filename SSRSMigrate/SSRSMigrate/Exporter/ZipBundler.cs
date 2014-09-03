using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using SSRSMigrate.Wrappers;

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

    public class ZipBundler : IZipBundler
    {
        private readonly IZipFileWrapper mZipFileWrapper = null;

        public ZipBundler(IZipFileWrapper zipFileWrapper)
        {
            if (zipFileWrapper == null)
                throw new ArgumentException("zipFileWrapper");

            this.mZipFileWrapper = zipFileWrapper;
        }

        ~ZipBundler()
        {
            this.mZipFileWrapper.Dispose();
        }
    }
}
