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
            return null;
        }

        public static string GetFullDestinationPath(string itemPath, string sourcePath, string destinationPath)
        {
            return null;
        }

        public static SSRSVersion GetSqlServerVersion(ReportingService2005 reportServerConnection)
        {
            return SSRSVersion.Unknown;
        }

        public static SSRSVersion GetSqlServerVersion(string versionText)
        {
            return SSRSVersion.Unknown;
        }

        public static ReportItem CatalogItemToReportItem(CatalogItem item)
        {
            return null;
        }
    }
}
