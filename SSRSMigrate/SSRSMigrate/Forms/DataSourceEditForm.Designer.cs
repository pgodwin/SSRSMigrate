namespace SSRSMigrate.Forms
{
    partial class DataSourceEditForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblConnectString = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.txtConnectString = new System.Windows.Forms.TextBox();
            this.grpCredentials = new System.Windows.Forms.GroupBox();
            this.lblCredPassword = new System.Windows.Forms.Label();
            this.lblCredUserName = new System.Windows.Forms.Label();
            this.lblCredType = new System.Windows.Forms.Label();
            this.rdoCredWithoutCreds = new System.Windows.Forms.RadioButton();
            this.rdoCredByPrompt = new System.Windows.Forms.RadioButton();
            this.rdoCredUseFollowingCreds = new System.Windows.Forms.RadioButton();
            this.rdoCredUserViewingReport = new System.Windows.Forms.RadioButton();
            this.lblExtension = new System.Windows.Forms.Label();
            this.txtExtension = new System.Windows.Forms.TextBox();
            this.txtCredUserName = new System.Windows.Forms.TextBox();
            this.txtCredPassword = new System.Windows.Forms.TextBox();
            this.cboCredType = new System.Windows.Forms.ComboBox();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.txtCredPrompt = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkImpersonateUser = new System.Windows.Forms.CheckBox();
            this.grpCredentials.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblConnectString
            // 
            this.lblConnectString.AutoSize = true;
            this.lblConnectString.Location = new System.Drawing.Point(9, 101);
            this.lblConnectString.Name = "lblConnectString";
            this.lblConnectString.Size = new System.Drawing.Size(94, 13);
            this.lblConnectString.TabIndex = 0;
            this.lblConnectString.Text = "Connection String:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(30, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(74, 13);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(295, 20);
            this.txtName.TabIndex = 3;
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(74, 45);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(295, 20);
            this.txtPath.TabIndex = 4;
            // 
            // txtConnectString
            // 
            this.txtConnectString.Location = new System.Drawing.Point(12, 117);
            this.txtConnectString.Multiline = true;
            this.txtConnectString.Name = "txtConnectString";
            this.txtConnectString.Size = new System.Drawing.Size(538, 45);
            this.txtConnectString.TabIndex = 5;
            // 
            // grpCredentials
            // 
            this.grpCredentials.Controls.Add(this.chkImpersonateUser);
            this.grpCredentials.Controls.Add(this.txtCredPrompt);
            this.grpCredentials.Controls.Add(this.lblPrompt);
            this.grpCredentials.Controls.Add(this.cboCredType);
            this.grpCredentials.Controls.Add(this.txtCredPassword);
            this.grpCredentials.Controls.Add(this.txtCredUserName);
            this.grpCredentials.Controls.Add(this.lblCredPassword);
            this.grpCredentials.Controls.Add(this.lblCredUserName);
            this.grpCredentials.Controls.Add(this.lblCredType);
            this.grpCredentials.Controls.Add(this.rdoCredWithoutCreds);
            this.grpCredentials.Controls.Add(this.rdoCredByPrompt);
            this.grpCredentials.Controls.Add(this.rdoCredUseFollowingCreds);
            this.grpCredentials.Controls.Add(this.rdoCredUserViewingReport);
            this.grpCredentials.Location = new System.Drawing.Point(15, 197);
            this.grpCredentials.Name = "grpCredentials";
            this.grpCredentials.Size = new System.Drawing.Size(535, 232);
            this.grpCredentials.TabIndex = 7;
            this.grpCredentials.TabStop = false;
            this.grpCredentials.Text = "Credentials";
            // 
            // lblCredPassword
            // 
            this.lblCredPassword.AutoSize = true;
            this.lblCredPassword.Location = new System.Drawing.Point(59, 174);
            this.lblCredPassword.Name = "lblCredPassword";
            this.lblCredPassword.Size = new System.Drawing.Size(56, 13);
            this.lblCredPassword.TabIndex = 6;
            this.lblCredPassword.Text = "Password:";
            // 
            // lblCredUserName
            // 
            this.lblCredUserName.AutoSize = true;
            this.lblCredUserName.Location = new System.Drawing.Point(54, 148);
            this.lblCredUserName.Name = "lblCredUserName";
            this.lblCredUserName.Size = new System.Drawing.Size(61, 13);
            this.lblCredUserName.TabIndex = 5;
            this.lblCredUserName.Text = "User name:";
            // 
            // lblCredType
            // 
            this.lblCredType.AutoSize = true;
            this.lblCredType.Location = new System.Drawing.Point(15, 122);
            this.lblCredType.Name = "lblCredType";
            this.lblCredType.Size = new System.Drawing.Size(100, 13);
            this.lblCredType.TabIndex = 4;
            this.lblCredType.Text = "Type of credentials:";
            // 
            // rdoCredWithoutCreds
            // 
            this.rdoCredWithoutCreds.AutoSize = true;
            this.rdoCredWithoutCreds.Location = new System.Drawing.Point(18, 92);
            this.rdoCredWithoutCreds.Name = "rdoCredWithoutCreds";
            this.rdoCredWithoutCreds.Size = new System.Drawing.Size(136, 17);
            this.rdoCredWithoutCreds.TabIndex = 3;
            this.rdoCredWithoutCreds.TabStop = true;
            this.rdoCredWithoutCreds.Text = "Without any credentials";
            this.rdoCredWithoutCreds.UseVisualStyleBackColor = true;
            // 
            // rdoCredByPrompt
            // 
            this.rdoCredByPrompt.AutoSize = true;
            this.rdoCredByPrompt.Location = new System.Drawing.Point(18, 69);
            this.rdoCredByPrompt.Name = "rdoCredByPrompt";
            this.rdoCredByPrompt.Size = new System.Drawing.Size(283, 17);
            this.rdoCredByPrompt.TabIndex = 2;
            this.rdoCredByPrompt.TabStop = true;
            this.rdoCredByPrompt.Text = "By prompting the user viewing the report for credentials";
            this.rdoCredByPrompt.UseVisualStyleBackColor = true;
            this.rdoCredByPrompt.CheckedChanged += new System.EventHandler(this.rdoCredByPrompt_CheckedChanged);
            // 
            // rdoCredUseFollowingCreds
            // 
            this.rdoCredUseFollowingCreds.AutoSize = true;
            this.rdoCredUseFollowingCreds.Location = new System.Drawing.Point(18, 44);
            this.rdoCredUseFollowingCreds.Name = "rdoCredUseFollowingCreds";
            this.rdoCredUseFollowingCreds.Size = new System.Drawing.Size(168, 17);
            this.rdoCredUseFollowingCreds.TabIndex = 1;
            this.rdoCredUseFollowingCreds.TabStop = true;
            this.rdoCredUseFollowingCreds.Text = "Using the following credentials";
            this.rdoCredUseFollowingCreds.UseVisualStyleBackColor = true;
            this.rdoCredUseFollowingCreds.CheckedChanged += new System.EventHandler(this.rdoCredUseFollowingCreds_CheckedChanged);
            // 
            // rdoCredUserViewingReport
            // 
            this.rdoCredUserViewingReport.AutoSize = true;
            this.rdoCredUserViewingReport.Location = new System.Drawing.Point(18, 21);
            this.rdoCredUserViewingReport.Name = "rdoCredUserViewingReport";
            this.rdoCredUserViewingReport.Size = new System.Drawing.Size(165, 17);
            this.rdoCredUserViewingReport.TabIndex = 0;
            this.rdoCredUserViewingReport.TabStop = true;
            this.rdoCredUserViewingReport.Text = "As the user viewing the report";
            this.rdoCredUserViewingReport.UseVisualStyleBackColor = true;
            // 
            // lblExtension
            // 
            this.lblExtension.AutoSize = true;
            this.lblExtension.Location = new System.Drawing.Point(12, 76);
            this.lblExtension.Name = "lblExtension";
            this.lblExtension.Size = new System.Drawing.Size(56, 13);
            this.lblExtension.TabIndex = 8;
            this.lblExtension.Text = "Extension:";
            // 
            // txtExtension
            // 
            this.txtExtension.Location = new System.Drawing.Point(74, 73);
            this.txtExtension.Name = "txtExtension";
            this.txtExtension.ReadOnly = true;
            this.txtExtension.Size = new System.Drawing.Size(116, 20);
            this.txtExtension.TabIndex = 9;
            // 
            // txtCredUserName
            // 
            this.txtCredUserName.Location = new System.Drawing.Point(121, 145);
            this.txtCredUserName.Name = "txtCredUserName";
            this.txtCredUserName.Size = new System.Drawing.Size(116, 20);
            this.txtCredUserName.TabIndex = 8;
            // 
            // txtCredPassword
            // 
            this.txtCredPassword.Location = new System.Drawing.Point(121, 171);
            this.txtCredPassword.Name = "txtCredPassword";
            this.txtCredPassword.Size = new System.Drawing.Size(116, 20);
            this.txtCredPassword.TabIndex = 9;
            // 
            // cboCredType
            // 
            this.cboCredType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCredType.FormattingEnabled = true;
            this.cboCredType.Items.AddRange(new object[] {
            "Windows user name and password",
            "Database user name and password"});
            this.cboCredType.Location = new System.Drawing.Point(121, 118);
            this.cboCredType.Name = "cboCredType";
            this.cboCredType.Size = new System.Drawing.Size(233, 21);
            this.cboCredType.TabIndex = 10;
            // 
            // lblPrompt
            // 
            this.lblPrompt.AutoSize = true;
            this.lblPrompt.Location = new System.Drawing.Point(75, 148);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(40, 13);
            this.lblPrompt.TabIndex = 11;
            this.lblPrompt.Text = "Prompt";
            this.lblPrompt.Visible = false;
            // 
            // txtCredPrompt
            // 
            this.txtCredPrompt.Location = new System.Drawing.Point(121, 145);
            this.txtCredPrompt.Name = "txtCredPrompt";
            this.txtCredPrompt.Size = new System.Drawing.Size(392, 20);
            this.txtCredPrompt.TabIndex = 12;
            this.txtCredPrompt.Visible = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(477, 435);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 26);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(394, 435);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(77, 26);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkImpersonateUser
            // 
            this.chkImpersonateUser.AutoSize = true;
            this.chkImpersonateUser.Location = new System.Drawing.Point(62, 197);
            this.chkImpersonateUser.Name = "chkImpersonateUser";
            this.chkImpersonateUser.Size = new System.Drawing.Size(425, 17);
            this.chkImpersonateUser.TabIndex = 13;
            this.chkImpersonateUser.Text = "Log in using these credentials, but then try to impersonate the user viewing the " +
    "report";
            this.chkImpersonateUser.UseVisualStyleBackColor = true;
            // 
            // DataSourceEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 494);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtExtension);
            this.Controls.Add(this.lblExtension);
            this.Controls.Add(this.grpCredentials);
            this.Controls.Add(this.txtConnectString);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblConnectString);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataSourceEditForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Source Edit";
            this.Load += new System.EventHandler(this.DataSourceEditForm_Load);
            this.grpCredentials.ResumeLayout(false);
            this.grpCredentials.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConnectString;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtConnectString;
        private System.Windows.Forms.GroupBox grpCredentials;
        private System.Windows.Forms.Label lblCredPassword;
        private System.Windows.Forms.Label lblCredUserName;
        private System.Windows.Forms.Label lblCredType;
        private System.Windows.Forms.RadioButton rdoCredWithoutCreds;
        private System.Windows.Forms.RadioButton rdoCredByPrompt;
        private System.Windows.Forms.RadioButton rdoCredUseFollowingCreds;
        private System.Windows.Forms.RadioButton rdoCredUserViewingReport;
        private System.Windows.Forms.Label lblExtension;
        private System.Windows.Forms.TextBox txtExtension;
        private System.Windows.Forms.TextBox txtCredPassword;
        private System.Windows.Forms.TextBox txtCredUserName;
        private System.Windows.Forms.ComboBox cboCredType;
        private System.Windows.Forms.TextBox txtCredPrompt;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkImpersonateUser;
    }
}