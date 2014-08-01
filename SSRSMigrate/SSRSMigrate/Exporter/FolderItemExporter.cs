using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using System.IO;

namespace SSRSMigrate.Exporter
{
    public class FolderItemExporter : IItemExporter<FolderItem>
    {
        public FolderItemExporter()
        {
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
                else if (Directory.Exists(fileName) && overwrite)
                    Directory.Delete(fileName, true);

                if (!Directory.Exists(fileName))
                    Directory.CreateDirectory(fileName);

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
