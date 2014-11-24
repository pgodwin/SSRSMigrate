using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Windows.Forms;
using Ninject.Extensions.Logging;
using SSRSMigrate.Bundler;
using SSRSMigrate.Bundler.Events;
using SSRSMigrate.Importer;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Writer;
using SSRSMigrate.Status;
using SSRSMigrate.Utility;

namespace SSRSMigrate.Forms
{
    [CoverageExclude]
    public partial class ImportZipForm : Form
    {
        private readonly IBundleReader mBundleReader = null;
        private readonly IItemImporter<DataSourceItem> mDataSourceItemImporter = null;
        private readonly IItemImporter<FolderItem> mFolderItemImporter = null;
        private readonly IItemImporter<ReportItem> mReportItemImporter = null;
        private readonly IReportServerWriter mReportServerWriter = null;
        private ILoggerFactory mLoggerFactory = null;
        private readonly string mSourceFileName = null;
        private readonly string mDestinationRootPath = null;
        private readonly string mDestinationServerUrl = null;
        private readonly IFileSystem mFileSystem = null;

        private BackgroundWorker mSourceRefreshWorker = null;
        private BackgroundWorker mImportWorker = null;
        private ILogger mLogger = null;
        private DebugForm mDebugForm = null;

        #region Properties
        public DebugForm DebugForm
        {
            set { this.mDebugForm = value; }
        }
        #endregion

        public ImportZipForm(
            string sourceFileName,
            string destinationRootPath,
            string destinationServerUrl,
            IBundleReader bundleReader,
            IReportServerWriter writer,
            ILoggerFactory loggerFactory,
            IItemImporter<DataSourceItem> dataSourceItemImporter,
            IItemImporter<FolderItem> folderItemImporter,
            IItemImporter<ReportItem> reportItemImporter,
            IFileSystem fileSystem)
        {
            if (string.IsNullOrEmpty(sourceFileName))
                throw new ArgumentException("sourceFileName");

            if (string.IsNullOrEmpty(destinationRootPath))
                throw new ArgumentException("destinationRootPath");

            if (string.IsNullOrEmpty(destinationServerUrl))
                throw new ArgumentException("destinationServerUrl");

            if (bundleReader == null)
                throw new ArgumentNullException("bundleReader");

            if (writer == null)
                throw new ArgumentNullException("writer");

            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            if (dataSourceItemImporter == null)
                throw new ArgumentNullException("dataSourceItemImporter");

            if (folderItemImporter == null)
                throw new ArgumentNullException("folderItemImporter");

            if (reportItemImporter == null)
                throw new ArgumentNullException("reportItemImporter");

            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
    
            InitializeComponent();

            this.mSourceFileName = sourceFileName;
            this.mDestinationRootPath = destinationRootPath;
            this.mDestinationServerUrl = destinationServerUrl;
            this.mBundleReader = bundleReader;
            this.mReportServerWriter = writer;
            this.mLoggerFactory = loggerFactory;
            this.mLogger = this.mLoggerFactory.GetCurrentClassLogger();
            this.mDataSourceItemImporter = dataSourceItemImporter;
            this.mFolderItemImporter = folderItemImporter;
            this.mReportItemImporter = reportItemImporter;
            this.mFileSystem = fileSystem;
        }

        #region UI Events
        private void ImportZipForm_Load(object sender, EventArgs e)
        {
            this.mSourceRefreshWorker = new BackgroundWorker();
            this.mSourceRefreshWorker.WorkerReportsProgress = true;
            this.mSourceRefreshWorker.WorkerSupportsCancellation = true;
            this.mSourceRefreshWorker.DoWork += new DoWorkEventHandler(this.SourceRefreshWorker);
            this.mSourceRefreshWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_SourceRefreshCompleted);
            this.mSourceRefreshWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_SourceRefreshProgressChanged);

