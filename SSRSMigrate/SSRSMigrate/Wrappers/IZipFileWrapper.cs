using System;

namespace SSRSMigrate.Wrappers
{
    public interface IZipFileWrapper : IDisposable
    {
        bool FileExists(string zipFileName);

        void AddEntry(string entryName, string content);
        void AddFile(string fileName);
        void AddFile(string fileName, string directoryPathInArchive);
        void AddDirectory(string directoryName);
        void AddDirectory(string directoryName, string directoryPathInArchive);
        void Save(string fileName);
        void Save(string fileName, bool overwrite);
        void Reset();
    }
}
