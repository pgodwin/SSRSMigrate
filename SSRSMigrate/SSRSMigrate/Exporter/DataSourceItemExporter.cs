using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using Newtonsoft.Json;
using System.IO;

namespace SSRSMigrate.Exporter
{
    public class DataSourceItemExporter : IItemExporter<DataSourceItem>
    {
        public DataSourceItemExporter()
        {

        }

        public ExportStatus SaveItem(DataSourceItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                if (File.Exists(fileName))
                    throw new IOException(string.Format("File '{0}' already exists.", fileName));

                string json = JsonConvert.SerializeObject(item, Formatting.Indented);

                using (StreamWriter sw = new StreamWriter(fileName))
                    sw.Write(json);

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
