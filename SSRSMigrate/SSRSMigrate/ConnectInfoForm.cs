using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SSRSMigrate.SSRS.Reader;
using Ninject;
using SSRSMigrate.Factory;
using SSRSMigrate.SSRS.Repository;
using SSRSMigrate.Exporter;
using Ninject.Extensions.Logging.Log4net;
using log4net.Config;

namespace SSRSMigrate
{
    public partial class ConnectInfoForm : Form
    {
        private StandardKernel mKernel = null;

        public ConnectInfoForm()
        {
            XmlConfigurator.Configure();

            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            this.mKernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new ReportServerRepositoryModule());

            InitializeComponent();
        }

        #region Migrate Method Radio Events
        private void rdoMethodDirect_MouseHover(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "Perform a direct migration of items on one SSRS server to another SSRS server.";
        }

        private void rdoMethodDirect_MouseLeave(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "";
        }

        private void rdoMethodExportZip_MouseHover(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "Perform a migration by exporting of items to a single ZIP archive that can be imported at a destination server at a later date.";
        }

        private void rdoMethodExportZip_MouseLeave(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "";
        }

        private void rdoMethodExportDisk_MouseHover(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "Export all items to a directory on disk.";
        }

        private void rdoMethodExportDisk_MouseLeave(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "";
        }

        private void rdoMethodImportZip_MouseHover(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "Import previously exported items from a specified ZIP archive.";
        }

        private void rdoMethodImportZip_MouseLeave(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "";
        }

