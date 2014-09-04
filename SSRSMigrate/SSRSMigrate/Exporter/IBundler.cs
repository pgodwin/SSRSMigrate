using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Exporter
{
    public interface IBundler
    {
        BundleSummaryEntry CreateEntrySummary(string item);

        void AddItem(string key, string itemFileName, string itemPath);

        string CreateSummary();
        string Save(string fileName);
    }
}
