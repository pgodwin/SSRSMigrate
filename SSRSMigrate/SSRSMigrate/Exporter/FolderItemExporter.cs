using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Status;

namespace SSRSMigrate.Exporter
{
    public class FolderItemExporter : IItemExporter<FolderItem>
    {
        private readonly IExportWriter mExportWriter = null;

        public FolderItemExporter(IExportWriter exportWriter)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            this.mExportWriter = exportWriter;
        }

        public ExportStatus SaveItem(FolderItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                this.mExportWriter.Save(fileName, overwrite);

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
