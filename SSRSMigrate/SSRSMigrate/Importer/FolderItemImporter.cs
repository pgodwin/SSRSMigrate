using System;
using System.IO;
using System.IO.Abstractions;
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

            this.mLogger.Debug("ImportItem - filename = {0}", filename);

            string token = "\\Export\\";

            string diskPath = this.mFileSystem.Path.GetDirectoryName(filename);
            Exception error = null;
            bool success = true;

            FolderItem item = null;

            try
            {
                this.mLogger.Debug("ImportItem - diskPath = {0}", diskPath);

                string name = this.mFileSystem.Path.GetFileName(filename);

                this.mLogger.Debug("ImportItem - name = {0}", name);

                // If the Item's filename does not contain the token, throw an exception and fail the import.
                if (!filename.Contains(token))
                    throw new Exception(string.Format("Item's filename '{0}' does not contain token '{1}'.",
                        filename, token));

                // Get the path Item's path from the path on disk, inside the Export\ directory
                string path = filename.Substring(filename.IndexOf(token) + token.Length - 1).Replace("\\", "/");

                this.mLogger.Debug("ImportItem - path = {0}", path);

                item = new FolderItem()
                {
                    Name = name,
                    Path = path,
                };

                success = true;
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
