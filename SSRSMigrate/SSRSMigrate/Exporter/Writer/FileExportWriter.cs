using System;
using System.Collections.Generic;
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
        public FileExportWriter()
        {
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

            //TODO Use System.IO.Abstractions
            if (File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write("");
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

            //TODO Use System.IO.Abstractions
            if (File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write(data);
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

            //TODO Use System.IO.Abstractions
            if (File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllBytes(fileName, data);
        }
    }
}
