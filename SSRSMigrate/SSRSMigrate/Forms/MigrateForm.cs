using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Writer;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using SSRSMigrate.Utility;
using SSRSMigrate.SSRS.Errors;
using SSRSMigrate.Status;

namespace SSRSMigrate.Forms
{
    [CoverageExclude]
    public partial class MigrateForm : Form
    {
        private readonly IReportServerReader mReportServerReader = null;
        private readonly IReportServerWriter mReportServerWriter = null;
        private readonly ILoggerFactory mLoggerFactory = null;
        private readonly string mSourceRootPath = null;
        private readonly string mSourceServerUrl = null;
        private readonly string mDestinationRootPath = null;
        private readonly string mDestinationServerUrl = null;

        private BackgroundWorker mSourceRefreshWorker = null;
        private BackgroundWorker mMigrationWorker = null;
        private ILogger mLogger = null;

        private DebugForm mDebugForm = null;
        private SummaryForm mSummaryForm = null;

        #region Properties
        public DebugForm DebugForm
        {
            set { this.mDebugForm = value; }
        }
        #endregion

        public MigrateForm(
            string sourceRootPath, 
            string sourceServerUrl,
            string destinationRootPath,
            string destinationServerUrl,
            IReportServerReader reader,
            IReportServerWriter writer,
            ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(sourceRootPath))
                throw new ArgumentException("sourceRootPath");

            if (string.IsNullOrEmpty(sourceServerUrl))
                throw new ArgumentException("sourceServerUrl");

            if (string.IsNullOrEmpty(destinationServerUrl))
                throw new ArgumentException("destinationServerUrl");

            if (string.IsNullOrEmpty(sourceServerUrl))
                throw new ArgumentException("sourceServerUrl");

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (writer == null)
                throw new ArgumentNullException("writer");

            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            InitializeComponent();

            this.mSourceRootPath = sourceRootPath;
            this.mSourceServerUrl = sourceServerUrl;
            this.mDestinationRootPath = destinationRootPath;
            this.mDestinationServerUrl = destinationServerUrl;
            this.mReportServerReader = reader;
            this.mReportServerWriter = writer;
            this.mLoggerFactory = loggerFactory;

            this.mLogger = mLoggerFactory.GetCurrentClassLogger();

            this.mLogger.Info("sourceRootPath: {0}", this.mSourceRootPath);
            this.mLogger.Info("sourceServerUrl: {0}", this.mSourceServerUrl);
            this.mLogger.Info("destinationRootPath: {0}", this.mDestinationRootPath);
            this.mLogger.Info("destinationServerUrl: {0}", this.mDestinationServerUrl);
            this.mSummaryForm = new SummaryForm();
        }

