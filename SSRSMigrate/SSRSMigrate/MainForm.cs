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
using Ninject;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate
{
    [CoverageExcludeAttribute]
    public partial class MainForm : Form
    {
        private static ILog Log = null;

        private ReportServerReader mReportServerReader = null;

        // Source server 
        private BackgroundWorker mSourceRefreshWorker = null;

        public MainForm()
        {
            InitializeComponent();

            XmlConfigurator.Configure();
            Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        }

        #region UI Events
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Setup form default values
            this.cboSrcDefaultCred.SelectedIndex = 0;
            this.cboSrcVersion.SelectedIndex = 0;

            this.mSourceRefreshWorker = new BackgroundWorker();
            this.mSourceRefreshWorker.WorkerReportsProgress = true;
            this.mSourceRefreshWorker.WorkerSupportsCancellation = true;
            this.mSourceRefreshWorker.DoWork += new DoWorkEventHandler(this.SourceRefreshReportsWorker);
            this.mSourceRefreshWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_SourceRefreshReportsCompleted);
            this.mSourceRefreshWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_SourceRefreshReportsProgressChanged);

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void cboSrcDefaultCred_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboSrcDefaultCred.SelectedIndex == 0)
            {
                this.txtSrcDomain.Enabled = false;
                this.txtSrcUsername.Enabled = false;
                this.txtSrcPassword.Enabled = false;
            }
            else
            {
                this.txtSrcDomain.Enabled = true;
                this.txtSrcUsername.Enabled = true;
                this.txtSrcPassword.Enabled = true;
            }
        }

        private void btnSrcRefreshReports_Click(object sender, EventArgs e)
        {
            try
            {
                this.SourceCheckUI();
            }
            catch (Exception er)
            {
                MessageBox.Show(string.Format("Missing source server information: {0}", er.Message),
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            this.SourceRefreshReports();
        }
        #endregion

        #region Settings Methods
        private void LoadSettings()
        {

        }
        #endregion

        #region Source UI Check Methods
        private void SourceCheckUI()
        {
            if (string.IsNullOrEmpty(txtSrcUrl.Text))
                throw new Exception("url");

            if (this.cboSrcDefaultCred.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(txtSrcUsername.Text))
                    throw new Exception("username");

                if (string.IsNullOrEmpty(txtSrcPassword.Text))
                    throw new Exception("password");

                if (string.IsNullOrEmpty(txtSrcDomain.Text))
                    throw new Exception("domain");
            }

            if (string.IsNullOrEmpty(txtSrcPath.Text))
                txtSrcPath.Text = "/";
        }
        #endregion

        #region Source Source Reports Methods
        private void SourceRefreshReports()
        {
            this.lstSrcReports.Items.Clear();

            try
            {
                bool defaultCred = true;

                if (cboSrcDefaultCred.SelectedIndex == 0)
                    defaultCred = true;
                else
                    defaultCred = false;

                StandardKernel kernel = new StandardKernel(new DependencyModule(false, txtSrcPath.Text, txtSrcUrl.Text, defaultCred, txtSrcUsername.Text, txtSrcPassword.Text, txtSrcDomain.Text));
                this.mReportServerReader = kernel.Get<ReportServerReader>();

                this.mSourceRefreshWorker.RunWorkerAsync();
            }
            catch (Exception er)
            {

            }
        }

        public void SourceRefreshReportsWorker(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<ReportItem> reports = this.mReportServerReader.GetReports("/");
            int itemCount = reports.Count();

            foreach (ReportItem report in reports)
            {
                worker.ReportProgress(0, report);
            }
        }

        private void bw_SourceRefreshReportsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                string sMsg = string.Format("Cancelled. {0}", e.Result);

            }
            else if ((e.Error != null))
            {
                Log.Error("Reporting listing error.", e.Error);
            }
            else
            {
                string sMsg = string.Format("Completed. {0}", e.Result);
            }
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
