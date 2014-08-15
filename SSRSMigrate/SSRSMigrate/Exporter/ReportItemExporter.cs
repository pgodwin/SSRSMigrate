using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using System.IO;
using System.Xml;
using SSRSMigrate.Exporter.Writer;

namespace SSRSMigrate.Exporter
{
    public class ReportItemExporter : IItemExporter<ReportItem>
    {
        private readonly IExportWriter mExportWriter = null;

        public ReportItemExporter(IExportWriter exportWriter)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            this.mExportWriter = exportWriter;
        }

        public ExportStatus SaveItem(ReportItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                this.mExportWriter.Save(fileName, item.Definition, overwrite);

                //XmlDocument doc = new XmlDocument();

                //using (MemoryStream oStream = new MemoryStream(item.Definition))
                //{
                //    doc.Load(oStream);
                //    doc.Save(fileName);
                //}

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
