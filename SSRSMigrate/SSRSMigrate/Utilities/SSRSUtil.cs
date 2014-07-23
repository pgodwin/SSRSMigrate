using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Enums;
using SSRSMigrate.ReportServer2005;
using SSRSMigrate.SSRS;
using System.Xml;

namespace SSRSMigrate.Utilities
{
    public static class SSRSUtil
    {
        public static byte[] UpdateReportDefinition(string destinationReportServerUrl, string sourcePath, string destinationPath, byte[] reportDefinition)
        {
            if (string.IsNullOrEmpty(destinationReportServerUrl))
                throw new ArgumentException("destinationReportServerUrl");

            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("sourcePath");

            if (string.IsNullOrEmpty(destinationPath))
                throw new ArgumentException("destinationPath");

            if (reportDefinition == null)
                throw new ArgumentNullException("reportDefinition");

            var doc = new XmlDocument();
            UTF8Encoding decoder = new UTF8Encoding();
            string reportDefinitionString = decoder.GetString(reportDefinition);

            if (reportDefinitionString.Substring(0, 1) != "<")
                reportDefinitionString = reportDefinitionString.Substring(1, reportDefinitionString.Length - 1);

            doc.LoadXml(reportDefinitionString);

            // Update DataSourceReference tag to new path
            XmlNodeList dataSourceNodes = doc.GetElementsByTagName("DataSourceReference");

            for (int i = 0; i < dataSourceNodes.Count; i++)
                dataSourceNodes[i].InnerText = GetFullDestinationPathForItem(
                    sourcePath,
                    destinationPath,
                    dataSourceNodes[i].InnerText);

            // Update ReportServerUrl tag with new url
            XmlNodeList reportServerUrlNodes = doc.GetElementsByTagName("ReportServerUrl", "*");
            if (reportServerUrlNodes.Count > 0)
                for (int i = 0; i < reportServerUrlNodes.Count; i++)
                    reportServerUrlNodes[i].InnerText = destinationReportServerUrl;

            // Update SubReports to new path
            XmlNodeList subReportsNode = doc.GetElementsByTagName("Subreport");

            for (int i = 0; i < subReportsNode.Count; i++)
            {
                for (int j = 0; j < subReportsNode[i].ChildNodes.Count; j++)
                {
                    if (subReportsNode[i].ChildNodes[j].Name == "ReportName")
                    {
                        subReportsNode[i].ChildNodes[j].InnerText = GetFullDestinationPathForItem(
                            sourcePath, 
                            destinationPath, 
                            subReportsNode[i].ChildNodes[j].InnerText);
                    }
                }
            }

            return Encoding.UTF8.GetBytes(doc.OuterXml);
        }

        /// <summary>
        /// Gets the full destination path for an item by taking the path to the item on the source server and
        /// replacing the root path on the server server with the root path on the destination server.
        /// </summary>
        /// <param name="sourcePath">The path on the source server, including server url.</param>
        /// <param name="destinationPath">The path on the destination server, including server url.</param>
        /// <param name="itemPath">The complete item path on the source server.</param>
        /// <returns>Returns the path to the item as it would be on the destination server, using the same folder structure.</returns>
        public static string GetFullDestinationPathForItem(string sourcePath, string destinationPath, string itemPath)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentException("sourcePath");

            if (string.IsNullOrEmpty(destinationPath))
                throw new ArgumentException("destinationPath");

            if (string.IsNullOrEmpty(itemPath))
                throw new ArgumentException("itemPath");

            if (sourcePath != "/")
                if (sourcePath.EndsWith("/"))
                    sourcePath = sourcePath.Substring(0, sourcePath.LastIndexOf("/"));

            if (destinationPath != "/")
                if (destinationPath.EndsWith("/"))
                    destinationPath = destinationPath.Substring(0, destinationPath.LastIndexOf("/"));

            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(sourcePath);
            string itemDestPath = re.Replace(itemPath, destinationPath, 1);

            return itemDestPath;
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

        public static byte[] TextToByteArray(string text)
        {
            char[] CharArray = text.ToCharArray();

            byte[] ByteArray = new byte[CharArray.Length];

            for (int i = 0; i < CharArray.Length; i++)
            {
                ByteArray[i] = Convert.ToByte(CharArray[i]);
            }

            return ByteArray;
        }
    }
}
