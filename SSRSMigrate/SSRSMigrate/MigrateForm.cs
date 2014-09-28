using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;
using log4net.Core;
using SSRSMigrate.SSRS.Writer;

namespace SSRSMigrate
{
    [CoverageExcludeAttribute]
    public partial class MigrateForm : Form
    {
        private readonly IReportServerReader mReportServerReader = null;
        private readonly IReportServerWriter mReportServerWriter = null;
        private readonly string mSourceRootPath = null;
        private readonly string mDestinationRootPath = null;

        private BackgroundWorker mSourceRefreshWorker = null;

        public MigrateForm(
            string sourceRootPath, 
            string destinationRootPath,
            IReportServerReader reader,
            IReportServerWriter writer)
        {
            if (string.IsNullOrEmpty(sourceRootPath))
                throw new ArgumentException("sourceRootPath");

            if (string.IsNullOrEmpty(destinationRootPath))
                throw new ArgumentException("destinationRootPath");

            if (reader == null)
                throw new ArgumentNullException("reader");

            if (writer == null)
                throw new ArgumentNullException("writer");

            InitializeComponent();

            this.mSourceRootPath = sourceRootPath;
            this.mDestinationRootPath = destinationRootPath;
            this.mReportServerReader = reader;
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
            }

            this.btnSrcRefreshReports.Enabled = true;
        }

        private void bw_SourceRefreshReportsProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReportItem report = (ReportItem)e.UserState;

            ListViewItem oItem = new ListViewItem(report.Name);
            oItem.Checked = true;
            oItem.Tag = report;
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
            oItem.Tag = item;
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
    }
}