            this.mImportWorker = new BackgroundWorker();
            this.mImportWorker.WorkerReportsProgress = true;
            this.mImportWorker.WorkerSupportsCancellation = true;
            this.mImportWorker.DoWork += new DoWorkEventHandler(this.ImportWorker);
            this.mImportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_ImportCompleted);
            this.mImportWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_ImportProgressChanged);

            this.mBundleReader.OnDataSourceRead += BundleReaderOnDataSourceRead;
            this.mBundleReader.OnFolderRead += BundleReaderOnFolderRead;
            this.mBundleReader.OnReportRead += BundleReaderOnReportRead;
        }

        private void ImportZipForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If a report refresh is in progress, don't allow the form to close
            if (this.mSourceRefreshWorker != null)
                if (this.mSourceRefreshWorker.IsBusy)
                    e.Cancel = true;

            // If the import is in progress, don't allow the form to close
            if (this.mImportWorker != null)
                if (this.mImportWorker.IsBusy)
                    e.Cancel = true;

            if (this.mBundleReader != null)
            {
                this.mBundleReader.OnDataSourceRead -= BundleReaderOnDataSourceRead;
                this.mBundleReader.OnFolderRead -= BundleReaderOnFolderRead;
                this.mBundleReader.OnReportRead -= BundleReaderOnReportRead;
            }

            if (this.mSourceRefreshWorker != null)
            {
                this.mSourceRefreshWorker.DoWork -= new DoWorkEventHandler(this.SourceRefreshWorker);
                this.mSourceRefreshWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.bw_SourceRefreshCompleted);
                this.mSourceRefreshWorker.ProgressChanged -= new ProgressChangedEventHandler(this.bw_SourceRefreshProgressChanged);
            }

            if (this.mImportWorker != null)
            {
                this.mImportWorker.DoWork -= new DoWorkEventHandler(this.ImportWorker);
                this.mImportWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.bw_ImportCompleted);
                this.mImportWorker.ProgressChanged -= new ProgressChangedEventHandler(this.bw_ImportProgressChanged);

            }

            if (this.mFileSystem.Directory.Exists(this.mBundleReader.UnPackDirectory))
                this.mFileSystem.Directory.Delete(this.mBundleReader.UnPackDirectory, true);
        }

        private void btnPerformImport_Click(object sender, EventArgs e)
        {
            // If there are no items in the list, there is nothing to import
            if (this.lstSrcReports.Items.Count <= 0)
                return;

            this.ImportZip();
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {

        }

        private void btnSrcRefreshReports_Click(object sender, EventArgs e)
        {
            this.SourceRefreshReports();
        }
        #endregion

        #region Source Reports Methods
        private void SourceRefreshReports()
        {
            this.lstSrcReports.Items.Clear();

            try
            {
                this.btnSrcRefreshReports.Enabled = false;
                this.btnPerformImport.Enabled = false;

                // Extract bundle
                this.mLogger.Info("Extracting bundle...");
                this.lblStatus.Text = string.Format("Extracting bundle...");
                this.mBundleReader.Extract();
                this.mLogger.Info("Bundle extracted!");
                this.lblStatus.Text = "Bundle extracted!";

                this.mSourceRefreshWorker.RunWorkerAsync();
            }
            catch (Exception er)
            {
                string msg = string.Format("Error getting list of items from '{0}' archive.",
                    this.mSourceFileName);

                this.mDebugForm.LogMessage(msg, er);

                this.mLogger.Error(er, msg);

                MessageBox.Show(
                   string.Format("Error getting list of items from '{0}' archive.\n\r\n\r{1}",
                        this.mSourceFileName,
                        er.Message),
                   "Refresh Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
        }

        public void SourceRefreshWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string path = (string) e.Argument;

            // Read export summary entry
            this.mBundleReader.ReadExportSummary();

            // Read bundle
            this.mBundleReader.Read();
        }

        private void bw_SourceRefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = null;

            if ((e.Cancelled == true))
            {
                msg = string.Format("Cancelled. {0}", e.Result);

                this.mDebugForm.LogMessage(msg);
            }
            else if ((e.Error != null))
            {
                if (e.Error is FileNotFoundException)
                {
                    msg = string.Format("Invalid export bundle. File '{0}' does not exist.", e.Error.Message);
                }
                else
                {
                    msg = string.Format("Error. {0}", e.Error.Message);
                }
                

                this.mLogger.Error(e.Error, "Error during item refresh");

                this.mDebugForm.LogMessage(msg, e.Error);

                MessageBox.Show(msg,
                    "Refresh Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                msg = string.Format("Completed.");

                // Only allow import if the refresh completed without error
                //  and there are items to import
                if (this.lstSrcReports.Items.Count > 0)
                    this.btnPerformImport.Enabled = true;

                this.mDebugForm.LogMessage(msg);
            }

            this.mLogger.Info("Item refresh: {0}", msg);
            this.lblStatus.Text = msg;
            this.btnSrcRefreshReports.Enabled = true;
        }

        private void bw_SourceRefreshProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void BundleReaderOnDataSourceRead(IBundleReader sender, ItemReadEvent itemReadEvent)
        {
            ListViewItem oItem = null;

            if (itemReadEvent.Success)
            {
                ImportStatus status = null;
                DataSourceItem item = this.mDataSourceItemImporter.ImportItem(itemReadEvent.FileName, out status);

                if (status.Success)
                {
                    oItem = this.AddListViewItem_Success(itemReadEvent, status, item);
                }
                else
                {
                    oItem = this.AddListViewItem_ImportFailed(itemReadEvent, status);
                }
            }
            else
            {
                oItem = this.AddListViewItem_ExtractFailed(itemReadEvent);
            }

            oItem.SubItems.Add(itemReadEvent.Path);
            oItem.SubItems.Add(itemReadEvent.FileName);

            oItem.Group = this.lstSrcReports.Groups["dataSourcesGroup"];

            this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
            this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

            this.lblStatus.Text = string.Format("Refreshing item '{0}'...", itemReadEvent.FileName);

            this.mLogger.Debug("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path);

            this.mDebugForm.LogMessage(string.Format("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path));
        }

        private void BundleReaderOnReportRead(IBundleReader sender, ItemReadEvent itemReadEvent)
        {
            ListViewItem oItem = null;

            if (itemReadEvent.Success)
            {
                ImportStatus status = null;
                ReportItem item = this.mReportItemImporter.ImportItem(itemReadEvent.FileName, out status);

                if (status.Success)
                {
                    oItem = this.AddListViewItem_Success(itemReadEvent, status, item);
                }
                else
                {
                    oItem = this.AddListViewItem_ImportFailed(itemReadEvent, status);
                }
            }
            else
            {
                oItem = this.AddListViewItem_ExtractFailed(itemReadEvent);
            }

            oItem.SubItems.Add(itemReadEvent.Path);
            oItem.SubItems.Add(itemReadEvent.FileName);

            oItem.Group = this.lstSrcReports.Groups["reportsGroup"];

            this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
            this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

            this.lblStatus.Text = string.Format("Refreshing item '{0}'...", itemReadEvent.FileName);

            this.mLogger.Debug("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path);

            this.mDebugForm.LogMessage(string.Format("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path));
        }

        private void BundleReaderOnFolderRead(IBundleReader sender, ItemReadEvent itemReadEvent)
        {
            ListViewItem oItem = null;

            if (itemReadEvent.Success)
            {
                ImportStatus status = null;
                FolderItem item = this.mFolderItemImporter.ImportItem(itemReadEvent.FileName, out status);

                if (status.Success)
                {
                    oItem = this.AddListViewItem_Success(itemReadEvent, status, item);
                }
                else
                {
                    oItem = this.AddListViewItem_ImportFailed(itemReadEvent, status);
                }
            }
            else
            {
                oItem = this.AddListViewItem_ExtractFailed(itemReadEvent);
            }

            oItem.SubItems.Add(itemReadEvent.Path);
            oItem.SubItems.Add(itemReadEvent.FileName);

            oItem.Group = this.lstSrcReports.Groups["foldersGroup"];

            this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
            this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

            this.lblStatus.Text = string.Format("Refreshing item '{0}'...", itemReadEvent.FileName);

            this.mLogger.Debug("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path);

            this.mDebugForm.LogMessage(string.Format("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path));
        }

        //lstSrcReports Columns:
        //Name
        //Path
        //Error
        //ZipPath
        //ExtractedTo

        /// <summary>
        /// Adds an item to the ListView for a successfully 'imported' entry.
        /// </summary>
        /// <param name="e">The ItemReadEvent object for the entry imported.</param>
        /// <param name="status">The ImportStatus object for the entry imported.</param>
        /// <param name="item">The ReportServerItem object that was imported.</param>
        /// <returns></returns>
        private ListViewItem AddListViewItem_Success(ItemReadEvent e, ImportStatus status, ReportServerItem item)
        {
            ListViewItem oItem = new ListViewItem(item.Name);

            oItem.Tag = item;
            oItem.SubItems.Add(item.Path);
            oItem.SubItems.Add("");

            oItem.Checked = true;

            return oItem;
        }

        /// <summary>
        /// Adds an item to the Listview for an entry that was not successfully extracted to disk.
        /// </summary>
        /// <param name="e">The ItemReadEvent object for the entry that failed to extract.</param>
        /// <returns></returns>
        private ListViewItem AddListViewItem_ExtractFailed(ItemReadEvent e)
        {
            string name = this.mFileSystem.Path.GetFileNameWithoutExtension(e.FileName);
            string errors = string.Join("; ", e.Errors);

            ListViewItem oItem = new ListViewItem(name);

            oItem.Checked = false;
            oItem.ForeColor = Color.Red;

            //oItem.Tag = e.FileName;
            oItem.SubItems.Add("");
            oItem.SubItems.Add(errors);

            return oItem;
        }

        /// <summary>
        /// Adds an item to the ListView for an entry that was not successfully imported from disk.
        /// </summary>
        /// <param name="e">The ItemReadEvent object for the entry that failed to import from disk.</param>
        /// <param name="status">The ImportStatus object for the entry that failed to import from disk.</param>
        /// <returns></returns>
        private ListViewItem AddListViewItem_ImportFailed(ItemReadEvent e, ImportStatus status)
        {
            string name = this.mFileSystem.Path.GetFileNameWithoutExtension(e.FileName);

            ListViewItem oItem = new ListViewItem(name);

            oItem.Checked = false;
            oItem.ForeColor = Color.Red;

            //oItem.Tag = e.FileName;
            oItem.SubItems.Add(e.Path);
            oItem.SubItems.Add(status.Error.Message);

            return oItem;
        }
        #endregion

        #region Import Methods
        // Used for getting the ListView items from within the BackgroundWorker thread.
        private delegate ListView.ListViewItemCollection GetItems(ListView listView);

        // Used for getting the ListView items from within the BackgroundWorker thread.
        private ListView.ListViewItemCollection GetListViewItems(ListView listView)
        {
            ListView.ListViewItemCollection tmpListViewColl = new ListView.ListViewItemCollection(new ListView());

            if (!listView.InvokeRequired)
            {
                foreach (ListViewItem item in listView.Items)
                    tmpListViewColl.Add((ListViewItem)item.Clone());

                return tmpListViewColl;
            }
            else
                return (ListView.ListViewItemCollection)this.Invoke(new ImportZipForm.GetItems(GetListViewItems), new object[] { listView });
        }

        private void ImportZip()
        {
            this.lstDestReports.Items.Clear();

            try
            {
                this.btnPerformImport.Enabled = false;
                this.btnSrcRefreshReports.Enabled = false;

                this.mImportWorker.RunWorkerAsync(this.mDestinationRootPath);
            }
            catch (Exception er)
            {
                this.mLogger.Fatal(er, "Error importing items.");

                MessageBox.Show(
                    string.Format("Error importing items to '{0}':\n\r{1}", this.mDestinationRootPath, er.Message),
                    "Import Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ImportWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string destinationRootPath = (string)e.Argument;

            // Get the source root path from the bundle
            string sourceRootPath = this.mBundleReader.Summary.SourceRootPath;

            // If the source root path is empty or null, throw an exception
            if (string.IsNullOrEmpty(sourceRootPath))
            {
                this.mLogger.Fatal("ImportWorker - SourceRootPath is NULL or empty.");

                throw new Exception("Provided 'SourceRootPath' from bundle is empty.");
            }

            this.mLogger.Debug("ImportWorker - SourceRootPath = {0}", sourceRootPath);
            this.mLogger.Debug("ImportWorker - DestinationRootPath = {0}", this.mDestinationRootPath);

            // Stopwatch to track how long the import takes
            Stopwatch watch = new Stopwatch();

            // Start stopwatch to get how long it takes to get the total number of checked items
            watch.Start();

            IEnumerable<ListViewItem> lvItems = GetListViewItems(this.lstSrcReports).Cast<ListViewItem>();

            // Get total count of items in ListView that are checked
            int totalItems = lvItems.Where(lv => lv.Checked == true).Count();

            // Stop stopwatch after getting the total number of checked items, and log how long it took
            watch.Stop();
            this.mLogger.Debug("ImportWorker - Took {0} seconds to get checked ListView items", watch.Elapsed.TotalSeconds);

            // Start stopwatch to get how long it takes to import everything
            watch.Start();

            int progressCounter = 0;
            int reportsImportedCounter = 0;
            int reportsTotalCount = 0;
            int foldersImportedCounter = 0;
            int foldersTotalCount = 0;
            int dataSourcesImportedCounter = 0;
            int dataSourcesTotalCount = 0;
           
            // Import folders
            // Get path of ListView items in the folder group that are checked.
            var folderItems =
                (from lv in lvItems
                    where lv.Group.Name == "foldersGroup" &&
                          lv.Checked == true &&
                          lv.Tag != null    // We don't want anything with a NULL Tag
                    select new
                    {
                        Item = (FolderItem) lv.Tag,
                        ExtractedTo = lv.SubItems[4].Text
                    });

            foldersTotalCount = folderItems.Count();

            // Iterate through each item that was checked in the ListView's 'foldersGroup' group
            foreach (var folderItem in folderItems)
            {
                MigrationStatus status = new MigrationStatus()
                {
                    Success = false,
                    Item = folderItem.Item,
                    FromPath = folderItem.ExtractedTo
                };

                // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests_Destination/Data Sources'
                string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                    sourceRootPath, 
                    this.mDestinationRootPath,
                    folderItem.Item.Path);

                // Update the FolderItem.Path to be the new destination path
                folderItem.Item.Path = destItemPath;
                status.ToPath = destItemPath;

                this.mLogger.Debug("ImportWorker - FolderItem.FromPath = {0}; ToPath = {1}", 
                    status.FromPath, 
                    status.ToPath);

                try
                {
                    // Write the FolderItem to the server
                    string warning = this.mReportServerWriter.WriteFolder(folderItem.Item);
                    if (!string.IsNullOrEmpty(warning))
                        status.Warnings = new string[] { warning };

                    status.Success = true;

                    ++foldersImportedCounter;
                }
                catch (ItemAlreadyExistsException er)
                {
                    this.mLogger.Error(er, "Folder item already exists.");

                    status.Success = false;
                    status.Error = er;

                    this.mDebugForm.LogMessage(
                        string.Format("Folder can't be imported from '{0}' to '{1}', it already exists.",
                        status.FromPath,
                        status.ToPath),
                        er);
                }

                // Always report progress
                if (worker != null)
                    worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
                else
                {
                    this.mLogger.Warn("ImportWorker - worker is NULL.");
                }
            }

            // Import Data Sources
            // Get path of ListView items in the data sources group that are checked.
            var dataSourceItems =
                (from lv in lvItems
                 where lv.Group.Name == "dataSourcesGroup" &&
                       lv.Checked == true &&
                       lv.Tag != null    // We don't want anything with a NULL Tag
                 select new
                 {
                     Item = (DataSourceItem)lv.Tag,
                     ExtractedTo = lv.SubItems[4].Text
                 });

            dataSourcesTotalCount = dataSourceItems.Count();

            foreach (var dataSourceItem in dataSourceItems)
            {
                MigrationStatus status = new MigrationStatus()
                {
                    Success = false,
                    Item = dataSourceItem.Item,
                    FromPath = dataSourceItem.ExtractedTo
                };

                // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests_Destination/Data Sources/AWTestDataSource'
                string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                    sourceRootPath,
                    this.mDestinationRootPath,
                    dataSourceItem.Item.Path);

                // Update the FolderItem.Path to be the new destination path
                dataSourceItem.Item.Path = destItemPath;
                status.ToPath = destItemPath;

                this.mLogger.Debug("ImportWorker - DataSourceItem.FromPath = {0}; ToPath = {1}",
                    status.FromPath,
                    status.ToPath);

                try
                {
                    // Write the DataSourceItem to the server
                    string warning = this.mReportServerWriter.WriteDataSource(dataSourceItem.Item);
                    if (!string.IsNullOrEmpty(warning))
                        status.Warnings = new string[] { warning };

                    status.Success = true;

                    ++dataSourcesImportedCounter;
                }
                catch (ItemAlreadyExistsException er)
                {
                    this.mLogger.Error(er, "Data Source already exists.");

                    status.Success = false;
                    status.Error = er;

                    this.mDebugForm.LogMessage(
                        string.Format("Data Source can't be imported from '{0}' to '{1}', it already exists.",
                        status.FromPath,
                        status.ToPath),
                        er);
                }

                // Always report progress
                if (worker != null)
                    worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
                else
                {
                    this.mLogger.Warn("ImportWorker - worker is NULL.");
                }
            }

            // Import reports
            // Get path of ListView items in the reports group that are checked.
            var reportItems =
                (from lv in lvItems
                 where lv.Group.Name == "reportsGroup" &&
                       lv.Checked == true &&
                       lv.Tag != null    // We don't want anything with a NULL Tag
                 select new
                 {
                     Item = (ReportItem)lv.Tag,
                     ExtractedTo = lv.SubItems[4].Text
                 });

            reportsTotalCount = reportItems.Count();

            foreach (var reportItem in reportItems)
            {
                MigrationStatus status = new MigrationStatus()
                {
                    Success = false,
                    Item = reportItem.Item,
                    FromPath = reportItem.ExtractedTo
                };

                // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests_Destination/Reports/Company Sales'
                string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                    sourceRootPath,
                    this.mDestinationRootPath,
                    reportItem.Item.Path);

                // Update the FolderItem.Path to be the new destination path
                reportItem.Item.Path = destItemPath;
                status.ToPath = destItemPath;

                this.mLogger.Debug("ImportWorker - ReportItem.FromPath = {0}; ToPath = {1}",
                    status.FromPath,
                    status.ToPath);

                //if (reportItem.Item.Definition != null)
                //    this.mLogger.Debug("ImportWorker - ReportItem.Definition Before = {0}", 
                //        SSRSUtil.ByteArrayToString(reportItem.Item.Definition));

                // Update the ReportItem.Definition to point to the new server
                reportItem.Item.Definition = SSRSUtil.UpdateReportDefinition(
                    this.mDestinationServerUrl,
                    sourceRootPath,
                    this.mDestinationRootPath,
                    reportItem.Item.Definition
                    );

                // if (reportItem.Item.Definition != null)
                //    this.mLogger.Debug("ImportWorker - ReportItem.Definition After = {0}",
                //        SSRSUtil.ByteArrayToString(reportItem.Item.Definition));

                try
                {
                    // Write ReportItem to server
                    string[] warnings = this.mReportServerWriter.WriteReport(reportItem.Item);

                    if (warnings != null)
                        if (warnings.Length > 0)
                            status.Warnings = warnings;

                    status.Success = true;

                    ++reportsImportedCounter;
                }
                catch (ItemAlreadyExistsException er)
                {
                    this.mLogger.Error(er, "Report item already exists.");

                    status.Success = false;
                    status.Error = er;

                    this.mDebugForm.LogMessage(
                        string.Format("Report can't be migrated from '{0}' to '{1}', it already exists.",
                            status.FromPath,
                            status.ToPath),
                        er);
                }

                // Always report progress
                if (worker != null)
                    worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
                else
                {
                    this.mLogger.Warn("ImportWorker - worker is NULL.");
                }
            }

            // Stop stopwatch and get how long it took for the migration to complete successfully
            watch.Stop();
            double averageItem = watch.Elapsed.TotalSeconds / progressCounter;

            //string result = string.Format("{0}/{1} items imported in {2}h {3}m {4}s (@ {5:0.00} items/s)",
            //    (foldersMigratedCounter + dataSourcesMigratedCounter + reportsMigratedCounter),
            //    (foldersTotalCount + dataSourcesTotalCount + reportsTotalCount),
            //    watch.Elapsed.Hours,
            //    watch.Elapsed.Minutes,
            //    watch.Elapsed.Seconds,
            //    averageItem);

            string result = string.Format("{0}/{1} folders, {2}/{3} data sources, {4}/{5} reports imported in {6}h {7}m {8}s (@ {9:0.00} items/s)",
                foldersImportedCounter,
                foldersTotalCount,
                dataSourcesImportedCounter,
                dataSourcesTotalCount,
                reportsImportedCounter,
                reportsTotalCount,
                watch.Elapsed.Hours,
                watch.Elapsed.Minutes,
                watch.Elapsed.Seconds,
                averageItem);

            this.mLogger.Debug("ImportWorker - {0}", result);

            e.Result = result;
        }

        private void bw_ImportCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = null;

            if ((e.Cancelled == true))
            {
                msg = string.Format("Cancelled.");
            }
            else if ((e.Error != null))
            {
                msg = string.Format("Error. {0}", e.Error.Message);

                this.mLogger.Error(e.Error, "Error during import");

                this.mDebugForm.LogMessage(msg, e.Error);

                MessageBox.Show(msg,
                    "Import Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                msg = string.Format("Completed. {0}", e.Result);
            }

            this.mLogger.Info("Import completed: {0}", msg);
            this.lblStatus.Text = msg;
            this.btnSrcRefreshReports.Enabled = true;
            this.btnPerformImport.Enabled = false;
        }

        private void bw_ImportProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                MigrationStatus status = (MigrationStatus) e.UserState;

                if (status.Item != null)
                {
                    ReportServerItem item = (ReportServerItem) status.Item;

                    ListViewItem oItem = new ListViewItem(item.Name);
                    oItem.SubItems.Add(item.Path);

                    if (!status.Success)
                    {
                        oItem.SubItems.Add(status.Error.Message);
                        oItem.ForeColor = Color.Red;

                        oItem.ToolTipText = status.Error.Message;
                    }

                    if (status.Warnings.Length > 0)
                    {
                        string warnings = string.Join("; ", status.Warnings);

                        oItem.SubItems.Add(warnings);
                        oItem.ForeColor = Color.OrangeRed;

                        oItem.ToolTipText = string.Join("\n\r", status.Warnings);  
                    }

                    // Assign to proper ListViewGroup
                    if (item.GetType() == typeof(FolderItem))
                        oItem.Group = this.lstDestReports.Groups["foldersGroup"];
                    else if (item.GetType() == typeof(DataSourceItem))
                        oItem.Group = this.lstDestReports.Groups["dataSourcesGroup"];
                    else if (item.GetType() == typeof(ReportItem))
                        oItem.Group = this.lstDestReports.Groups["reportsGroup"];

                    this.lstDestReports.Items.Add(oItem);
                    oItem.EnsureVisible();

                    progressBar.ToolTipText = item.Name;

                    string msg = string.Format("Imported item from '{0}' to '{1}'...", 
                        status.FromPath,
                        status.ToPath);

                    this.mDebugForm.LogMessage(msg);
                    this.mLogger.Info("ImportProgressChanged - {0}", msg);
                    this.lblStatus.Text = string.Format("Imported item '{0}'...", item.Path);
                }
                else
                    this.mLogger.Warn("ImportProgressChanged - MigrationStatus.Item is NULL for item imported from '{0}' to '{1}'.", 
                        status.FromPath, 
                        status.ToPath);
            }

            progressBar.Value = e.ProgressPercentage;
            progressBar.Maximum = 100;
        }
        #endregion
    }
}
