using System.Collections.Generic;

namespace SSRSMigrate.Bundler
{
    public interface IBundler
    {
        string ExportSummaryFilename { get; }
        Dictionary<string, List<BundleSummaryEntry>> Entries { get; }
        string GetZipPath(string itemFileName, string itemPath, bool isFolder = false);
        BundleSummaryEntry CreateEntrySummary(string itemFileName, string zipPath, bool isFolder = false);
        void AddItem(string key, string itemFileName, string itemPath, bool isFolder);
        string CreateSummary();
        string Save(string fileName);
        //TODO Add Exists method that checks if a file or entry exists in the underlying IZipFileWrapper
    }
}
