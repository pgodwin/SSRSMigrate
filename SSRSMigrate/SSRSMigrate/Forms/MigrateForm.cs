using System;
using System.ComponentModel;
using System.Windows.Forms;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Writer;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using SSRSMigrate.Utility;
using SSRSMigrate.SSRS.Errors;

namespace SSRSMigrate.Forms
{
    [CoverageExclude]
    public partial class MigrateForm : Form
    {
        private readonly IReportServerReader mReportServerReader = null;
        private readonly IReportServerWriter mReportServerWriter = null;
        private readonly ILoggerFactory mLoggerFactory = null;
        private readonly string mSourceRootPath = null;
        private readonly string mSourceFullPath = null;
        private readonly string mDestinationRootPath = null;
        private readonly string mDestinationFullPath = null;

        private BackgroundWorker mSourceRefreshWorker = null;
        private BackgroundWorker mMigrationWorker = null;
        private ILogger mLogger = null;

        public MigrateForm(
            string sourceRootPath, 
            string sourceFullPath,
            string destinationRootPath,
            string destinationFullPath,
            IReportServerReader reader,
            IReportServerWriter writer,
            ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(sourceRootPath))
                throw new ArgumentException("sourceRootPath");

            if (string.IsNullOrEmpty(sourceFullPath))
                throw new ArgumentException("sourceFullPath");

            if (string.IsNullOrEmpty(destinationFullPath))
                throw new ArgumentException("destinationFullPath");

            if (string.IsNullOrEmpty(sourceFullPath))
                throw new ArgumentException("sourceFullPath");

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (writer == null)
                throw new ArgumentNullException("writer");

            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            InitializeComponent();

            this.mSourceRootPath = sourceRootPath;
            this.mSourceFullPath = sourceFullPath;
            this.mDestinationRootPath = destinationRootPath;
            this.mDestinationFullPath = destinationFullPath;
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
                msg = string.Format("Reporting listing error.", e.Error);
            }
            else
            {
                msg = string.Format("Completed.");

                this.btnPerformMigration.Enabled = true;
            }

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
        }

        private void btnPerformMigration_Click(object sender, EventArgs e)
        {
            // If there are no items in the list, there is nothing to migrate
            if (this.lstSrcReports.Items.Count <= 0)
                return;

            this.DirectMigration();
        }

        // Commented out because we can do these in a single method above.
        //private void ReportsReader_Report_Reporter(ReportItem item)
        //{
        //    ListViewItem oItem = new ListViewItem(item.Name);
        //    oItem.Checked = true;
        //    oItem.Tag = item;
        //    oItem.SubItems.Add(item.Path);

        //    this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
        //    this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

        //    this.lblStatus.Text = string.Format("Refreshing report '{0}'...", item.Path);
        //}

        //private void ReportsReader_DataSource_Reporter(DataSourceItem item)
        //{
        //    ListViewItem oItem = new ListViewItem(item.Name);
        //    oItem.Checked = true;
        //    oItem.Tag = item;
        //    oItem.SubItems.Add(item.Path);

        //    this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
        //    this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

        //    this.lblStatus.Text = string.Format("Refreshing data source '{0}'...", item.Path);
        //}

        //private void ReportsReader_Folder_Reporter(FolderItem item)
        //{
        //    ListViewItem oItem = new ListViewItem(item.Name);
        //    oItem.Checked = true;
        //    oItem.Tag = item;
        //    oItem.SubItems.Add(item.Path);

        //    this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
        //    this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

        //    this.lblStatus.Text = string.Format("Refreshing folder '{0}'...", item.Path);
        //}
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

            IEnumerable<ListViewItem> lvItems = GetListViewItems(this.lstSrcReports).Cast<ListViewItem>();

            // Get total count of items in ListView that are checked
            int totalItems = lvItems.Where(lv => lv.Checked == true).Count();
            int itemCounter = 0;

            // Export folders
            // Get path of ListView items in the folder group that are checked.
            var folderPaths = from lv in lvItems
                              where lv.Group.Name == "foldersGroup" &&
                              lv.Checked == true
                              select (string)lv.Tag;

            foreach (string folderPath in folderPaths)
            {
                FolderItem folderItem = null;

                if (!string.IsNullOrEmpty(folderPath))
                {
                    folderItem = this.mReportServerReader.GetFolder(folderPath);

                    if (folderItem != null)
                    {
                        // Get the destination path for this item (e.g. '/SSRSMigrate_AW_Tests/Data Sources' to '/SSRSMigrate_AW_Destination/Data Sources'
                        string destItemPath = SSRSUtil.GetFullDestinationPathForItem(
                            this.mSourceRootPath,
                            this.mDestinationRootPath,
                            folderItem.Path);

                        folderItem.Path = destItemPath;

                        try
                        {
                            this.mReportServerWriter.WriteFolder(folderItem);
                        }
                        catch (ItemAlreadyExistsException er)
                        {
                            this.mLogger.Error(er, "Folder item already exists.");

                            //TODO Should have some sort of event that ConnectInfoForm subscribes to in order to report errors to a debug window?
                        }
                    }
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++itemCounter * 100) / totalItems), folderItem);
            }

            // Export data sources
            var dataSourcePaths = from lv in lvItems
                                  where lv.Group.Name == "dataSourcesGroup" &&
                                  lv.Checked == true
                                  select (string)lv.Tag;

            foreach (string dataSourcePath in dataSourcePaths)
            {
                DataSourceItem dataSourceItem = null;

                if (!string.IsNullOrEmpty(dataSourcePath))
                {
                    dataSourceItem = this.mReportServerReader.GetDataSource(dataSourcePath);

                    if (dataSourceItem != null)
                    {
                        
                    }
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++itemCounter * 100) / totalItems), dataSourceItem);
            }

            // Export reports
            var reportPaths = from lv in lvItems
                              where lv.Group.Name == "reportsGroup" &&
                              lv.Checked == true
                              select (string)lv.Tag;

            foreach (string reportPath in reportPaths)
            {
                ReportItem reportItem = null;

                if (!string.IsNullOrEmpty(reportPath))
                {
                    reportItem = this.mReportServerReader.GetReport(reportPath);

                    if (reportItem != null)
                    {
                        
                    }
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++itemCounter * 100) / totalItems), reportItem);
            }
        }

        private void bw_MigrationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = null;

            if ((e.Cancelled == true))
            {
                msg = string.Format("Cancelled. {0}", e.Result);
            }
            else if ((e.Error != null))
            {
                msg = string.Format("Migration error.", e.Error);
            }
            else
            {
                msg = string.Format("Completed.");
            }

            this.btnSrcRefreshReports.Enabled = true;
            this.btnPerformMigration.Enabled = false;
        }

        private void bw_MigrationProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                ReportServerItem item = (ReportServerItem)e.UserState;

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

                this.lstDestReports.Items.Add(oItem);

                progressBar.ToolTipText = item.Name;
            }

            progressBar.Value = e.ProgressPercentage;
            progressBar.Maximum = 100;
        }
        #endregion
    }
}
