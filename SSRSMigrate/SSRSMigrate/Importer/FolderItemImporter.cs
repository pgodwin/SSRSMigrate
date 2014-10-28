using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Importer
{
    public class FolderItemImporter : IItemImporter<FolderItem>
    {
        private readonly IFileSystem mFileSystem = null;
        private readonly ILogger mLogger = null;
 
        public FolderItemImporter(IFileSystem fileSystem,
            ILogger logger)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mFileSystem = fileSystem;
            this.mLogger = logger;
        }

        public FolderItem ImportItem(string filename, out ImportStatus status)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            if (!this.mFileSystem.Directory.Exists(filename))
                throw new DirectoryNotFoundException(filename);

            string diskPath = this.mFileSystem.Path.GetDirectoryName(filename);
            Exception error = null;
            bool success = true;

            FolderItem item = null;

            try
            {
                string name = this.mFileSystem.Path.GetFileName(filename);

                // Get the path Item's path from the path on disk, inside the Export\ directory
                string token = "\\Export\\";
                string path = filename.Substring(filename.IndexOf(token) + token.Length - 1).Replace("\\", "/");

                item = new FolderItem();
                item.Name = name;
                item.Path = path;

                success = true;
            }
            catch (Exception er)
            {
                success = false;
                error = er;
            }

            status = new ImportStatus(filename, diskPath, error, success);

            return item;
        }
    }
}
