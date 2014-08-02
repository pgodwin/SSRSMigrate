using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SSRSMigrate.Exporter.Writer
{
    public class FolderExportWriter : IExportWriter
    {
        public FolderExportWriter()
        {
        }

        //TODO This doesn't feel right
        public void Save(string fileName, bool overwrite = true)
        {
            if (Directory.Exists(fileName) && !overwrite)
                throw new IOException(string.Format("Directory '{0}' already exists.", fileName));


            if (Directory.Exists(fileName))
                Directory.Delete(fileName, true);

            Directory.CreateDirectory(fileName);
        }

        public void Save(string fileName, string data, bool overwrite = true)
        {
            throw new NotImplementedException();
        }

        public void Save(string fileName, byte[] data, bool overwrite = true)
        {
            throw new NotImplementedException();
        }
    }
}
