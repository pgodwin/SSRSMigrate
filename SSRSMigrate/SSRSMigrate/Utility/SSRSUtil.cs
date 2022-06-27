using System;
using System.Text;
using SSRSMigrate.Enum;
using SSRSMigrate.ReportServer2005;
using System.Xml;
using System.Text.RegularExpressions;
using SSRSMigrate.SSRS;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Utility
{
    public static class SSRSUtil
    {
        /// <summary>
        /// Updates the report definition by setting the new report server url, poiting Data Sources and Sub Reports to the new destination path.
        /// </summary>
        /// <param name="destinationReportServerUrl">The destination report server URL.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="destinationPath">The destination path.</param>
        /// <param name="reportDefinition">The report definition.</param>
        /// <returns>Returns the report report definition after it is updated using the new report server url and paths.</returns>
        /// <exception cref="System.ArgumentException">
        /// destinationReportServerUrl
        /// or
        /// sourcePath
        /// or
        /// destinationPath
        /// </exception>
        /// <exception cref="System.ArgumentNullException">reportDefinition</exception>
        public static byte[] UpdateDefinitionPaths(string destinationReportServerUrl, string sourcePath, string destinationPath, byte[] reportDefinition)
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
        /// <param name="sourcePath">The path on the source server.</param>
        /// <param name="destinationPath">The path on the destination server.</param>
        /// <param name="itemPath">The complete item path on the source server.</param>
        /// <returns>Returns the path to the item as it would be on the destination server, using the same folder structure.</returns>
        /// <exception cref="System.ArgumentException">
        /// sourcePath
        /// or
        /// destinationPath
        /// or
        /// itemPath
        /// </exception>
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

            // Get the item's path, up until the item name, which would be the current parent path of the item
            string path = string.Empty;

            // For some reason, sometimes people design a report that references a DataSource without a complete path (e.g. 'DataSource' instead of '/Folder/DataSource').
            // When we encounter this, we cannot exactly change them from the source to the destination path, so we'll just leave them as is
            if (!itemPath.StartsWith("/") && !itemPath.Contains("/"))
            {
                return itemPath;
            }

            // Handle item paths that are at the root in SSRS (e.g. /Report)
            if (itemPath.LastIndexOf("/") == 0)
            {
                path = itemPath.Substring(0, itemPath.LastIndexOf("/") + 1);
            }
            else 
            {
                path = itemPath.Substring(0, itemPath.LastIndexOf("/"));
            }

            // Get the item's name, this would be everything after the last / in the itemPath
            string name = itemPath.Substring(itemPath.LastIndexOf("/") + 1);

            // Replace the source path with the destination path, in the current path of the item to get the destination path
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(sourcePath, RegexOptions.IgnoreCase);

            string itemDestPath = string.Empty;

            if (sourcePath.Equals("/") && !path.EndsWith("/"))
            {
                itemDestPath = re.Replace(path, destinationPath + "/", 1);
            }
            else
            {
                itemDestPath = re.Replace(path, destinationPath, 1);
            }

            // In some situations the destination path no longer ends in /, so we add it.
            // This is a kludge and I should refactor this entire method, but not today...
            if (!itemDestPath.EndsWith("/"))
            {
                itemDestPath += "/";
            }

            itemDestPath += name;

            return itemDestPath;
        }

        public static SqlServerInfo GetSqlServerVersion(ReportingService2005 reportServerConnection)
        {
            if (reportServerConnection == null)
                throw new ArgumentNullException("reportServerConnection");

            reportServerConnection.ListSecureMethods();

            return GetSqlServerVersion(reportServerConnection.ServerInfoHeaderValue.ReportServerVersion);
        }

        /// <summary>
        /// Gets the SQL server version from the Reporting Services version string.
        /// </summary>
        /// <param name="versionText">The Reporting Services version string.</param>
        /// <returns></returns>
        public static SqlServerInfo GetSqlServerVersion(string versionText)
        {
            // Reference: https://sqlserverbuilds.blogspot.com/
            // Example: Microsoft SQL Server Reporting Services Version 16.01.200.8

            Regex oVersionRe = new Regex(@"Microsoft SQL Server Reporting Services Version (?<version>[0-9]+\.[0-9]+)\.(?<subver>[0-9]*\.*[0-9]*)");
            Match oMatch = oVersionRe.Match(versionText);

            var sqlServerInfo = new SqlServerInfo();

            if (oMatch.Success)
            {
                string sVersion = oMatch.Groups["version"].ToString();
                string sSubVersion = oMatch.Groups["subver"].ToString();

                sqlServerInfo.Version = sVersion;
                sqlServerInfo.FullVersion = versionText;
                sqlServerInfo.SubVersion = sSubVersion;

                Console.WriteLine(sVersion);

                switch (sVersion)
                {
                    case "7.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer7;
                        break;
                    case "8.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2000;
                        break;
                    case "9.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2005;
                        break;
                    case "10.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2008;
                        break;
                    case "10.50":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2008R2;
                        break;
                    case "11.0":
                    case "11.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2012;
                        break;
                    case "12.0":
                    case "12.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2014;
                        break;
                    case "13.0":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2016;
                        break;
                    case "14.0":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2017;
                        break;
                    case "15.0":
                    case "15.00":
                        sqlServerInfo.SsrsVersion =  SSRSVersion.SqlServer2019;
                        break;
                    default:
                        sqlServerInfo.SsrsVersion = SSRSVersion.Unknown;
                        break;
                }
            }
            else
            {
                sqlServerInfo.SsrsVersion = SSRSVersion.Unknown;
            }

            return sqlServerInfo;
        }

        /// <summary>
        /// Converts a string to a byte array.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string text)
        {
            //char[] charArray = text.ToCharArray();

            //byte[] byteArray = new byte[charArray.Length];

            //for (int i = 0; i < charArray.Length; i++)
            //{
            //    byteArray[i] = Convert.ToByte(charArray[i]);
            //}

            byte[] byteArray = Encoding.UTF8.GetBytes(text);

            return byteArray;
        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="byteArray">The byte array.</param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        public static string GetServerPathToPhysicalPath(string serverPath, string fileExtension = null)
        {
            if (string.IsNullOrEmpty(serverPath))
                throw new ArgumentException("serverPath");

            string physicalPath = serverPath.Replace('/', '\\');

            if (physicalPath.EndsWith("\\"))
                physicalPath = physicalPath.Substring(0, physicalPath.LastIndexOf('\\'));

            if (fileExtension != null)
                
                physicalPath += string.Format(".{0}", fileExtension);

            return physicalPath;
        }
    }
}
