using System.IO.Abstractions;

namespace SSRSMigrate.SSRS.Item.Proxy
{
    public class ReportItemDiskProxy : ReportItem
    {
        private byte[] mDefinition = null;
        private readonly string mFileName;
        private readonly IFileSystem mFileSystem;

        public ReportItemDiskProxy(string fileName, IFileSystem fileSystem)
        {
            this.mFileName = fileName;
            this.mFileSystem = fileSystem;
        }

        public override byte[] Definition
        {
            get
            {
                if (this.mDefinition == null)
                {
                    byte[] data = this.mFileSystem.File.ReadAllBytes(this.mFileName);

                    this.mDefinition = data;

                    return data;
                }
                else
                {
                    return this.mDefinition;
                }
            }

            set { this.mDefinition = value; }
        }
    }
}
