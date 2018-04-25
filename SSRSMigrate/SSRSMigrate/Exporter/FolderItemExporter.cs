using System;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Status;

namespace SSRSMigrate.Exporter
{
    public class FolderItemExporter : IItemExporter<FolderItem>
    {
        private readonly IExportWriter mExportWriter = null;
        private readonly ILogger mLogger = null;

        public FolderItemExporter(IExportWriter exportWriter, ILogger logger)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mExportWriter = exportWriter;
            this.mLogger = logger;
        }

        public ExportStatus SaveItem(FolderItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                this.mLogger.Trace("Saving item '{0}' to '{1}'.", item.Name, fileName);
                this.mExportWriter.Save(fileName, overwrite);

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
