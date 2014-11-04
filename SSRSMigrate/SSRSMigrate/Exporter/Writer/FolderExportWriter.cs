using System;
using System.IO.Abstractions;
using System.IO;

namespace SSRSMigrate.Exporter.Writer
{
    /// <summary>
    /// Creates a folder on disk.
    /// </summary>
    public class FolderExportWriter : IExportWriter
    {
        private readonly IFileSystem mFileSystem = null;

        public FolderExportWriter(IFileSystem fileSystem)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            this.mFileSystem = fileSystem;
        }

        /// <summary>
        /// Saves the specified file name on disk as a folder.
        /// </summary>
        /// <param name="fileName">Name of the folder.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrites any folder on disk if it already exists.</param>
        /// <exception cref="System.IO.IOException"></exception>
        public void Save(string fileName, bool overwrite = true)
        {
            if (this.mFileSystem.Directory.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("Directory '{0}' already exists.", fileName));


            if (this.mFileSystem.Directory.Exists(fileName))
                this.mFileSystem.Directory.Delete(fileName, true);

            this.mFileSystem.Directory.CreateDirectory(fileName);
        }

        /// <summary>
        /// Saves the specified file name on disk as a folder.
        /// </summary>
        /// <param name="fileName">Name of the folder.</param>
        /// <param name="data">The data.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrites any folder on disk if it already exists.</param>
        public void Save(string fileName, string data, bool overwrite = true)
        {
            this.Save(fileName, overwrite);
        }

        /// <summary>
        /// Saves the specified file name on disk as a folder.
        /// </summary>
        /// <param name="fileName">Name of the folder.</param>
        /// <param name="data">The data.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrites any folder on disk if it already exists.</param>
        public void Save(string fileName, byte[] data, bool overwrite = true)
        {
            this.Save(fileName, overwrite);
        }
    }
}
