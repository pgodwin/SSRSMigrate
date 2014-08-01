using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using System.IO;
using System.Xml;

namespace SSRSMigrate.Exporter
{
    public class ReportItemExporter : IItemExporter<ReportItem>
    {
        public ReportItemExporter()
        {
        }

        public ExportStatus SaveItem(ReportItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                if (File.Exists(fileName) && !overwrite)
                    throw new IOException(string.Format("File '{0}' already exists.", fileName));
                else if (File.Exists(fileName) && overwrite)
                    File.Delete(fileName);

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                XmlDocument doc = new XmlDocument();

                using (MemoryStream oStream = new MemoryStream(item.Definition))
                {
                    doc.Load(oStream);
                    doc.Save(fileName);
                }

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
