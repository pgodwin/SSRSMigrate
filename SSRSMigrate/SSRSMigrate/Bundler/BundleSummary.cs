using System.Collections.Generic;
using SSRSMigrate.Enum;

namespace SSRSMigrate.Bundler
{
    public class BundleSummary
    {
        public string SourceRootPath { get; set; }
        public SSRSVersion SourceVersion { get; set; }
        public Dictionary<string, List<BundleSummaryEntry>> Entries { get; set; }

        public BundleSummary()
        {
            // Create entries Dictionary with default keys
            this.Entries = new Dictionary<string, List<BundleSummaryEntry>>()
		    {
			    { "DataSources", new List<BundleSummaryEntry>() },
			    { "Reports", new List<BundleSummaryEntry>() },
			    { "Folders", new List<BundleSummaryEntry>() }
		    };
        }
    }
}
