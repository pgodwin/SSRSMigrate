using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Enum;
using SSRSMigrate.ReportServer2005;
using System.Xml;
using System.Text.RegularExpressions;
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
            string path = itemPath.Substring(0, itemPath.LastIndexOf("/"));
            string name = itemPath.Substring(itemPath.LastIndexOf("/"));

            // Replace the source path with the destination path, in the current path of the item to get the destination path
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(sourcePath);
            string itemDestPath = re.Replace(path, destinationPath, 1);

            itemDestPath += name;

            return itemDestPath;
        }

        public static SSRSVersion GetSqlServerVersion(ReportingService2005 reportServerConnection)
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
        public static SSRSVersion GetSqlServerVersion(string versionText)
        {
            Regex oVersionRE = new Regex(@"Microsoft SQL Server Reporting Services Version (?<version>[0-9]+\.[0-9]+)\.(?<subver>[0-9]*\.*[0-9]*)");
            Match oMatch = oVersionRE.Match(versionText);

            if (oMatch.Success)
            {
                string sVersion = oMatch.Groups["version"].ToString();
                string sSubVersion = oMatch.Groups["subver"].ToString();

                Console.WriteLine(sVersion);

                switch (sVersion)
                {
                    case "7.00":
                        return SSRSVersion.SqlServer7;
                    case "8.00":
                        return SSRSVersion.SqlServer2000;
                    case "9.00":
                        return SSRSVersion.SqlServer2005;
                    case "10.00":
                        return SSRSVersion.SqlServer2008;
                    case "10.50":
                        return SSRSVersion.SqlServer2008R2;
                    case "11.00":
                        return SSRSVersion.SqlServer2012;
                    default:
                        return SSRSVersion.Unknown;
                }
            }
            else
            {
                return SSRSVersion.Unknown;
            }
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

        public static string GetParentPath(ReportServerItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(item.Path))
                throw new ArgumentException("item.Path");

            if (string.IsNullOrEmpty(item.Name))
                throw new ArgumentException("item.Name");

            string path = item.Path;
            string name = item.Name;

            string parentPath = null;
            if (path == "/")
                parentPath = path;
            else
            {
                //parentPath = path.Replace(name, "");
                parentPath = path.Substring(0, path.LastIndexOf(name));

                if (parentPath.EndsWith("/"))
                {
                    parentPath = parentPath.Substring(0, parentPath.Length - 1);

                    if (parentPath.EndsWith("/"))
                        parentPath = parentPath.Substring(0, parentPath.Length - 1);
                }
            }

            if (!parentPath.StartsWith("/"))
                parentPath = "/" + parentPath;

            return parentPath;
        }
    }
}
