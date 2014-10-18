using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ninject.Extensions.Logging;
using SSRSMigrate.Bundler;
using SSRSMigrate.Bundler.Events;
using SSRSMigrate.SSRS.Writer;

namespace SSRSMigrate.Forms
{
    [CoverageExclude]
    public partial class ImportZipForm : Form
    {
        private readonly IBundleReader mBundleReader = null;
        //private readonly IBundleItemReader mBundleItemReader = null;
        private readonly IReportServerWriter mReportServerWriter = null;
        private ILoggerFactory mLoggerFactory = null;
        private readonly string mSourceFileName = null;
        private readonly string mDestinationRootPath = null;
        private readonly string mDestinationServerUrl = null;

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
            ILoggerFactory loggerFactory)
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

            InitializeComponent();

            this.mSourceFileName = sourceFileName;
            this.mDestinationRootPath = destinationRootPath;
            this.mDestinationServerUrl = destinationServerUrl;
            this.mBundleReader = bundleReader;
            this.mReportServerWriter = writer;
            this.mLoggerFactory = loggerFactory;
            this.mLogger = this.mLoggerFactory.GetCurrentClassLogger();
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

            //this.mImportWorker = new BackgroundWorker();
            //this.mImportWorker.WorkerReportsProgress = true;
            //this.mImportWorker.WorkerSupportsCancellation = true;
            //this.mImportWorker.DoWork += new DoWorkEventHandler(this.MigrationWorker);
            //this.mImportWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_MigrationCompleted);
            //this.mImportWorker.ProgressChanged += new ProgressChangedEventHandler(this.bw_MigrationProgressChanged);

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
        }

        private void btnPerformImport_Click(object sender, EventArgs e)
        {

        }

        private void btnDebug_Click(object sender, EventArgs e)
        {

        }

        private void btnSrcRefreshReports_Click(object sender, EventArgs e)
        {

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
                   string.Format("Error getting list of items from '{0}' archive.\n\r\n\r{2}",
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
            ListViewItem oItem = new ListViewItem(itemReadEvent.FileName);
            oItem.Checked = true;
            oItem.Tag = itemReadEvent.Path;
            oItem.SubItems.Add(itemReadEvent.Path);
            oItem.Group = this.lstSrcReports.Groups["dataSourcesGroup"];

            this.lstSrcReports.Invoke(new Action(() => this.lstSrcReports.Items.Add(oItem)));
            this.lstSrcReports.Invoke(new Action(() => oItem.EnsureVisible()));

            this.lblStatus.Text = string.Format("Refreshing item '{0}'...", itemReadEvent.FileName);

            this.mLogger.Debug("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path);

            this.mDebugForm.LogMessage(string.Format("Refreshing item '{0}' in path '{1}'...", itemReadEvent.FileName, itemReadEvent.Path));
        
        }


        private void BundleReaderOnReportRead(IBundleReader sender, ItemReadEvent itemReadEvent)
        {
        }

        private void BundleReaderOnFolderRead(IBundleReader sender, ItemReadEvent itemReadEvent)
        {
        }
        #endregion

        private void btnSrcRefreshReports_Click_1(object sender, EventArgs e)
        {
            this.SourceRefreshReports();
        }

        #region Import Methods
        #endregion
    }
}
