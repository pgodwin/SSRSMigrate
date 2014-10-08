using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSRSMigrate.Bundler.Events;

namespace SSRSMigrate.Bundler
{
    // Event handlers
    public delegate void FolderReadEventHandler(IBundleReader sender, ItemReadEvent e);
    public delegate void DataSourceReadEventHandler(IBundleReader sender, ItemReadEvent e);
    public delegate void ReportReadEventHandler(IBundleReader sender, ItemReadEvent e);

    public interface IBundleReader 
    {
        // Properties
        string ExportSummaryFilename { get; }
        Dictionary<string, List<BundleSummaryEntry>> Entries { get; }

        // Events
        event FolderReadEventHandler OnFolderRead;
        event DataSourceReadEventHandler OnDataSourceRead;
        event ReportReadEventHandler OnReportRead;

        // Methods
        string Extract(string fileName, string unpackDirectory);
        void ReadExportSummary();
        void Read();
    }
}
