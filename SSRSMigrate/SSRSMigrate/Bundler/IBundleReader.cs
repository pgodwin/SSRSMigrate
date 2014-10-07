using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Bundler
{
    public interface IBundleReader 
    {
        string ExportSummaryFilename { get; }
        Dictionary<string, List<BundleSummaryEntry>> Entries { get; }
    }
}