        #region UI Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.mSourceRefreshWorker = new BackgroundWorker();
            this.mSourceRefreshWorker.WorkerReportsProgress = true;
            this.mSourceRefreshWorker.WorkerSupportsCancellation = true;
            this.mSourceRefreshWorker.DoWork += new DoWorkEventHandler(this.SourceRefreshReportsWorker);
            this.mSourceRefreshWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_SourceRefreshReportsCompleted);
            this.mSourceRefreshWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_SourceRefreshReportsProgressChanged);

            this.mMigrationWorker = new BackgroundWorker();
            this.mMigrationWorker.WorkerReportsProgress = true;
            this.mMigrationWorker.WorkerSupportsCancellation = true;
            this.mMigrationWorker.DoWork += new DoWorkEventHandler(this.MigrationWorker);
            this.mMigrationWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_MigrationCompleted);
            this.mMigrationWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_MigrationProgressChanged);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If a report refresh is in progress, don't allow the form to close
            if (this.mSourceRefreshWorker != null)
                if (this.mSourceRefreshWorker.IsBusy)
                    e.Cancel = true;

            // If the migration is in progress, don't allow the form to close
            if (this.mMigrationWorker != null)
                if (this.mMigrationWorker.IsBusy)
                    e.Cancel = true;

            if (this.mSourceRefreshWorker != null)
            {
                this.mSourceRefreshWorker.DoWork -= new DoWorkEventHandler(this.SourceRefreshReportsWorker);
                this.mSourceRefreshWorker.RunWorkerCompleted -=
                    new RunWorkerCompletedEventHandler(this.bw_SourceRefreshReportsCompleted);
                this.mSourceRefreshWorker.ProgressChanged -=
                    new ProgressChangedEventHandler(this.bw_SourceRefreshReportsProgressChanged);
            }

            if (this.mMigrationWorker != null)
            {
                this.mMigrationWorker.DoWork -= new DoWorkEventHandler(this.MigrationWorker);
                this.mMigrationWorker.RunWorkerCompleted -=
                    new RunWorkerCompletedEventHandler(this.bw_MigrationCompleted);
                this.mMigrationWorker.ProgressChanged -=
                    new ProgressChangedEventHandler(this.bw_MigrationProgressChanged);
            }
        }

        private void btnPerformMigration_Click(object sender, EventArgs e)
        {
            // If there are no items in the list, there is nothing to migrate
            if (this.lstSrcReports.Items.Count <= 0)
                return;

            this.DirectMigration();
        }

        private void btnSrcRefreshReports_Click(object sender, EventArgs e)
        {
            this.SourceRefreshReports();
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lstSrcReports.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = true);
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lstSrcReports.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = false);
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            if (this.mDebugForm.Visible)
                this.mDebugForm.Hide();
            else
                this.mDebugForm.Show();
        }
        #endregion

        #region Source Source Reports Methods
        private void SourceRefreshReports()
        {
            this.lstSrcReports.Items.Clear();

            try
            {
                this.btnSrcRefreshReports.Enabled = false;
                this.btnPerformMigration.Enabled = false;
                this.mSourceRefreshWorker.RunWorkerAsync(this.mSourceRootPath);
            }
            catch (Exception er)
            {
                string msg = string.Format("Error getting list of items from '{0}' on server '{1}'.",
                    this.mSourceRootPath,
                    this.mSourceServerUrl);

                this.mDebugForm.LogMessage(msg, er);

                this.mLogger.Error(er, msg);

                MessageBox.Show(
                   string.Format("Error getting list of items from '{0}' on server '{1}'.\n\r\n\r{2}", 
                        this.mSourceRootPath,
                        this.mSourceServerUrl,
                        er.Message),
                   "Refresh Error",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
        }

        public void SourceRefreshReportsWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string path = (string)e.Argument;

            // Get folders from the specified path and add them to the Reports ListView control
            this.mReportServerReader.GetFolders(path, ReportsReader_Reporter);

            // Get data sources from the specified path and add them to the Reports ListView control
            this.mReportServerReader.GetDataSources(path, ReportsReader_Reporter);

            // Get reports from the specified path and add them to the Reports ListView control
            this.mReportServerReader.GetReports(path, ReportsReader_Reporter);
        }

        private void bw_SourceRefreshReportsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = null;

            if ((e.Cancelled == true))
            {
                msg = string.Format("Cancelled. {0}", e.Result);

                this.mDebugForm.LogMessage(msg);
            }
            else if ((e.Error != null))
            {
                msg = string.Format("Error. {0}", e.Error.Message);

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

                // Only allow migration if the refresh completed without error
                //  and there are items to migrate
                if (this.lstSrcReports.Items.Count > 0)
                    this.btnPerformMigration.Enabled = true;

                this.mDebugForm.LogMessage(msg);
            }
 
            this.mLogger.Info("Item refresh: {0}", msg);
            this.lblStatus.Text = msg;
            this.btnSrcRefreshReports.Enabled = true;
        }

        private void bw_SourceRefreshReportsProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReportServerItem report = (ReportServerItem)e.UserState;

            ListViewItem oItem = new ListViewItem(report.Name);
            oItem.Checked = true;
            oItem.Tag = report.Path;
            oItem.SubItems.Add(report.Path);

            lstSrcReports.Items.Add(oItem);

            progressBar.Value = e.ProgressPercentage;
            progressBar.Maximum = 100;
            progressBar.ToolTipText = report.Name;
        }

        // Reporters
        private void ReportsReader_Reporter(ReportServerItem item)
        {
            if (item == null)
            {
                this.mLogger.Warn("ReportsReader_Reporter - item contains a NULL value.");

                return;
            }

            ListViewItem oItem = new ListViewItem(item.Name);
            oItem.Checked = true;
            oItem.Tag = item.Path;
            oItem.SubItems.Add(item.Path);

            // Assign to proper ListViewGroup
            if (item.GetType() == typeof(FolderItem))
                oItem.Group = this.lstSrcReports.Groups["foldersGroup"];
            else if (item.GetType() == typeof(DataSourceItem))
                oItem.Group = this.lstSrcReports.Groups["dataSourcesGroup"];
            else if (item.GetType() == typeof(ReportItem))
                oItem.Group = this.lstSrcReports.Groups["reportsGroup"];

            this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
            this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

            this.lblStatus.Text = string.Format("Refreshing item '{0}'...", item.Path);

            this.mLogger.Debug("Refreshing item '{0}' on server '{1}'...", item.Path, this.mSourceServerUrl);

            this.mDebugForm.LogMessage(string.Format("Refreshing item '{0}' on server '{1}'...", item.Path, this.mSourceServerUrl));
        }
        #endregion

        #region Migration Methods
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
                return (ListView.ListViewItemCollection)this.Invoke(new GetItems(GetListViewItems), new object[] { listView });
        }

        private void DirectMigration()
        {
            this.lstDestReports.Items.Clear();

            try
            {
                this.btnPerformMigration.Enabled = false;
                this.btnSrcRefreshReports.Enabled = false;

                this.mMigrationWorker.RunWorkerAsync(this.mDestinationRootPath);
            }
            catch (Exception er)
            {
                this.mLogger.Fatal(er, "Error migrating items.");

                MessageBox.Show(
                    string.Format("Error migrating items to '{0}':\n\r{1}", this.mDestinationRootPath, er.Message),
                    "Migration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void MigrationWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string destinationRootPath = (string)e.Argument;

            this.mLogger.Debug("MigrationWorker - destinationRootPath: {0}", destinationRootPath);

            // Stopwatch to track how long the migration takes
            Stopwatch watch = new Stopwatch();

            // Start stopwatch to get how long it takes to get the total number of checked items
            watch.Start();

            IEnumerable<ListViewItem> lvItems = GetListViewItems(this.lstSrcReports).Cast<ListViewItem>();

            // Get total count of items in ListView that are checked
            int totalItems = lvItems.Where(lv => lv.Checked == true).Count();
            
            // Stop stopwatch after getting the total number of checked items, and log how long it took
            watch.Stop();
            this.mLogger.Debug("MigrationWorker - Took {0} seconds to get checked ListView items", watch.Elapsed.TotalSeconds);

            // Start stopwatch to get how long it takes to migrate everything
            watch.Start();

            int progressCounter = 0;
            int reportsMigratedCounter = 0;
            int reportsTotalCount = 0;
            int foldersMigratedCounter = 0;
            int foldersTotalCount = 0;
            int dataSourcesMigratedCounter = 0;
            int dataSourcesTotalCount = 0;

            // Export folders
            // Get path of ListView items in the folder group that are checked.
            var folderPaths = from lv in lvItems
                              where lv.Group.Name == "foldersGroup" &&
                              lv.Checked == true
                              select (string)lv.Tag;

            foldersTotalCount = folderPaths.Count();

            foreach (string folderPath in folderPaths)
            {
                FolderItem folderItem = null;
                MigrationStatus status = new MigrationStatus()
                {
                    Success = false
                };

                if (!string.IsNullOrEmpty(folderPath))
                {
                    folderItem = this.mReportServerReader.GetFolder(folderPath);

                    if (folderItem != null)
                    {
                        status.Item = folderItem;
                        status.FromPath = folderItem.Path;

                        this.mLogger.Debug("MigrationWorker - BEFORE FolderItem.FromPath = {0}; SourceRootPath = {1}", status.FromPath, this.mSourceRootPath);
                        this.mLogger.Debug("MigrationWorker - BEFORE FolderItem.FromPath = {0}; DestinationRootPath = {1}", status.FromPath, this.mDestinationRootPath);

                        // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests/Data Sources' to '/SSRSMigrate_AW_Destination/Data Sources'
                        string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            folderItem.Path);

                        this.mLogger.Debug("MigrationWorker - AFTER FolderItem.FromPath = {0}; destItemPath = {1}", status.FromPath, destItemPath);

                        folderItem.Path = destItemPath;
                        status.ToPath = destItemPath;

                        this.mLogger.Debug("MigrationWorker - FolderItem.FromPath = {0}; ToPath = {1}", status.FromPath, status.ToPath);

                        try
                        {
                            string warning = this.mReportServerWriter.WriteFolder(folderItem);

                            if (!string.IsNullOrEmpty(warning))
                                status.Warnings = new string[] { warning };

                            status.Success = true;

                            ++foldersMigratedCounter;
                        }
                        catch (ItemAlreadyExistsException er)
                        {
                            this.mLogger.Error(er, "Folder item already exists.");

                            status.Success = false;
                            status.Error = er;

                            this.mDebugForm.LogMessage(
                                string.Format("Folder can't be migrated from '{0}' to '{1}', it already exists.", 
                                    status.FromPath,
                                    status.ToPath),
                                er);
                        }
                    }
                    else
                        this.mLogger.Warn("MigrationWorker - FolderItem for path '{0}' returned NULL.", folderPath);
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                if (worker != null)
                    worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
                else
                {
                    this.mLogger.Warn("MigrationWorker - worker is NULL.");
                }
            }

            // Export data sources
            var dataSourcePaths = from lv in lvItems
                                  where lv.Group.Name == "dataSourcesGroup" &&
                                  lv.Checked == true
                                  select (string)lv.Tag;

            dataSourcesTotalCount = dataSourcePaths.Count();

            foreach (string dataSourcePath in dataSourcePaths)
            {
                DataSourceItem dataSourceItem = null;
                MigrationStatus status = new MigrationStatus()
                {
                    Success = false
                };

                if (!string.IsNullOrEmpty(dataSourcePath))
                {
                    dataSourceItem = this.mReportServerReader.GetDataSource(dataSourcePath);

                    if (dataSourceItem != null)
                    {
                        status.Item = dataSourceItem;
                        status.FromPath = dataSourceItem.Path;

                        // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests/Data Sources/AWDataSource' to '/SSRSMigrate_AW_Destination/Data Sources/AWDataSource'
                        string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            dataSourceItem.Path);

                        dataSourceItem.Path = destItemPath;
                        status.ToPath = destItemPath;

                        this.mLogger.Debug("MigrationWorker - DataSourceItem.FromPath = {0}; ToPath = {1}", status.FromPath, status.ToPath);

                        try
                        {
                            string warning = this.mReportServerWriter.WriteDataSource(dataSourceItem);

                            if (!string.IsNullOrEmpty(warning))
                                status.Warnings = new string[] { warning };

                            status.Success = true;

                            ++dataSourcesMigratedCounter;
                        }
                        catch (ItemAlreadyExistsException er)
                        {
                            this.mLogger.Error(er, "Data Source item already exists.");

                            status.Success = false;
                            status.Error = er;

                            this.mDebugForm.LogMessage(
                                string.Format("Data Source can't be migrated from '{0}' to '{1}', it already exists.",
                                    status.FromPath,
                                    status.ToPath),
                                er);
                        }
                    }
                    else
                        this.mLogger.Warn("MigrationWorker - DataSourceItem for path '{0}' returned NULL.", dataSourcePath);
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                if (worker != null)
                    worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
                else
                {
                    this.mLogger.Warn("MigrationWorker - worker is NULL.");
                }
            }

            // Export reports
            var reportPaths = from lv in lvItems
                              where lv.Group.Name == "reportsGroup" &&
                              lv.Checked == true
                              select (string)lv.Tag;

            reportsTotalCount = reportPaths.Count();

            foreach (string reportPath in reportPaths)
            {
                ReportItem reportItem = null;
                MigrationStatus status = new MigrationStatus()
                {
                    Success = false
                };

                if (!string.IsNullOrEmpty(reportPath))
                {
                    reportItem = this.mReportServerReader.GetReport(reportPath);

                    if (reportItem != null)
                    {
                        status.Item = reportItem;
                        status.FromPath = reportItem.Path;

                        // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests/Reports/Company Sales' to '/SSRSMigrate_AW_Destination/Reports/Company Sales'
                        string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            reportItem.Path);

                        reportItem.Path = destItemPath;
                        status.ToPath = destItemPath;

                        this.mLogger.Debug("MigrationWorker - ReportItem.FromPath = {0}; ToPath = {1}", status.FromPath, status.ToPath);

                        //if (reportItem.Definition != null)
                        //    this.mLogger.Debug("MigrationWorker - ReportItem.Definition Before = {0}", SSRSUtil.ByteArrayToString(reportItem.Definition));

                        reportItem.Definition = SSRSUtil.UpdateReportDefinition(
                            this.mDestinationServerUrl,
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            reportItem.Definition);

                        //if (reportItem.Definition != null)
                        //    this.mLogger.Debug("MigrationWorker - ReportItem.Definition After = {0}", SSRSUtil.ByteArrayToString(reportItem.Definition));

                        try
                        {
                            string[] warnings = this.mReportServerWriter.WriteReport(reportItem);

                            if (warnings != null)
                                if (warnings.Length > 0)
                                    status.Warnings = warnings;

                            status.Success = true;

                            ++reportsMigratedCounter;
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
                    }
                    else
                        this.mLogger.Warn("MigrationWorker - ReportItem for path '{0}' returned NULL.", reportPath);
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                if (worker != null)
                    worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
                else
                {
                    this.mLogger.Warn("MigrationWorker - worker is NULL.");
                }
            }

            // Stop stopwatch and get how long it took for the migration to complete successfully
            watch.Stop();
            double averageItem = watch.Elapsed.TotalSeconds / progressCounter;

            //string result = string.Format("{0}/{1} items migrated in {2}h {3}m {4}s (@ {5:0.00} items/s)",
            //    (foldersMigratedCounter + dataSourcesMigratedCounter + reportsMigratedCounter),
            //    (foldersTotalCount + dataSourcesTotalCount + reportsTotalCount),
            //    watch.Elapsed.Hours,
            //    watch.Elapsed.Minutes,
            //    watch.Elapsed.Seconds,
            //    averageItem);

            string result = string.Format("{0}/{1} folders, {2}/{3} data sources, {4}/{5} reports migrated in {6}h {7}m {8}s (@ {9:0.00} items/s)",
                foldersMigratedCounter,
                foldersTotalCount,
                dataSourcesMigratedCounter,
                dataSourcesTotalCount,
                reportsMigratedCounter,
                reportsTotalCount,
                watch.Elapsed.Hours,
                watch.Elapsed.Minutes,
                watch.Elapsed.Seconds,
                averageItem);

            this.mLogger.Debug("MigrationWorker - {0}", result);

            e.Result = result;
        }

        private void bw_MigrationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = null;

            if ((e.Cancelled == true))
            {
                msg = string.Format("Cancelled.");
            }
            else if ((e.Error != null))
            {
                msg = string.Format("Error. {0}", e.Error.Message);
                
                this.mLogger.Error(e.Error, "Error during migration");

                this.mDebugForm.LogMessage(msg, e.Error);

                MessageBox.Show(msg,
                    "Migration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                msg = string.Format("Completed. {0}", e.Result);
            }

            this.mLogger.Info("Migration completed: {0}", msg);
            this.lblStatus.Text = msg;
            this.btnSrcRefreshReports.Enabled = true;
            this.btnPerformMigration.Enabled = false;

            this.mSummaryForm.ShowDialog();
        }

        private void bw_MigrationProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                MigrationStatus status = (MigrationStatus) e.UserState;

                if (status.Item != null)
                {
                    ReportServerItem item = (ReportServerItem) status.Item;

                    ListViewItem oItem = new ListViewItem(item.Name);
                    oItem.Checked = true;
                    oItem.Tag = item.Path;
                    oItem.SubItems.Add(item.Path);

                    if (!status.Success)
                    {
                        oItem.SubItems.Add(status.Error.Message);
                        oItem.ForeColor = Color.Red;

                        oItem.ToolTipText = status.Error.Message;

                        this.mSummaryForm.AddFailedItem(item.Path,
                            status.Error.Message);
                    }
                    else
                    {
                        this.mSummaryForm.IncrementSuccessfulItemsCount();
                    }

                    if (status.Warnings.Length > 0)
                    {
                        string warnings = string.Join("; ", status.Warnings);

                        oItem.SubItems.Add(warnings);
                        oItem.ForeColor = Color.OrangeRed;

                        oItem.ToolTipText = string.Join("\n\r", status.Warnings);  
                    }

                    // Assign to proper ListViewGroup
                    if (item.GetType() == typeof (FolderItem))
                        oItem.Group = this.lstDestReports.Groups["foldersGroup"];
                    else if (item.GetType() == typeof (DataSourceItem))
                        oItem.Group = this.lstDestReports.Groups["dataSourcesGroup"];
                    else if (item.GetType() == typeof (ReportItem))
                        oItem.Group = this.lstDestReports.Groups["reportsGroup"];

                    this.lstDestReports.Items.Add(oItem);
                    oItem.EnsureVisible();

                    progressBar.ToolTipText = item.Name;

                    string msg = string.Format("Migrated item from '{0}' to '{1}'...", 
                        status.FromPath,
                        status.ToPath);

                    this.mDebugForm.LogMessage(msg);
                    this.mLogger.Info("MigrationProgressChanged - {0}", msg);
                    this.lblStatus.Text = string.Format("Migrated item '{0}'...", item.Path);

                    this.mSummaryForm.IncrementTotalItemsCount();
                }
                else
                    this.mLogger.Warn("MigrationProgressChanged - MigrationStatus.Item is NULL for item migrated from '{0}' to '{1}'.", status.FromPath, status.ToPath);
            }

            progressBar.Value = e.ProgressPercentage;
            progressBar.Maximum = 100;
        }
        #endregion
    }
}
