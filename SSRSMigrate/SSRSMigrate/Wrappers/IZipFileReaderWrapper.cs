using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Wrappers
{
    // Event handlers
    public delegate void EntryExtractedEventHandler(IZipFileReaderWrapper sender, ZipEntryReadEvent e);

    public interface IZipFileReaderWrapper : IDisposable
    {
        // Properties
        string UnPackDirectory { get;  }
        string FileName { get; set; }

        // Events
        event EntryExtractedEventHandler OnEntryExtracted;

        // Methods
        string UnPack();
        string ReadEntry(string entryName);

        // Internal Zip Event Handler
        //void ExtractProgressHandler(object sender, EventArgs e);
    }

    public abstract class ZipFileReader
    {
        abstract internal void ExtractProgressHandler(object sender, EventArgs e);
    }
}
