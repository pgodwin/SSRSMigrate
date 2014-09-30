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
        private readonly string destinationServerUrl = null;

        private BackgroundWorker mSourceRefreshWorker = null;
        private BackgroundWorker mMigrationWorker = null;
        private ILogger mLogger = null;

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
            this.destinationServerUrl = destinationServerUrl;
            this.mReportServerReader = reader;
            this.mReportServerWriter = writer;
            this.mLoggerFactory = loggerFactory;

            this.mLogger = mLoggerFactory.GetCurrentClassLogger();
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
        }

        private void btnSrcRefreshReports_Click(object sender, EventArgs e)
        {
            this.SourceRefreshReports();
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
                this.mLogger.Error(er, "Error getting list of items from '{0}' on server '{1}'.}",
                    this.mSourceRootPath,
                    this.mSourceServerUrl);

                MessageBox.Show(
                   string.Format("Error getting list of items from '{0}' on server '{1}'.\n\r\n\r{2}", 
                        this.mSourceRootPath,
                        this.mSourceServerUrl,
                        er.Message),
                   "Migration Error",
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
            }
            else if ((e.Error != null))
            {
                msg = string.Format("{0}", e.Error.Message);

                this.mLogger.Error(e.Error, "Error during item refresh");

                MessageBox.Show(msg,
                    "Refresh Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                msg = string.Format("Completed.");

                this.btnPerformMigration.Enabled = true;
            }

            this.mLogger.Info("Item refresh completed: {0}", msg);
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
        }
        #endregion

        #region Migration Methods
        private void btnPerformMigration_Click(object sender, EventArgs e)
        {
            // If there are no items in the list, there is nothing to migrate
            if (this.lstSrcReports.Items.Count <= 0)
                return;

            this.DirectMigration();
        }

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
                MessageBox.Show(
                    string.Format("Error migrating items to '{0}':\n\r{1}", this.mDestinationRootPath, er.Message),
                    "Migration Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        //TODO Exception handling in MigrationWorker
        public void MigrationWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string destinationRootPath = (string)e.Argument;

            // Stopwatch to track how long the migration takes
            Stopwatch watch = new Stopwatch();

            // Start stopwatch to get how long it takes to get the total number of checked items
            watch.Start();

            IEnumerable<ListViewItem> lvItems = GetListViewItems(this.lstSrcReports).Cast<ListViewItem>();

            // Get total count of items in ListView that are checked
            int totalItems = lvItems.Where(lv => lv.Checked == true).Count();
            
            // Stop stopwatch after getting the total number of checked items, and log how long it took
            watch.Stop();
            this.mLogger.Trace("MigrationWorker - Took {0} seconds to get checked ListView items", watch.Elapsed.TotalSeconds);

            // Start stopwatch to get how long it takes to migrate everything
            watch.Start();
            int progressCounter = 0;

            // Export folders
            // Get path of ListView items in the folder group that are checked.
            var folderPaths = from lv in lvItems
                              where lv.Group.Name == "foldersGroup" &&
                              lv.Checked == true
                              select (string)lv.Tag;

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

                        // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests/Data Sources' to '/SSRSMigrate_AW_Destination/Data Sources'
                        string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            folderItem.Path);

                        folderItem.Path = destItemPath;
                        status.ToPath = destItemPath;

                        this.mLogger.Trace("MigrationWorker - FolderItem.FromPath = {0}; ToPath = {1}", status.FromPath, status.ToPath);

                        try
                        {
                            string warning = this.mReportServerWriter.WriteFolder(folderItem);

                            if (!string.IsNullOrEmpty(warning))
                                status.Warnings = new string[] { warning };

                            status.Success = true;
                        }
                        catch (ItemAlreadyExistsException er)
                        {
                            this.mLogger.Error(er, "Folder item already exists.");

                            status.Success = false;
                            status.Error = er;

                            //TODO Should have some sort of event that ConnectInfoForm subscribes to in order to report errors to a debug window?
                        }
                    }
                    else
                        this.mLogger.Warn("MigrationWorker - FolderItem for path '{0}' returned NULL.", folderPath);
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
            }

            // Export data sources
            var dataSourcePaths = from lv in lvItems
                                  where lv.Group.Name == "dataSourcesGroup" &&
                                  lv.Checked == true
                                  select (string)lv.Tag;

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

                        this.mLogger.Trace("MigrationWorker - DataSourceItem.FromPath = {0}; ToPath = {1}", status.FromPath, status.ToPath);

                        try
                        {
                            string warning = this.mReportServerWriter.WriteDataSource(dataSourceItem);

                            if (!string.IsNullOrEmpty(warning))
                                status.Warnings = new string[] { warning };

                            status.Success = true;
                        }
                        catch (ItemAlreadyExistsException er)
                        {
                            this.mLogger.Error(er, "Data Source item already exists.");

                            status.Success = false;
                            status.Error = er;

                            //TODO Should have some sort of event that ConnectInfoForm subscribes to in order to report errors to a debug window?
                        }
                    }
                    else
                        this.mLogger.Warn("MigrationWorker - DataSourceItem for path '{0}' returned NULL.", dataSourcePath);
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
            }

            // Export reports
            var reportPaths = from lv in lvItems
                              where lv.Group.Name == "reportsGroup" &&
                              lv.Checked == true
                              select (string)lv.Tag;

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

                        this.mLogger.Trace("MigrationWorker - ReportItem.FromPath = {0}; ToPath = {1}", status.FromPath, status.ToPath);

                        if (reportItem.Definition != null)
                            this.mLogger.Trace("MigrationWorker - ReportItem.Definition Before = {0}", SSRSUtil.ByteArrayToString(reportItem.Definition));

                        reportItem.Definition = SSRSUtil.UpdateReportDefinition(
                            this.destinationServerUrl,
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            reportItem.Definition);

                        if (reportItem.Definition != null)
                            this.mLogger.Trace("MigrationWorker - ReportItem.Definition After = {0}", SSRSUtil.ByteArrayToString(reportItem.Definition));

                        try
                        {
                            string[] warnings = this.mReportServerWriter.WriteReport(reportItem);

                            if (warnings != null)
                                if (warnings.Length > 0)
                                    status.Warnings = warnings;

                            status.Success = true;
                        }
                        catch (ItemAlreadyExistsException er)
                        {
                            this.mLogger.Error(er, "Report item already exists.");

                            status.Success = false;
                            status.Error = er;

                            //TODO Should have some sort of event that ConnectInfoForm subscribes to in order to report errors to a debug window?
                        }
                    }
                    else
                        this.mLogger.Warn("MigrationWorker - ReportItem for path '{0}' returned NULL.", reportPath);
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++progressCounter * 100) / totalItems), status);
            }

            // Stop stopwatch and get how long it took for the migration to complete successfully
            watch.Stop();
            double average_item = watch.Elapsed.TotalSeconds/progressCounter;

            string result = string.Format("{0} items migrated in {1}h {2}m {3}s (@ {4:0.00} items/s)", 
                progressCounter,
                watch.Elapsed.Hours,
                watch.Elapsed.Minutes,
                watch.Elapsed.Seconds,
                average_item);

            this.mLogger.Trace("MigrationWorker - {0}", result);

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
                msg = string.Format("{0}", e.Error.Message);
                
                this.mLogger.Error(e.Error, "Error during migration");

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

                    this.lblStatus.Text = string.Format("Migrated item at '{0}'...", item.Path);
                }
                else
                    this.mLogger.Warn("MigrationProgressChanged - MigrationStatus.Item is NULL for item migrated from '{0}' to '{1}'.", status.FromPath, status.ToPath);
            }

            progressBar.Value = e.ProgressPercentage;
            progressBar.Maximum = 100;
        }
        #endregion

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lstSrcReports.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = true);
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lstSrcReports.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = false);
        }
    }
}
