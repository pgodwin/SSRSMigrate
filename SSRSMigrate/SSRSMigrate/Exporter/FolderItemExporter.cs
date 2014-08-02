using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using System.IO;
using SSRSMigrate.Exporter.Writer;

namespace SSRSMigrate.Exporter
{
    public class FolderItemExporter : IItemExporter<FolderItem>
    {
        private IExportWriter mExportWriter = null;

        public FolderItemExporter(IExportWriter exportWriter)
        {
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
                if (Directory.Exists(fileName) && !overwrite)
                    throw new IOException(string.Format("Directory '{0}' already exists.", fileName));

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
