using System;
using System.IO;
using Ionic.Zip;
using SSRSMigrate.Errors;

namespace SSRSMigrate.Wrappers
{
    public class ZipFileReaderWrapper : ZipFileReader, IZipFileReaderWrapper
    {
        private string mUnPackDirectory = null;
        private string mFileName = null;

        #region Public Properties
        public string UnPackDirectory
        {
            get { return this.mUnPackDirectory; }
            set
            {
                if (Directory.Exists(this.mUnPackDirectory))
                    Directory.Delete(this.mUnPackDirectory, true);

                this.mUnPackDirectory = value;
            }
        }

        public string FileName
        {
            get { return this.mFileName; }
            set
            {
                if (!ZipFile.IsZipFile(value))
                    throw new InvalidFileArchiveException(value);

                this.mFileName = value;
            }
        }
        #endregion

        #region Events
        public event EntryExtractedEventHandler OnEntryExtracted;
        #endregion

        public ZipFileReaderWrapper()
        {
            
        }

        public ZipFileReaderWrapper(string unpackDirectory)
        {
            if (string.IsNullOrEmpty(unpackDirectory))
                throw new ArgumentException("unpackDirectory");

            if (Directory.Exists(this.mUnPackDirectory))
                Directory.Delete(this.mUnPackDirectory, true);

            this.mUnPackDirectory = unpackDirectory;
        }

        public ZipFileReaderWrapper(string fileName, string unpackDirectory)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(unpackDirectory))
                throw new ArgumentException("unpackDirectory");

            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            if (!ZipFile.IsZipFile(fileName))
                throw new InvalidFileArchiveException(fileName);

            if (Directory.Exists(this.mUnPackDirectory))
                Directory.Delete(this.mUnPackDirectory, true);

            this.mFileName = fileName;
            this.mUnPackDirectory = unpackDirectory;
        }

        public string UnPack()
        {
            if (string.IsNullOrEmpty(this.mFileName))
                throw new ArgumentException("Please specify a filename.");

            using (ZipFile zipFile = ZipFile.Read(this.mFileName))
            {
                zipFile.ExtractProgress += ExtractProgressHandler;

                foreach (ZipEntry entry in zipFile)
                {
                    entry.Extract(this.mUnPackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }

                return this.mUnPackDirectory;
            }
        }

        public string ReadEntry(string entryName)
        {
            if (string.IsNullOrEmpty(entryName))
                throw new ArgumentException("entryName");

            using (ZipFile zipFile = ZipFile.Read(this.mFileName))
            {
                if (!zipFile.ContainsEntry(entryName))
                    throw new FileNotFoundException(entryName);

                ZipEntry entry = zipFile[entryName];

                using (var stream = entry.OpenReader())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        internal override void ExtractProgressHandler(object sender, EventArgs e)
        {
            ExtractProgressEventArgs evt = (ExtractProgressEventArgs) e;

            if (evt.EventType == ZipProgressEventType.Extracting_AfterExtractEntry)
            {
                string fileName = evt.CurrentEntry.FileName;
                fileName = fileName.Replace('/', '\\');

                string extractTo = Path.Combine(evt.ExtractLocation, fileName);

                if (this.OnEntryExtracted != null)
                {
                    ZipEntryReadEvent readEvent = new ZipEntryReadEvent(
                        fileName,
                        extractTo);

                    this.OnEntryExtracted(this, readEvent);
                }
            }
        }

        public void Dispose()
        {
            //this.mZipFile.ExtractProgress -= ExtractProgressHandler;
            //this.mZipFile.Dispose();
        }
    }
}
