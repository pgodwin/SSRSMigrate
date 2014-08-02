using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Exporter.Writer
{
    public interface IExportWriter
	{
        void Save(string fileName, bool overwrite);
        void Save(string fileName, string data, bool overwrite);
        void Save(string fileName, byte[] data, bool overwrite);
	}
}
