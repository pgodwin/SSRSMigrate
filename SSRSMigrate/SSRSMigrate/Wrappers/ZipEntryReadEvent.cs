using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Wrappers
{
    public class ZipEntryReadEvent : EventArgs
    {
        public string FileName { get; private set; }
        public string ExtractedTo { get; private set; }
        public int EntryProgress { get; private set; }
        public int EntryTotal { get; private set; }

        public ZipEntryReadEvent(
            string fileName,
            string extractedTo,
            int entryProgress,
            int entryTotal)
        {
            this.FileName = fileName;
            this.ExtractedTo = extractedTo;
            this.EntryProgress = entryProgress;
            this.EntryTotal = entryTotal;
        }
    }
}
