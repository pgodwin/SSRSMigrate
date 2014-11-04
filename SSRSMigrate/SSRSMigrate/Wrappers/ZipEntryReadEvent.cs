using System;

namespace SSRSMigrate.Wrappers
{
    public class ZipEntryReadEvent : EventArgs
    {
        public string FileName { get; private set; }
        public string ExtractedTo { get; private set; }

        public ZipEntryReadEvent(
            string fileName,
            string extractedTo)
        {
            this.FileName = fileName;
            this.ExtractedTo = extractedTo;
        }
    }
}
