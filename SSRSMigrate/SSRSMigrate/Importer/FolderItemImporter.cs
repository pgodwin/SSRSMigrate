using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Importer
{
    public class FolderItemImporter : IItemImporter<FolderItem>
    {
        private readonly IFileSystem mFileSystem = null;

        public FolderItemImporter(IFileSystem fileSystem)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            this.mFileSystem = fileSystem;
        }

        public FolderItem ImportItem(string filename, out ImportStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
