using System;
using Ninject.Extensions.Logging;
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
        private readonly ILogger mLogger = null;

        public DataSourceItemExporter(
            IExportWriter exportWriter,
            ISerializeWrapper serializeWrapper,
            ILogger logger)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            if (serializeWrapper == null)
                throw new ArgumentNullException("serializeWrapper");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mExportWriter = exportWriter;
            this.mSerializeWrapper = serializeWrapper;
            this.mLogger = logger;
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
                this.mLogger.Trace("Serializing item '{0}' to JSON...", item.Name);
                string json = this.mSerializeWrapper.SerializeObject(item);
                this.mLogger.Trace("Serialized item '{0}' to JSON: {1}", item.Name, json);

                this.mLogger.Trace("Saving item '{0}' to '{1}'.", item.Name, fileName);
                this.mExportWriter.Save(fileName, json, overwrite);

                return new ExportStatus(fileName, item.Path, null, true);
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "Error saving item '{0}' to '{1}'", item.Name, fileName);

                return new ExportStatus(fileName, item.Path, new string[] { er.Message }, false);
            }
        }
    }
}