        private void rdoMethodDirect_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoMethodDirect.Checked)
            {
                this.grpDestServer.Visible = true;
            }
            else
            {
                this.grpDestServer.Visible = false;
            }
        }

        private void rdoMethodExportDisk_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoMethodExportDisk.Checked)
            {
                this.grpExportDisk.Visible = true;
            }
            else
            {
                this.grpExportDisk.Visible = false;
            }
        }

        private void rdoMethodExportZip_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoMethodExportZip.Checked)
            {
                this.grpExportZip.Visible = true;
            }
            else
            {
                this.grpExportZip.Visible = false;
            }
        }

        private void rdoMethodImportZip_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion

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

        private void cboDestDefaultCred_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cboDestDefaultCred.SelectedIndex == 0)
            {
                this.txtDestDomain.Enabled = false;
                this.txtDestUsername.Enabled = false;
                this.txtDestPassword.Enabled = false;
            }
            else
            {
                this.txtDestDomain.Enabled = true;
                this.txtDestUsername.Enabled = true;
                this.txtDestPassword.Enabled = true;
            }
        }

        private void ConnectInfoForm_Load(object sender, EventArgs e)
        {
            // Setup form default values
            this.cboSrcDefaultCred.SelectedIndex = 0;
            this.cboSrcVersion.SelectedIndex = 0;
            this.cboDestDefaultCred.SelectedIndex = 0;
            this.cboDestVersion.SelectedIndex = 0;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.UI_FieldsCheck();

                if (this.rdoMethodDirect.Checked)
                    this.DirectMigration_Connection();
                else if (this.rdoMethodExportDisk.Checked)
                    this.ExportToDisk_Connection();
                else if (this.rdoMethodExportZip.Checked)
                    this.ExportToZip_Connection();
            }
            catch (Exception er)
            {
                MessageBox.Show(string.Format("Please populate: {0}", er.Message),
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
        }

        #region Connection Methods
        private void DirectMigration_Connection()
        {
            // Save configuration
            this.Save_SourceConfiguration();
            this.Save_DestinationConfiguration();

            ReportServerReader reader = null;
            //TODO ReportServerWriter writer = null;
            string version = "2005-SRC";

            if (this.cboSrcVersion.SelectedIndex == 0)
                version = "2005-SRC";
            else
                version = "2010-SRC";

            reader = this.mKernel.Get<IReportServerReaderFactory>().GetReader<ReportServerReader>(version);

            //TODO Create ReportServerWriter

            //TODO Pass ReportServerWriter to PerformDirectMigrate
            this.PerformDirectMigrate(this.txtSrcPath.Text, this.txtDestPath.Text, reader);
        }

        private void ExportToDisk_Connection()
        {
            // Save configuration
            this.Save_SourceConfiguration();

            ReportServerReader reader = null;
            DataSourceItemExporter dataSourceExporter = null;
            FolderItemExporter folderExporter = null;
            ReportItemExporter reportExporter = null;

            string version = "2005-SRC";

            if (this.cboSrcVersion.SelectedIndex == 0)
                version = "2005-SRC";
            else
                version = "2010-SRC";

            reader = this.mKernel.Get<IReportServerReaderFactory>().GetReader<ReportServerReader>(version);

            //TODO Create ItemExporters
            dataSourceExporter = this.mKernel.Get<DataSourceItemExporter>();
            folderExporter = this.mKernel.Get<FolderItemExporter>();
            reportExporter = this.mKernel.Get<ReportItemExporter>();

           this.PerformExportToDisk(this.txtSrcPath.Text,
                this.txtExportDiskFolderName.Text,
                reader,
                folderExporter,
                reportExporter,
                dataSourceExporter);
        }

        private void ExportToZip_Connection()
        {
            
        }
        #endregion

        #region UI Input Check Methods
        private void UI_FieldsCheck()
        {
            if (this.rdoMethodDirect.Checked)
            {
                this.UI_SourceCheck();
                this.UI_DirectMigration_DestinationCheck();
            }
            else if (this.rdoMethodExportDisk.Checked)
            {
                this.UI_SourceCheck();
                this.UI_ExportDisk_DestinationCheck();
            }
            else if (this.rdoMethodExportZip.Checked)
            {
                //TODO UI_SourceCheck();
                //TODO UI_ExportZip_DestinationCheck();
            }
            else if (this.rdoMethodImportZip.Checked)
            {
                //TODO UI_ImportZip_SourceCheck();
                //TODO UI_ImportZip_DestinationCheck();
            }
        }
        #endregion

        #region Save connection information from UI to App.config
        private void Save_SourceConfiguration()
        {
            if (this.cboSrcDefaultCred.SelectedIndex == 0)
                Properties.Settings.Default.SrcDefaultCred = true;
            else
                Properties.Settings.Default.SrcDefaultCred = false;

            Properties.Settings.Default.SrcDomain = this.txtSrcDomain.Text;
            Properties.Settings.Default.SrcPassword = this.txtSrcPassword.Text;
            Properties.Settings.Default.SrcPath = this.txtSrcPath.Text;
            Properties.Settings.Default.SrcUsername = this.txtSrcUsername.Text;

            if (this.cboSrcVersion.SelectedIndex == 0)
                Properties.Settings.Default.SrcVersion = "2005";
            else
                Properties.Settings.Default.SrcVersion = "2010";

            Properties.Settings.Default.SrcWebServiceUrl = this.txtSrcUrl.Text;

            Properties.Settings.Default.Save();
        }

        private void Save_DestinationConfiguration()
        {
            if (this.cboDestDefaultCred.SelectedIndex == 0)
                Properties.Settings.Default.DestDefaultCred = true;
            else
                Properties.Settings.Default.DestDefaultCred = false;

            Properties.Settings.Default.DestDomain = this.txtDestDomain.Text;
            Properties.Settings.Default.DestPassword = this.txtDestPassword.Text;
            Properties.Settings.Default.DestPath = this.txtDestPath.Text;
            Properties.Settings.Default.DestUsername = this.txtDestUsername.Text;

            if (this.cboDestVersion.SelectedIndex == 0)
                Properties.Settings.Default.DestVersion = "2005";
            else
                Properties.Settings.Default.DestVersion = "2010";

            Properties.Settings.Default.DestWebServiceUrl = this.txtDestUrl.Text;

            Properties.Settings.Default.Save();
        }
        #endregion

        #region Direct Export Group
        private void UI_SourceCheck()
        {
            if (string.IsNullOrEmpty(this.txtSrcUrl.Text))
                throw new Exception("source url");

            if (this.cboSrcDefaultCred.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(this.txtSrcUsername.Text))
                    throw new Exception("source username");

                if (string.IsNullOrEmpty(this.txtSrcPassword.Text))
                    throw new Exception("source password");

                if (string.IsNullOrEmpty(this.txtSrcDomain.Text))
                    throw new Exception("source domain");
            }

            if (string.IsNullOrEmpty(this.txtSrcPath.Text))
                this.txtSrcPath.Text = "/";
        }

        private void UI_DirectMigration_DestinationCheck()
        {
            if (string.IsNullOrEmpty(this.txtDestUrl.Text))
                throw new Exception("destination url");

            if (this.cboDestDefaultCred.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(this.txtDestUsername.Text))
                    throw new Exception("destination username");

                if (string.IsNullOrEmpty(this.txtDestPassword.Text))
                    throw new Exception("destination password");

                if (string.IsNullOrEmpty(this.txtDestDomain.Text))
                    throw new Exception("destination domain");
            }

            if (string.IsNullOrEmpty(this.txtDestPath.Text))
                this.txtDestPath.Text = "/";
        }

        private void PerformDirectMigrate(string sourceRootPath, string destinationRootPath, ReportServerReader reader)
        {
            MigrateForm migrateForm = new MigrateForm(sourceRootPath, destinationRootPath, reader);

            migrateForm.ShowDialog();
        }
        #endregion

        #region Export to disk Group
        private void UI_ExportDisk_DestinationCheck()
        {
            if (string.IsNullOrEmpty(this.txtExportDiskFolderName.Text))
                throw new Exception("folder name");
        }

        private void btnExportDiskFolderBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
            folderDialog.Description = "Select directory to export to...";

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtExportDiskFolderName.Text = folderDialog.SelectedPath;
            }

        }

        private void PerformExportToDisk(string sourceRootPath,
            string destinationPath,
            ReportServerReader reader,
            FolderItemExporter folderExporter,
            ReportItemExporter reportExporter,
            DataSourceItemExporter dataSourceExporter)
        {
            ExportDiskForm exportDiskForm = new ExportDiskForm(sourceRootPath,
                destinationPath,
                reader,
                folderExporter,
                reportExporter,
                dataSourceExporter);

            exportDiskForm.ShowDialog();
        }
        #endregion

        #region Export to Zip Archive Group
        private void btnExportZipFileBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
            saveDialog.FilterIndex = 2;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtExportZipFilename.Text = saveDialog.FileName;
            }
        }
        #endregion
        
    }
}
