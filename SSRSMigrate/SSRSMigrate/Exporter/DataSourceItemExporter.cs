using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using Newtonsoft.Json;
using System.IO;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Status;

namespace SSRSMigrate.Exporter
{
    public class DataSourceItemExporter : IItemExporter<DataSourceItem>
    {
        private readonly IExportWriter mExportWriter = null;

        public DataSourceItemExporter(IExportWriter exportWriter)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            this.mExportWriter = exportWriter;
        }

        public ExportStatus SaveItem(DataSourceItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                // Serialize DataSourceItem to JSON
                //TODO Use ISerializeWrapper
                string json = JsonConvert.SerializeObject(item, Formatting.Indented);

                this.mExportWriter.Save(fileName, json, overwrite);

                //using (StreamWriter sw = new StreamWriter(fileName))
                //    sw.Write(json);

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
