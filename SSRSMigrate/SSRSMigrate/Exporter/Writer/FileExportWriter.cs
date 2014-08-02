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
            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write("");
        }

        public void Save(string fileName, string data, bool overwrite = true)
        {
            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write(data);
        }

        public void Save(string fileName, byte[] data, bool overwrite = true)
        {
            if (File.Exists(fileName) && overwrite)
                File.Delete(fileName);

            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllBytes(fileName, data);
        }
    }
}
