using System;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Status;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Exporter
{
    public class DataSourceItemExporter : IItemExporter<DataSourceItem>
    {
        private readonly IExportWriter mExportWriter = null;
        private readonly ISerializeWrapper mSerializeWrapper = null;

        public DataSourceItemExporter(
            IExportWriter exportWriter,
            ISerializeWrapper serializeWrapper)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            if (serializeWrapper == null)
                throw new ArgumentNullException("serializeWrapper");

            this.mExportWriter = exportWriter;
            this.mSerializeWrapper = serializeWrapper;
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
                string json = this.mSerializeWrapper.SerializeObject(item);

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
