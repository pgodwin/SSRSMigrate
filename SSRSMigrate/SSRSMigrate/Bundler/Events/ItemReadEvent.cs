using System;

namespace SSRSMigrate.Bundler.Events
{
    public class ItemReadEvent : EventArgs
    {
        public string FileName { get; private set; }
        public string Path { get; private set; }
        public bool IsFolder { get; private set; }
        public string CheckSum { get; private set; }
        public bool Success { get; private set; }
        public string[] Errors { get; private set; }

        public ItemReadEvent(
            string fileName,
            string path,
            bool isFolder,
            string checkSum,
            bool success, 
            string[] errors = null)
        {
            this.FileName = fileName;
            this.Path = path;
            this.IsFolder = isFolder;
            this.CheckSum = checkSum;
            this.Success = success;
            this.Errors = errors;
        }
    }
}
