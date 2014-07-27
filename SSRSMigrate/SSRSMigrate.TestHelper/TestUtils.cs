using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace SSRSMigrate.TestHelper
{
    public static class TestUtils
    {
        public static string LoadRDLFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("file");

            if (!File.Exists(file))
                throw new FileNotFoundException("RDL not found", file);

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            return doc.InnerXml;
        }
    }
}
