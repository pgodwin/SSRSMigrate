using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SSRSMigrate.Exporter;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.Status;
using SSRSMigrate.Utility;
using Ninject.Extensions.Logging;

namespace SSRSMigrate.Forms
{
    [CoverageExclude]
    public partial class ExportZipForm : Form
    {
        private readonly IReportServerReader mReportServerReader = null;
        //TODO These should be interfaces
        private readonly FolderItemExporter mFolderExporter = null;
        private readonly ReportItemExporter mReportExporter = null;
        private readonly DataSourceItemExporter mDataSourceExporter = null;
        private readonly IBundler mZipBundler = null;
        private readonly ILoggerFactory mLoggerFactory = null;

        private readonly string mSourceRootPath = null;
        private readonly string mExportDestinationFilename = null;
        private readonly string mExportOutputTempDirectory = null;

        private BackgroundWorker mSourceRefreshWorker = null;
        private BackgroundWorker mExportWorker = null;
        private ILogger mLogger = null;

        public ExportZipForm(string sourceRootPath,
            string destinationFilename,
            IReportServerReader reader,
            FolderItemExporter folderExporter,
            ReportItemExporter reportExporter,
            DataSourceItemExporter dataSourceExporter,
            IBundler zipBundler,
            ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(sourceRootPath))
                throw new ArgumentException("sourceRootPath");

            if (string.IsNullOrEmpty(destinationFilename))
                throw new ArgumentException("destinationFilename");

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (folderExporter == null)
                throw new ArgumentNullException("folderExporter");

            if (reportExporter == null)
                throw new ArgumentNullException("reportExporter");

            if (dataSourceExporter == null)
                throw new ArgumentNullException("dataSourceExporter");

            if (zipBundler == null)
                throw new ArgumentNullException("zipBundler");

            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            InitializeComponent();

            this.mSourceRootPath = sourceRootPath;
            this.mExportDestinationFilename = destinationFilename;
            this.mReportServerReader = reader;
            this.mFolderExporter = folderExporter;
            this.mReportExporter = reportExporter;
            this.mDataSourceExporter = dataSourceExporter;
            this.mZipBundler = zipBundler;
            this.mLoggerFactory = loggerFactory;

            this.mLogger = mLoggerFactory.GetCurrentClassLogger();

            this.mExportOutputTempDirectory = this.GetTemporaryExportOutputFolder("SSRSMigrate_ExportZip");
            this.CreateExportOutputFolder(this.mExportOutputTempDirectory);
        }

        private string GetTemporaryExportOutputFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return Path.Combine(Path.GetTempPath(), name);
        }

        private void CreateExportOutputFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
        }

        #region UI Events
        private void ExportDiskForm_Load(object sender, EventArgs e)
        {
            // Create BackgroundWorker that is used to read items from ReportServer
            this.mSourceRefreshWorker = new BackgroundWorker();
            this.mSourceRefreshWorker.WorkerReportsProgress = true;
            this.mSourceRefreshWorker.WorkerSupportsCancellation = true;
            this.mSourceRefreshWorker.DoWork += new DoWorkEventHandler(this.SourceRefreshReportsWorker);
            this.mSourceRefreshWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_SourceRefreshReportsCompleted);
            this.mSourceRefreshWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_SourceRefreshReportsProgressChanged);
        
            // Create BackgroundWorker that is used to export items to disk
            this.mExportWorker = new BackgroundWorker();
            this.mExportWorker.WorkerReportsProgress = true;
            this.mExportWorker.WorkerSupportsCancellation = true;
            this.mExportWorker.DoWork += new DoWorkEventHandler(this.ExportItemsWorker);
            this.mExportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_ExportItems_Completed);
            this.mExportWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_ExportItems_ProgressChanged);
        }

        private void ExportDiskForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If a report refresh is in progress, don't allow the form to close
            if (this.mSourceRefreshWorker != null)
                if (this.mSourceRefreshWorker.IsBusy)
                    e.Cancel = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // If there are no items in the list, there is nothing to export
            if (this.lstSrcReports.Items.Count <= 0)
                return;

            this.ExportToDisk();
        }

        private void btnSrcRefreshReports_Click(object sender, EventArgs e)
        {
            if (this.mSourceRefreshWorker.IsBusy)
            {
                MessageBox.Show("Refresh in progress. Please wait for it to finish.",
                    "Refresh In Progress",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);

                return;
            }

            this.SourceRefreshReports();
        }
        #endregion

        #region Source Server Reports Methods
        private void SourceRefreshReports()
        {
            this.lstSrcReports.Items.Clear();

            try
            {
                this.btnExport.Enabled = false;
                this.btnSrcRefreshReports.Enabled = false;
                this.mSourceRefreshWorker.RunWorkerAsync(this.mSourceRootPath);
            }
            catch (Exception er)
            {
                MessageBox.Show(string.Format("Error refreshing items at '{0}':\n\r{1}", this.mSourceRootPath,
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
            }
            else if ((e.Error != null))
            {
                msg = string.Format("Error getting item list:\n\r{0}", e.Error);

                MessageBox.Show(msg,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                msg = string.Format("Completed.");

                // Only allow exporting if the refresh completed without error 
                //  and there are items to export.
                if (this.lstSrcReports.Items.Count > 0)
                    this.btnExport.Enabled = true;
            }

            this.btnSrcRefreshReports.Enabled = true;
            this.lblStatus.Text = msg;
        }

        private void bw_SourceRefreshReportsProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReportItem report = (ReportItem)e.UserState;

            progressBar.Value = e.ProgressPercentage;
            progressBar.Maximum = 100;
            progressBar.ToolTipText = report.Name;
        }

        private void ReportsReader_Reporter(ReportServerItem item)
        {
            ListViewItem oItem = new ListViewItem(item.Name);
            oItem.Checked = true;
            //oItem.Tag = item;
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
        #endregion

        #region Export Methods
        private void ExportToDisk()
        {
            try
            {
                this.btnExport.Enabled = false;
                this.btnSrcRefreshReports.Enabled = false;
                this.mExportWorker.RunWorkerAsync(this.mExportOutputTempDirectory);
            }
            catch (Exception er)
            {
                MessageBox.Show(
                    string.Format("Error exporting items to '{0}':\n\r{1}", this.mExportDestinationFilename, er.Message),
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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

        private void ExportItemsWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string exportPath = (string)e.Argument;

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
                ExportStatus status = null;

                if (!string.IsNullOrEmpty(folderPath))
                {
                    FolderItem folderItem = this.mReportServerReader.GetFolder(folderPath);

                    if (folderItem != null)
                    {
                        // Path to where to export folder on disk
                        string saveFilePath = exportPath + SSRSUtil.GetServerPathToPhysicalPath(folderPath);

                        status = this.mFolderExporter.SaveItem(folderItem,
                            saveFilePath,
                            true);

                        // If the save to the temporary path was successful, add folder to the ZipBundler
                        if (status.Success)
                        {
                            this.mZipBundler.AddItem(
                                "Folders",
                                status.ToPath,
                                status.FromPath,
                                true);
                        }
                    }
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++itemCounter * 100) / totalItems), status);
            }

            // Export data sources
            var dataSourcePaths = from lv in lvItems
                                  where lv.Group.Name == "dataSourcesGroup" &&
                                  lv.Checked == true
                                  select (string)lv.Tag;

            foreach (string dataSourcePath in dataSourcePaths)
            {
                ExportStatus status = null;

                if (!string.IsNullOrEmpty(dataSourcePath))
                {
                    DataSourceItem dataSourceItem = this.mReportServerReader.GetDataSource(dataSourcePath);

                    if (dataSourceItem != null)
                    {
                        // Path to where to export data source on disk
                        string saveFilePath = exportPath + SSRSUtil.GetServerPathToPhysicalPath(dataSourcePath, "json");

                        status = this.mDataSourceExporter.SaveItem(dataSourceItem,
                            saveFilePath,
                            true);

                        // If the save to the temporary path was successful, add file to the ZipBundler
                        if (status.Success)
                        {
                            this.mZipBundler.AddItem(
                                "DataSources",
                                status.ToPath,
                                status.FromPath,
                                false);
                        }
                    }
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++itemCounter * 100) / totalItems), status);
            }

            // Export reports
            var reportPaths = from lv in lvItems
                                  where lv.Group.Name == "reportsGroup" &&
                                  lv.Checked == true
                                  select (string)lv.Tag;

            foreach (string reportPath in reportPaths)
            {
                ExportStatus status = null;

                if (!string.IsNullOrEmpty(reportPath))
                {
                    ReportItem reportItem = this.mReportServerReader.GetReport(reportPath);

                    if (reportItem != null)
                    {
                        // Path to where to export report on disk
                        string saveFilePath = exportPath + SSRSUtil.GetServerPathToPhysicalPath(reportPath, "rdl");

                        status = this.mReportExporter.SaveItem(reportItem,
                            saveFilePath,
                            true);

                        // If the save to the temporary path was successful, add file to the ZipBundler
                        if (status.Success)
                        {
                            this.mZipBundler.AddItem(
                                "Reports",
                                status.ToPath,
                                status.FromPath,
                                false);
                        }
                    }
                }

                // Always report progress, even if a ListViewItem has an empty path and even if the item isn't retrieved by ReportServerReader.
                // This will keep the progress bar value from suddenly jumping up several values.
                worker.ReportProgress(((++itemCounter * 100) / totalItems), status);
            }
        }

        private void bw_ExportItems_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            string msg = null;

            if ((e.Cancelled == true))
            {
                msg = string.Format("Cancelled. {0}", e.Result);
            }
            else if ((e.Error != null))
            {
                msg = string.Format("Error exporting items:\n\r{0}", e.Error);

                MessageBox.Show(msg,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                //TODO Should log failures and perhaps display them in a debugger window.
            }
            else
            {
                msg = string.Format("Completed. {0} items exported.", e.Result);   
                
                // If the export completed, create the summary and save the ZipBundler
                this.mZipBundler.CreateSummary();
                this.mZipBundler.Save(this.mExportDestinationFilename);
            }

            this.btnSrcRefreshReports.Enabled = true;
            this.btnExport.Enabled = false;
            this.lblStatus.Text = msg;
        }

        private void bw_ExportItems_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            this.progressBar.Maximum = 100;
            this.progressBar.ToolTipText = string.Format("{0} %", e.ProgressPercentage);

            if (e.UserState != null)
            {
                ExportStatus exportStatus = (ExportStatus)e.UserState;

                if (exportStatus.Success)
                    this.lblStatus.Text = string.Format("Saved '{0}'", exportStatus.ToPath);
                else
                {
                    this.lblStatus.Text = string.Format("Failed '{0}': ",
                        exportStatus.ToPath,
                        string.Join(",", exportStatus.Errors));

                    //TODO Should log failures and perhaps display them in a debugger window.
                }
            }
        }
        #endregion
    }
}
