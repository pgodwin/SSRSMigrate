using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.IO;

namespace SSRSMigrate.Exporter.Writer
{
    /// <summary>
    /// Writes data to a file on disk.
    /// </summary>
    public class FileExportWriter : IExportWriter
    {
        private readonly IFileSystem mFileSystem = null;

        public FileExportWriter(IFileSystem fileSystem)
        {
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");

            this.mFileSystem = fileSystem;
        }

        /// <summary>
        /// Saves the specified file name with no data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrites any file on disk if it already exists.</param>
        /// <exception cref="System.ArgumentException">
        /// fileName
        /// or
        /// fileName
        /// </exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void Save(string fileName, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (this.mFileSystem.File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (this.mFileSystem.File.Exists(fileName) && overwrite)
                this.mFileSystem.File.Delete(fileName);

            if (!this.mFileSystem.Directory.Exists(Path.GetDirectoryName(fileName)))
                this.mFileSystem.Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            this.mFileSystem.File.WriteAllText(fileName, "");
        }

        /// <summary>
        /// Saves the specified file name with the data provided.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data to write to disk.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrites any file on disk if it already exists.</param>
        /// <exception cref="System.ArgumentException">
        /// fileName
        /// or
        /// data
        /// </exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void Save(string fileName, string data, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data");

            if (this.mFileSystem.File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (this.mFileSystem.File.Exists(fileName) && overwrite)
                this.mFileSystem.File.Delete(fileName);

            if (!this.mFileSystem.Directory.Exists(Path.GetDirectoryName(fileName)))
                this.mFileSystem.Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            this.mFileSystem.File.WriteAllText(fileName, data);
        }

        /// <summary>
        /// Saves the specified file name with the data provided.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="data">The data to write to disk.</param>
        /// <param name="overwrite">if set to <c>true</c> overwrites any file on disk if it already exists.</param>
        /// <exception cref="System.ArgumentException">fileName</exception>
        /// <exception cref="System.ArgumentNullException">data</exception>
        /// <exception cref="System.IO.IOException"></exception>
        public void Save(string fileName, byte[] data, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (data == null)
                throw new ArgumentNullException("data");

            if (this.mFileSystem.File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (this.mFileSystem.File.Exists(fileName) && overwrite)
                this.mFileSystem.File.Delete(fileName);

            if (!this.mFileSystem.Directory.Exists(Path.GetDirectoryName(fileName)))
                this.mFileSystem.Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            this.mFileSystem.File.WriteAllBytes(fileName, data);
        }
    }
}
