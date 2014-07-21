using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Enums;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.SSRS;

namespace SSRSMigrate.Utilities
{
    public static class SSRSUtil
    {
        public static byte[] UpdateReportDefinition(byte[] reportDefinition, string reportServerAddress)
        {
            throw new NotImplementedException();
        }

        public static string GetFullDestinationPath(string itemPath, string sourcePath, string destinationPath)
        {
            throw new NotImplementedException();
        }

        public static SSRSVersion GetSqlServerVersion(ReportingService2005 reportServerConnection)
        {
            throw new NotImplementedException();
        }

        public static SSRSVersion GetSqlServerVersion(string versionText)
        {
            throw new NotImplementedException();
        }

        public static ReportItem CatalogItemToReportItem(CatalogItem item)
        {
            throw new NotImplementedException();
        }
    }
}
