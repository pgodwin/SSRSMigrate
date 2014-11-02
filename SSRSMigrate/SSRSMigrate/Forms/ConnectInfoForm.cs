using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Windows.Forms;
using log4net.Config;
using Ninject;
using Ninject.Extensions.Logging.Log4net;
using SSRSMigrate.Bundler;
using SSRSMigrate.Enum;
using SSRSMigrate.Errors;
using SSRSMigrate.Exporter;
using SSRSMigrate.Factory;
using SSRSMigrate.Importer;
using SSRSMigrate.SSRS.Item;
using SSRSMigrate.SSRS.Reader;
using SSRSMigrate.SSRS.Writer;
using Ninject.Extensions.Logging;

namespace SSRSMigrate.Forms
{
    public partial class ConnectInfoForm : Form
    {
        private StandardKernel mKernel = null;
        private ILoggerFactory mLoggerFactory = null;
        private bool mDebug = false;
        private DebugForm mDebugForm = null;
        private IFileSystem mFileSystem = null;

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

            this.mLoggerFactory = mKernel.Get<ILoggerFactory>();
            this.mFileSystem = mKernel.Get<IFileSystem>();

            InitializeComponent();

            this.LoadSettings();

            // Create the DebugForm and hide it if debug is False
            this.mDebugForm = new DebugForm();
            this.mDebugForm.Show();
            if (!this.mDebug)
                this.mDebugForm.Hide();
        }

