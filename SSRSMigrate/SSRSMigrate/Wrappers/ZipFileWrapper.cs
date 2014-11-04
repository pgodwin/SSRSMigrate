using System;
using System.Linq;
using Ionic.Zip;
using System.IO;

namespace SSRSMigrate.Wrappers
{
    public class ZipFileWrapper : IZipFileWrapper
    {
        private readonly ZipFile mZipFile = null;

        public ZipFileWrapper()
        {
            this.mZipFile = new ZipFile();
        }

        public ZipFileWrapper(string comment)
        {
            this.mZipFile = new ZipFile();
            this.mZipFile.Comment = comment;
        }

        public bool FileExists(string zipFileName)
        {
            if (string.IsNullOrEmpty(zipFileName))
                throw new ArgumentException("zipFileName");

            if (zipFileName.EndsWith("/"))
                zipFileName = zipFileName.Remove(zipFileName.Length - 1);

            foreach (string e in this.mZipFile.EntryFileNames)
            {
                string entry = e;

                if (entry.EndsWith("/"))
                    entry = entry.Remove(entry.Length - 1);

                if (entry.Replace("/", "\\") == zipFileName)
                    return true;
            }

		    return false;
        }

        public void AddEntry(string entryName, string content)
        {
            this.mZipFile.AddEntry(entryName, content);
        }

        public void AddFile(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            this.mZipFile.AddFile(fileName);
        }

        public void AddFile(string fileName, string directoryPathInArchive)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            this.mZipFile.AddFile(fileName, directoryPathInArchive);
        }

        public void AddDirectory(string directoryName)
        {
            if (!Directory.Exists(directoryName))
                throw new DirectoryNotFoundException(directoryName);

            if (!this.mZipFile.Any(entry => entry.FileName.Contains(directoryName.Replace("\\", "/"))))
                this.mZipFile.AddDirectory(directoryName);
        }

        public void AddDirectory(string directoryName, string directoryPathInArchive)
        {
            if (!Directory.Exists(directoryName))
                throw new DirectoryNotFoundException(directoryName);

            if (!this.mZipFile.Any(entry => entry.FileName.Contains(directoryPathInArchive.Replace("\\", "/"))))
                this.mZipFile.AddDirectory(directoryName, directoryPathInArchive);
        }

        public void Save(string fileName)
        {
            this.Save(fileName, false);
        }

        public void Save(string fileName, bool overwrite)
        {
            if (overwrite)
                if (File.Exists(fileName))
                    File.Delete(fileName);

            this.mZipFile.Save(fileName);
        }

        public void Dispose()
        {
            this.mZipFile.Dispose();
        }
    }
}
