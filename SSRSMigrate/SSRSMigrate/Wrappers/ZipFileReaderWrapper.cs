using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;

namespace SSRSMigrate.Wrappers
{
    public class ZipFileReaderWrapper : ZipFileReader, IZipFileReaderWrapper
    {
        private ZipFile mZipFile = null;
        private string mUnPackDirectory = null;
        private string mFileName = null;

        #region Public Properties
        public string UnPackDirectory
        {
            get { return this.mUnPackDirectory; }
        }
        #endregion

        #region Events
        public event EntryExtractedEventHandler OnEntryExtracted;
        #endregion

        public ZipFileReaderWrapper()
        {            
        }

        public string UnPack(string fileName, string unpackDirectory)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("fileName");

            if (string.IsNullOrEmpty(unpackDirectory))
                throw new ArgumentException("unpackDirectory");

            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);

            if (Directory.Exists(this.mUnPackDirectory))
                Directory.Delete(this.mUnPackDirectory, true);

            this.mFileName = fileName;
            this.mUnPackDirectory = unpackDirectory;
            this.mZipFile = ZipFile.Read(fileName);

            this.mZipFile.ExtractProgress += ExtractProgressHandler;

            foreach (ZipEntry entry in this.mZipFile)
            {
                entry.Extract(this.mUnPackDirectory, ExtractExistingFileAction.OverwriteSilently);
            }

            return this.mUnPackDirectory;
        }

        //TODO Might be able to do away with this.
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
            this.mZipFile.ExtractProgress -= ExtractProgressHandler;
            this.mZipFile.Dispose();
        }
    }
}
