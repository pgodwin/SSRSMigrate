using System;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Exporter.Writer;
using SSRSMigrate.Status;

namespace SSRSMigrate.Exporter
{
    public class ReportItemExporter : IItemExporter<ReportItem>
    {
        private readonly IExportWriter mExportWriter = null;
        private readonly ILogger mLogger = null;

        public ReportItemExporter(IExportWriter exportWriter, ILogger logger)
        {
            if (exportWriter == null)
                throw new ArgumentNullException("exportWriter");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mExportWriter = exportWriter;
            this.mLogger = logger;
        }

        public ExportStatus SaveItem(ReportItem item, string fileName, bool overwrite = true)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            try
            {
                this.mLogger.Trace("Saving item '{0}' to '{1}'.", item.Name, fileName);

                this.mExportWriter.Save(fileName, item.Definition, overwrite);

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
