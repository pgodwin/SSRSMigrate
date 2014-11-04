using System;
using System.IO;
using System.IO.Abstractions;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.Status;

namespace SSRSMigrate.Importer
{
    public class ReportItemImporter : IItemImporter<ReportItem>
    {
        private readonly IFileSystem mFileSystem = null;
        private readonly ILogger mLogger = null;

        public ReportItemImporter(IFileSystem fileSystem,
            ILogger logger)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this.mFileSystem = fileSystem;
            this.mLogger = logger;
        }

        public ReportItem ImportItem(string filename, out ImportStatus status)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            if (!this.mFileSystem.File.Exists(filename))
                throw new FileNotFoundException(filename);

            this.mLogger.Debug("ImportItem - filename = {0}", filename);

            string token = "\\Export\\";

            byte[] data = this.mFileSystem.File.ReadAllBytes(filename);
            string diskPath = this.mFileSystem.Path.GetDirectoryName(filename);
            Exception error = null;
            bool success = true;

            ReportItem item = null;

            try
            {
                this.mLogger.Debug("ImportItem - diskPath = {0}", diskPath);

                string name = this.mFileSystem.Path.GetFileNameWithoutExtension(filename);

                this.mLogger.Debug("ImportItem - name = {0}", name);

                // If the Item's filename does not contain the token, throw an exception and fail the import.
                if (!filename.Contains(token))
                    throw new Exception(string.Format("Item's filename '{0}' does not contain token '{1}'.",
                        filename, token));

                // Get the path Item's path from the path on disk, inside the Export\ directory
                string path = filename.Substring(filename.IndexOf(token) + token.Length - 1).Replace("\\", "/");
                
                // Replace the last instance of .rdl in the path, which will be the 
                //  file extension for ReportItems saved to disk.
                int i = path.LastIndexOf(".rdl");
                if (i >= 0)
                    path = path.Substring(0, i) + path.Substring(i + ".rdl".Length);

                this.mLogger.Debug("ImportItem - path = {0}", path);

                item = new ReportItem()
                {
                    Name = name,
                    Path = path,
                    Definition = data
                };
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
