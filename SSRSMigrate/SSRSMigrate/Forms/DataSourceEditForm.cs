using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ninject.Extensions.Logging;
using SSRSMigrate.SSRS.Item;

namespace SSRSMigrate.Forms
{
    public partial class DataSourceEditForm : Form
    {
        private readonly ILogger mLogger = null;
        private DataSourceItem mDataSourceItem;

        #region Properties
        public DataSourceItem DataSourceItem
        {
            get { return mDataSourceItem; }
            set
            {
                mDataSourceItem = value;

                LoadDataSource(mDataSourceItem);
            }
        }
        #endregion

        public DataSourceEditForm(ILogger logger)
        {
            mLogger = logger;

            InitializeComponent();

            ShowCredentialsUserInfo(false);
            ShowCredentialsPrompt(false);
        }

        private void DataSourceEditForm_Load(object sender, EventArgs e)
        {

        }

        public void LoadDataSource(DataSourceItem dataSourceItem)
        {
            if (dataSourceItem == null)
            {
                throw new ArgumentNullException("dataSourceItem");
            }

            if (mDataSourceItem != dataSourceItem)
                mDataSourceItem = dataSourceItem;

            ShowCredentialsUserInfo(false);
            ShowCredentialsPrompt(false);

            txtName.Text = dataSourceItem.Name;
            txtPath.Text = dataSourceItem.Path;
            txtExtension.Text = dataSourceItem.Extension;
            txtConnectString.Text = dataSourceItem.ConnectString;

            switch (dataSourceItem.CredentialsRetrieval.ToLower())
            {
                case "none":
                    rdoCredWithoutCreds.Checked = true;

                    ShowCredentialsUserInfo(false);
                    ShowCredentialsPrompt(false);

                    break;
                case "integrated":
                    rdoCredUserViewingReport.Checked = true;

                    ShowCredentialsUserInfo(false);
                    ShowCredentialsPrompt(false);

                    break;
                case "store":
                    rdoCredUseFollowingCreds.Checked = true;
                    txtCredUserName.Text = dataSourceItem.UserName;
                    txtCredPassword.Text = dataSourceItem.Password;

                    if (dataSourceItem.WindowsCredentials)
                        cboCredType.SelectedIndex = 0;
                    else
                        cboCredType.SelectedIndex = 1;

                    if (dataSourceItem.ImpersonateUser)
                    {
                        chkImpersonateUser.Checked = true;
                    }
                    else
                    {
                        chkImpersonateUser.Checked = false;
                    }

                    ShowCredentialsUserInfo(true);
                    ShowCredentialsPrompt(false);

                    break;
                case "prompt":
                    if (dataSourceItem.WindowsCredentials)
                        cboCredType.SelectedIndex = 0;
                    else
                        cboCredType.SelectedIndex = 1;

                    txtCredPrompt.Text = dataSourceItem.Prompt;

                    ShowCredentialsUserInfo(false);
                    ShowCredentialsPrompt(true);

                    break;
            }
        }

        private void rdoCredByPrompt_CheckedChanged(object sender, EventArgs e)
        {
            ShowCredentialsPrompt(rdoCredByPrompt.Checked);
        }

        private void rdoCredUseFollowingCreds_CheckedChanged(object sender, EventArgs e)
        {
            ShowCredentialsUserInfo(rdoCredUseFollowingCreds.Checked);
        }

        private void ShowCredentialsUserInfo(bool enabled)
        {
            lblCredType.Visible = enabled;
            cboCredType.Visible = enabled;

            lblCredUserName.Visible = enabled;
            txtCredUserName.Visible = enabled;

            lblCredPassword.Visible = enabled;
            txtCredPassword.Visible = enabled;

            chkImpersonateUser.Visible = enabled;
        }

        private void ShowCredentialsPrompt(bool enabled)
        {
            lblCredType.Visible = enabled;
            txtCredPrompt.Visible = enabled;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mDataSourceItem = null;

            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveDataSource();

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception er)
            {
                mLogger.Error(er, "Error saving DataSource");

                MessageBox.Show(
                    er.Message, 
                    "Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        private void SaveDataSource()
        {
            if (string.IsNullOrEmpty(txtConnectString.Text))
            {
                throw new Exception("Invalid connection string");
            }

            mDataSourceItem.ConnectString = txtConnectString.Text;

            if (rdoCredUserViewingReport.Checked)
            {
                mDataSourceItem.CredentialsRetrieval = "Integrated";
                mDataSourceItem.Password = null;
                mDataSourceItem.UserName = null;
                mDataSourceItem.WindowsCredentials = false;

                mDataSourceItem.ImpersonateUser = false;
                mDataSourceItem.ImpersonateUserSpecified = true;
            }
            else if (rdoCredUseFollowingCreds.Checked)
            {
                mDataSourceItem.CredentialsRetrieval = "Store";

                if (string.IsNullOrEmpty(txtCredUserName.Text))
                {
                    throw new Exception("Invalid user name");
                }

                mDataSourceItem.Password = txtCredPassword.Text;
                mDataSourceItem.UserName = txtCredUserName.Text;

                if (cboCredType.SelectedIndex == 0)
                {
                    mDataSourceItem.WindowsCredentials = true;
                }
                else
                {
                    mDataSourceItem.WindowsCredentials = false;
                }
                
                mDataSourceItem.ImpersonateUser = chkImpersonateUser.Checked;
                mDataSourceItem.ImpersonateUserSpecified = true;
            }
            else if (rdoCredByPrompt.Checked)
            {
                mDataSourceItem.CredentialsRetrieval = "Prompt";
                mDataSourceItem.Prompt = txtCredPrompt.Text;

                mDataSourceItem.Password = null;
                mDataSourceItem.UserName = null;
                
                if (cboCredType.SelectedIndex == 0)
                {
                    mDataSourceItem.WindowsCredentials = true;
                }
                else
                {
                    mDataSourceItem.WindowsCredentials = false;
                }

                mDataSourceItem.ImpersonateUser = false;
                mDataSourceItem.ImpersonateUserSpecified = true;
            }
            else if (rdoCredWithoutCreds.Checked)
            {
                mDataSourceItem.CredentialsRetrieval = "None";
                mDataSourceItem.Password = null;
                mDataSourceItem.UserName = null;
                mDataSourceItem.WindowsCredentials = false;

                mDataSourceItem.ImpersonateUser = false;
                mDataSourceItem.ImpersonateUserSpecified = true;
            }
        }
    }
}
