using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate
{
    [CoverageExcludeAttribute]
    public partial class MigrateForm : Form
    {
        private static ILog Log = null;
        private readonly ReportServerReader mReportServerReader = null;
        //private readonly ReportServerWriter mReportServerWriter = null;
        private readonly string mSourceRootPath = null;
        private readonly string mDestinationRootPath = null;
        private BackgroundWorker mSourceRefreshWorker = null;

        public MigrateForm(string sourceRootPath, string destinationRootPath, ReportServerReader reader)
        {
            InitializeComponent();

            this.mSourceRootPath = sourceRootPath;
            this.mDestinationRootPath = destinationRootPath;
            this.mReportServerReader = reader;

            XmlConfigurator.Configure();
            Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
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

        private void ReportsReaderReporter(ReportItem item)
        {
            ListViewItem oItem = new ListViewItem(item.Name);
            oItem.Checked = true;
            oItem.Tag = item;
            oItem.SubItems.Add(item.Path);

            this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
            this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));
        }

        public void SourceRefreshReportsWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string path = (string)e.Argument;

            this.mReportServerReader.GetReports(path, ReportsReaderReporter);
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
                Log.Error("Reporting listing error.", e.Error);
            }
            else
            {
                msg = string.Format("Completed. {0}", e.Result);
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
        #endregion
    }
}
