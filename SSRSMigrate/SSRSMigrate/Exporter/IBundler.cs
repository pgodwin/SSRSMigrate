using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Exporter
{
    public interface IBundler
    {
        string ExportSummaryFilename { get; }
        Dictionary<string, List<BundleSummaryEntry>> Entries { get; }
        string GetZipPath(string itemFileName, string itemPath);
        BundleSummaryEntry CreateEntrySummary(string itemFileName, string zipPath);
        void AddItem(string key, string itemFileName, string itemPath, bool isFolder);
        string CreateSummary();
        string Save(string fileName);
    }
}
