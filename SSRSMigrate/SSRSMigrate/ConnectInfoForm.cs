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

namespace SSRSMigrate
{
    public partial class ConnectInfoForm : Form
    {
        private StandardKernel mKernel = null;

        public ConnectInfoForm()
        {
            InitializeComponent();
        }

        private void rdoMethodDirect_MouseHover(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "Perform a direct migration of items on one SSRS server to another SSRS server.";
        }

        private void rdoMethodDirect_MouseLeave(object sender, EventArgs e)
        {
            this.lblMethodInfo.Text = "";
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

                bool srcDefaultCred = true;
                bool srcReportingService2005 = false;

                if (cboSrcDefaultCred.SelectedIndex == 0)
                    srcDefaultCred = true;
                else
                    srcDefaultCred = false;

                if (cboSrcVersion.SelectedIndex == 0)
                {
                    srcReportingService2005 = true;
                }
                else
                {
                    srcReportingService2005 = false;
                }

                this.mKernel = new StandardKernel(new DependencyModule(
                   srcReportingService2005,
                   txtSrcPath.Text,
                   txtSrcUrl.Text,
                   srcDefaultCred,
                   txtSrcUsername.Text,
                   txtSrcPassword.Text,
                   txtSrcDomain.Text));

                ReportServerReader reader = this.mKernel.Get<ReportServerReader>();

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

        private void PerformDirectMigrate(string sourceRootPath, string destinationRootPath, ReportServerReader reader)
        {
            MigrateForm migrateForm = new MigrateForm(sourceRootPath, destinationRootPath, reader);

            migrateForm.ShowDialog();
        }
    }
}
