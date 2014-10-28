using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Importer
{
    public class DataSourceItemImporter : IItemImporter<DataSourceItem>
    {
        private readonly IFileSystem mFileSystem = null;
        private readonly ILogger mLogger = null;
        private readonly ISerializeWrapper mSerializeWrapper = null;

        public DataSourceItemImporter(ISerializeWrapper serializeWrapper,
            IFileSystem fileSystem, 
            ILogger logger)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            if (logger == null)
                throw new ArgumentNullException("logger");

            if (serializeWrapper == null)
                throw new ArgumentNullException("serializeWrapper");

            this.mFileSystem = fileSystem;
            this.mLogger = logger;
            this.mSerializeWrapper = serializeWrapper;
        }

        public DataSourceItem ImportItem(string filename, out ImportStatus status)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            if (!this.mFileSystem.File.Exists(filename))
                throw new FileNotFoundException(filename);

            this.mLogger.Debug("ImportItem - filename = {0}", filename);

            string data = this.mFileSystem.File.ReadAllText(filename);
            string diskPath = this.mFileSystem.Path.GetDirectoryName(filename);
            Exception error = null;
            bool success = true;

            DataSourceItem item = null;

            try
            {
                this.mLogger.Trace("ImportItem - data = {0}", data);
                this.mLogger.Debug("ImportItem - diskPath = {0}", diskPath);

                item = this.mSerializeWrapper.DeserializeObject<DataSourceItem>(data);
                success = true;

                this.mLogger.Debug("ImportItem - Name = {0}", item.Name);
                this.mLogger.Debug("ImportItem - Path = {0}", item.Path);
            }
            catch (Exception er)
            {
                this.mLogger.Error(er, "ImportItem - Error importing item from '{0}'.", filename);

                success = false;
                error = er;
            }
            
            status = new ImportStatus(filename, diskPath, error, success);

            return item;
        }
    }
}
