using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Importer
{
    public class ReportItemImporter : IItemImporter<ReportItem>
    {
        private readonly IFileSystem mFileSystem = null;

        public ReportItemImporter(IFileSystem fileSystem)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            this.mFileSystem = fileSystem;
        }

        public ReportItem ImportItem(string filename, out ImportStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