        private void LoadSettings()
        {
            this.mDebug = Properties.Settings.Default.Debug;
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
                this.grpSrcServer.Visible = true;
            }
            else
            {
                this.grpDestServer.Visible = false;
                this.grpSrcServer.Visible = false;
            }
        }

        private void rdoMethodExportDisk_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoMethodExportDisk.Checked)
            {
                this.grpExportDisk.Visible = true;
                this.grpSrcServer.Visible = true;
            }
            else
            {
                this.grpExportDisk.Visible = false;
                this.grpSrcServer.Visible = false;
            }
        }

        private void rdoMethodExportZip_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoMethodExportZip.Checked)
            {
                this.grpExportZip.Visible = true;
                this.grpSrcServer.Visible = true;
            }
            else
            {
                this.grpExportZip.Visible = false;
                this.grpSrcServer.Visible = false;
            }
        }

        private void rdoMethodImportZip_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rdoMethodImportZip.Checked)
            {
                this.grpImportZip.Visible = true;
                this.grpDestServer.Visible = true;
            }
            else
            {
                this.grpImportZip.Visible = false;
                this.grpDestServer.Visible = false;
            }
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

            // Start with Server-to-Server migration checked
            this.rdoMethodDirect.Checked = true;
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
                else if (this.rdoMethodImportZip.Checked)
                    this.ImportFromZip_Connection();
            }
            catch (UserInterfaceInvalidFieldException er)
            {
                MessageBox.Show(er.Message,
                    "Invalid Field",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message,
                    "Error",
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

            IReportServerReader reader = null;
            IReportServerWriter writer = null;
            string srcVersion = "2005-SRC";
            string destVersion = "2005-SRC";

            if (this.cboSrcVersion.SelectedIndex > this.cboDestVersion.SelectedIndex)
                throw new Exception("Source server is newer than destination server.");

            if (this.cboSrcVersion.SelectedIndex == 0)
                srcVersion = "2005-SRC";
            else
                srcVersion = "2010-SRC";

            if (this.cboDestVersion.SelectedIndex == 0)
                destVersion = "2005-DEST";
            else
                destVersion = "2010-DEST";

            reader = this.mKernel.Get<IReportServerReaderFactory>().GetReader<ReportServerReader>(srcVersion);
            writer = this.mKernel.Get<IReportServerWriterFactory>().GetWriter<ReportServerWriter>(destVersion);

            // Check source and destination server versions through the reader and writer
            SSRSVersion sourceVersion = reader.GetSqlServerVersion();
            SSRSVersion destinationVersion = writer.GetSqlServerVersion();

            // If the destination version is older than the source version, prevent migration.
            if ((int)destinationVersion < (int)sourceVersion)
                throw new Exception("Destination server is using an older version of SQL Server than the source server.");
        
            writer.Overwrite = this.cbkDestOverwrite.Checked; //TODO Should include this in the IoC container somehow

            this.PerformDirectMigrate(
                this.txtSrcUrl.Text,
                this.txtSrcPath.Text,
                this.txtDestUrl.Text,
                this.txtDestPath.Text, 
                reader, 
                writer);
        }

        private void ExportToDisk_Connection()
        {
            // Save configuration
            this.Save_SourceConfiguration();

            ReportServerReader reader = null;
            DataSourceItemExporter dataSourceExporter = null;
            FolderItemExporter folderExporter = null;
            ReportItemExporter reportExporter = null;

            string version = this.GetSourceServerVersion();

            reader = this.mKernel.Get<IReportServerReaderFactory>().GetReader<ReportServerReader>(version);

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
            // Save configuration
            this.Save_SourceConfiguration();

            ReportServerReader reader = null;
            DataSourceItemExporter dataSourceExporter = null;
            FolderItemExporter folderExporter = null;
            ReportItemExporter reportExporter = null;
            IBundler zipBundler = null;
            IFileSystem fileSystem = null;

            string version = this.GetSourceServerVersion();

            reader = this.mKernel.Get<IReportServerReaderFactory>().GetReader<ReportServerReader>(version);

            dataSourceExporter = this.mKernel.Get<DataSourceItemExporter>();
            folderExporter = this.mKernel.Get<FolderItemExporter>();
            reportExporter = this.mKernel.Get<ReportItemExporter>();

            zipBundler = this.mKernel.Get<IBundler>();
            fileSystem = this.mKernel.Get<IFileSystem>();

            this.PerformExportToZip(this.txtSrcPath.Text,
                this.txtExportZipFilename.Text,
                reader,
                folderExporter,
                reportExporter,
                dataSourceExporter,
                zipBundler,
                fileSystem);
        }

        private void ImportFromZip_Connection()
        {
            // Save configuration
            this.Save_DestinationConfiguration();
            this.Save_ImportZipConfiguration();

            IReportServerWriter writer = null;
            IBundleReader zipBundleReader = null;
            IItemImporter<DataSourceItem> dataSourceItemImporter = null;
            IItemImporter<FolderItem> folderItemImporter = null;
            IItemImporter<ReportItem> reportItemImporter = null;
            IFileSystem fileSystem = null;

            string version = this.GetDestinationServerVersion();

            writer = this.mKernel.Get<IReportServerWriterFactory>().GetWriter<ReportServerWriter>(version);
            zipBundleReader = this.mKernel.Get<IBundleReader>();

            // Set properties for IBundleReader
            zipBundleReader.UnPackDirectory = this.GetTempExtractPath();
            zipBundleReader.ArchiveFileName = this.txtImportZipFilename.Text;

            dataSourceItemImporter = this.mKernel.Get<DataSourceItemImporter>();
            folderItemImporter = this.mKernel.Get<FolderItemImporter>();
            reportItemImporter = this.mKernel.Get<ReportItemImporter>();
            fileSystem = this.mKernel.Get<IFileSystem>();

            this.PerformImportFromZip(
                this.txtImportZipFilename.Text,
                this.txtDestPath.Text,
                this.txtDestUrl.Text,
                writer,
                zipBundleReader,
                dataSourceItemImporter,
                folderItemImporter,
                reportItemImporter,
                fileSystem
                );
        }
        #endregion

        #region UI Input Check Methods
        private void UI_FieldsCheck()
        {
            if (this.rdoMethodDirect.Checked)
            {
                this.UI_SourceCheck();
                this.UI_Migration_DestinationCheck();
            }
            else if (this.rdoMethodExportDisk.Checked)
            {
                this.UI_SourceCheck();
                this.UI_ExportDisk_DestinationCheck();
            }
            else if (this.rdoMethodExportZip.Checked)
            {
                this.UI_SourceCheck();
                this.UI_ExportZip_DestinationCheck();
            }
            else if (this.rdoMethodImportZip.Checked)
            {
                this.UI_ImportZip_SourceCheck();
                this.UI_Migration_DestinationCheck();
            }
        }

        private void UI_SourceCheck()
        {
            if (string.IsNullOrEmpty(this.txtSrcUrl.Text))
                throw new UserInterfaceInvalidFieldException("source url");

            if (this.cboSrcDefaultCred.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(this.txtSrcUsername.Text))
                    throw new UserInterfaceInvalidFieldException("source username");

                if (string.IsNullOrEmpty(this.txtSrcPassword.Text))
                    throw new UserInterfaceInvalidFieldException("source password");

                if (string.IsNullOrEmpty(this.txtSrcDomain.Text))
                    throw new UserInterfaceInvalidFieldException("source domain");
            }

            if (string.IsNullOrEmpty(this.txtSrcPath.Text))
                this.txtSrcPath.Text = "/";
        }

        private void UI_Migration_DestinationCheck()
        {
            if (string.IsNullOrEmpty(this.txtDestUrl.Text))
                throw new UserInterfaceInvalidFieldException("destination url");

            if (this.cboDestDefaultCred.SelectedIndex == 1)
            {
                if (string.IsNullOrEmpty(this.txtDestUsername.Text))
                    throw new UserInterfaceInvalidFieldException("destination username");

                if (string.IsNullOrEmpty(this.txtDestPassword.Text))
                    throw new UserInterfaceInvalidFieldException("destination password");

                if (string.IsNullOrEmpty(this.txtDestDomain.Text))
                    throw new UserInterfaceInvalidFieldException("destination domain");
            }

            if (string.IsNullOrEmpty(this.txtDestPath.Text))
                this.txtDestPath.Text = "/";
        }

        private void UI_ExportDisk_DestinationCheck()
        {
            if (string.IsNullOrEmpty(this.txtExportDiskFolderName.Text))
                throw new UserInterfaceInvalidFieldException("folder name");
        }

        private void UI_ExportZip_DestinationCheck()
        {
            if (string.IsNullOrEmpty(this.txtExportZipFilename.Text))
                throw new UserInterfaceInvalidFieldException("filename");
        }

        private void UI_ImportZip_SourceCheck()
        {
            if (string.IsNullOrEmpty(this.txtImportZipFilename.Text))
                throw new UserInterfaceInvalidFieldException("filename");

            if (!this.mFileSystem.File.Exists(this.txtImportZipFilename.Text))
                throw new FileNotFoundException(this.txtImportZipFilename.Text);
        }

        private string GetSourceServerVersion()
        {
            string version = "2005-SRC";

            if (this.cboSrcVersion.SelectedIndex == 0)
                version = "2005-SRC";
            else
                version = "2010-SRC";

            return version;
        }

        private string GetDestinationServerVersion()
        {
            string version = "2005-DEST";

            if (this.cboDestVersion.SelectedIndex == 0)
                version = "2005-DEST";
            else
                version = "2010-DEST";

            return version;
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

        private void Save_ImportZipConfiguration()
        {
            Properties.Settings.Default.ImportZipFileName = this.txtImportZipFilename.Text;
            Properties.Settings.Default.ImportZipUnpackDir = this.mFileSystem.Path.Combine(this.mFileSystem.Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

            Properties.Settings.Default.Save();
        }
        #endregion

        #region Direct Export Group
        private void PerformDirectMigrate(
            string sourceServerUrl,
            string sourceRootPath, 
            string destinationServerUrl,
            string destinationRootPath, 
            IReportServerReader reader,
            IReportServerWriter writer)
        {
            MigrateForm migrateForm = new MigrateForm(
                sourceRootPath,
                sourceServerUrl,
                destinationRootPath,
                destinationServerUrl,
                reader, 
                writer, 
                this.mLoggerFactory);

            migrateForm.DebugForm = this.mDebugForm;
            migrateForm.ShowDialog();
        }
        #endregion

        #region Export to disk Group
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
                dataSourceExporter,
                this.mLoggerFactory);

            exportDiskForm.DebugForm = this.mDebugForm;
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

        private void PerformExportToZip(string sourceRootPath,
            string destinationFilename,
            ReportServerReader reader,
            FolderItemExporter folderExporter,
            ReportItemExporter reportExporter,
            DataSourceItemExporter dataSourceExporter,
            IBundler zipBunder,
            IFileSystem fileSystem)
        {
            ExportZipForm exportZipForm = new ExportZipForm(
                sourceRootPath,
                destinationFilename,
                reader,
                folderExporter,
                reportExporter,
                dataSourceExporter,
                zipBunder,
                this.mLoggerFactory,
                fileSystem);

            exportZipForm.DebugForm = this.mDebugForm;
            exportZipForm.ShowDialog();
        }
        #endregion

        #region Import from Zip Archive Group
        private void btnImportZipBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
            openDialog.FilterIndex = 2;
            openDialog.RestoreDirectory = true;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtImportZipFilename.Text = openDialog.FileName;
            }
        }

        private void PerformImportFromZip(
            string sourceFilename,
            string destinationRootPath,
            string destinationServerUrl,
            IReportServerWriter writer,
            IBundleReader zipBundleReader,
            IItemImporter<DataSourceItem> dataSourceItemImporter,
            IItemImporter<FolderItem> folderItemImporter,
            IItemImporter<ReportItem> reportItemImporter,
            IFileSystem fileSystem)
        {
            ImportZipForm importzipForm = new ImportZipForm(
                sourceFilename,
                destinationRootPath,
                destinationServerUrl,
                zipBundleReader,
                writer,
                this.mLoggerFactory,
                dataSourceItemImporter,
                folderItemImporter,
                reportItemImporter,
                fileSystem);

            importzipForm.DebugForm = this.mDebugForm;
            importzipForm.ShowDialog();
        }

        private string GetTempExtractPath()
        {
            string path = this.mFileSystem.Path.Combine(this.mFileSystem.Path.GetTempPath(),
                Guid.NewGuid().ToString("N"));

            return path;
        }
        #endregion
    }
}
