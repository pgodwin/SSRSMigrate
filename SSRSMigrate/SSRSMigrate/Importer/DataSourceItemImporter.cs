using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Importer
{
    public class DataSourceItemImporter : IItemImporter<DataSourceItem>
    {
        private readonly IFileSystem mFileSystem = null;
        private readonly ILogger mLogger = null;

        public DataSourceItemImporter(IFileSystem fileSystem, ILogger logger)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mFileSystem = fileSystem;
            this.mLogger = logger;
        }

        public DataSourceItem ImportItem(string filename, out ImportStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
