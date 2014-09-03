using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Wrappers
{
    public interface IZipFileWrapper : IDisposable
    {
        void AddEntry(string entryName, string content);
        void AddFile(string fileName, string directoryPathInArchive);
        void AddDirectory(string directoryName);
        void Save(string fileName);
    }
}
