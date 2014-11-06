using System.Collections.Generic;
using SSRSMigrate.Enum;

namespace SSRSMigrate.Bundler
{
    public interface IBundler
    {
        // Properties
        string ExportSummaryFilename { get; }
        BundleSummary Summary { get;  }

        string GetZipPath(string itemFileName, string itemPath, bool isFolder = false);
        BundleSummaryEntry CreateEntrySummary(string itemFileName, string zipPath, bool isFolder = false);
        void AddItem(string key, string itemFileName, string itemPath, bool isFolder);
        string CreateSummary(string rootPath, SSRSVersion version);
        string Save(string fileName);
        void Reset();
        //TODO Add Exists method that checks if a file or entry exists in the underlying IZipFileWrapper
    }
}
