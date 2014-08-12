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

namespace SSRSMigrate
{
    public partial class ConnectInfoForm : Form
    {
        private StandardKernel mKernel = null;

        public ConnectInfoForm()
        {
            this.mKernel = new StandardKernel(new ReportServerRepositoryModule());

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
            if (rdoMethodDirect.Checked)
            {
                grpDestServer.Visible = true;
            }
            else
            {
                grpDestServer.Visible = false;
            }
        }

        private void rdoMethodExportZip_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoMethodExportZip.Checked)
            {
                grpExportZip.Visible = true;
            }
            else
            {
                grpExportZip.Visible = false;
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
                this.UICheck();
                this.SaveUIToConfig();

                ReportServerReader reader = null;
                string version = "2005-SRC";

                if (cboSrcVersion.SelectedIndex == 0) 
                    version = "2005-SRC";
                else
                    version = "2010-SRC";

                reader = new ReportServerReader(this.mKernel.Get<IReportServerRepositoryFactory>().GetRepository(version));

                this.PerformDirectMigrate(txtSrcPath.Text, txtDestPath.Text, reader);
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

        private void UICheck()
        {
            if (rdoMethodDirect.Checked)
            {
                UISourceCheck();
                UIDestinationCheck();
            }
        }

        private void UISourceCheck()
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

        private void UIDestinationCheck()
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

        private void SaveUIToConfig()
        {
            if (cboSrcDefaultCred.SelectedIndex == 0)
                Properties.Settings.Default.SrcDefaultCred = true;
            else
                Properties.Settings.Default.SrcDefaultCred = false;

            Properties.Settings.Default.SrcDomain = txtSrcDomain.Text;
            Properties.Settings.Default.SrcPassword = txtSrcPassword.Text;
            Properties.Settings.Default.SrcPath = txtSrcPath.Text;
            Properties.Settings.Default.SrcUsername = txtSrcUsername.Text;

            if (cboSrcVersion.SelectedIndex == 0)
                Properties.Settings.Default.SrcVersion = "2005";
            else
                Properties.Settings.Default.SrcVersion = "2010";

            Properties.Settings.Default.SrcWebServiceUrl = txtSrcUrl.Text;

            if (cboDestDefaultCred.SelectedIndex == 0)
                Properties.Settings.Default.DestDefaultCred = true;
            else
                Properties.Settings.Default.DestDefaultCred = false;

            Properties.Settings.Default.DestDomain = txtDestDomain.Text;
            Properties.Settings.Default.DestPassword = txtDestPassword.Text;
            Properties.Settings.Default.DestPath = txtDestPath.Text;
            Properties.Settings.Default.DestUsername = txtDestUsername.Text;

            if (cboDestVersion.SelectedIndex == 0)
                Properties.Settings.Default.DestVersion = "2005";
            else
                Properties.Settings.Default.DestVersion = "2010";

            Properties.Settings.Default.DestWebServiceUrl = txtDestUrl.Text;

            Properties.Settings.Default.Save();
        }

        private void PerformDirectMigrate(string sourceRootPath, string destinationRootPath, ReportServerReader reader)
        {
            MigrateForm migrateForm = new MigrateForm(sourceRootPath, destinationRootPath, reader);

            migrateForm.ShowDialog();
        }
    }
}
