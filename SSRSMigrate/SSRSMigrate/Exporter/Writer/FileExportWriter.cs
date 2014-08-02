using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SSRSMigrate.Exporter.Writer
{
    public class FileExportWriter : IExportWriter
    {
        public FileExportWriter()
        {
        }

        public void Save(string fileName, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write("");
        }

        public void Save(string fileName, string data, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data");

            if (File.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("File '{0}' already exists.", fileName));

            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write(data);
        }

        public void Save(string fileName, byte[] data, bool overwrite = true)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (data == null)
                throw new ArgumentNullException("data");

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
