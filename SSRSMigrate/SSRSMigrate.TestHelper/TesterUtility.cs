using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using SSRSMigrate.SSRS.Item;

public class CoverageExcludeAttribute : System.Attribute { }

namespace SSRSMigrate.TestHelper
{
    [CoverageExcludeAttribute]
    public static class TesterUtility
    {
        public static string LoadRDLFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("file");

            if (!File.Exists(file))
                throw new FileNotFoundException("RDL not found", file);

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;

            string content = File.ReadAllText(file);

            if (content.Substring(0, 1) != "<")
                content = content.Substring(1, content.Length - 1);

            doc.LoadXml(content.Trim());

            return doc.InnerXml;
        }

        public static byte[] StringToByteArray(string text)
        {
            char[] charArray = text.ToCharArray();

            byte[] byteArray = new byte[charArray.Length];

            for (int i = 0; i < charArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(charArray[i]);
            }

            return byteArray;
        }

        public static string ByteArrayToString(byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        /// <summary>
        /// Compares the text file file1 with the text file file2.
        /// </summary>
        /// <param name="file1">The file1.</param>
        /// <param name="file2">The file2.</param>
        /// <returns>Returns true if both files match.</returns>
        /// <exception cref="System.ArgumentException">
        /// file1
        /// or
        /// file2
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// </exception>
        public static bool CompareTextFiles(string file1, string file2)
        {
            if (string.IsNullOrEmpty(file1))
                throw new ArgumentException("file1");

            if (string.IsNullOrEmpty(file2))
                throw new ArgumentException("file2");

            if (!File.Exists(file1))
                throw new FileNotFoundException(file1);

            if (!File.Exists(file2))
                throw new FileNotFoundException(file2);

            bool result = false;

            string[] file1_lines = File.ReadAllLines(file1);
            string[] file2_lines = File.ReadAllLines(file2);

            IEnumerable<string> file2_only = file2_lines.Except(file1_lines);

            return !file2_only.Any();
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
                parentPath = path.Replace(name, "");

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

        public static string GetParentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");

            if (!path.StartsWith("/"))
                path = "/" + path;

            string name = path.Substring(path.LastIndexOf("/") + 1);

            string parentPath = null;
            if (path == "/")
                parentPath = path;
            else
            {
                parentPath = path.Substring(0, path.LastIndexOf("/"));

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
