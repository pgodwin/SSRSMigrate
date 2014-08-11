namespace SSRSMigrate
{
    partial class MainForm
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnSrcRefreshReports = new System.Windows.Forms.Button();
            this.lstSrcReports = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboSrcVersion = new System.Windows.Forms.ComboBox();
            this.txtSrcPath = new System.Windows.Forms.TextBox();
            this.txtSrcDomain = new System.Windows.Forms.TextBox();
            this.txtSrcPassword = new System.Windows.Forms.TextBox();
            this.txtSrcUsername = new System.Windows.Forms.TextBox();
            this.cboSrcDefaultCred = new System.Windows.Forms.ComboBox();
            this.txtSrcUrl = new System.Windows.Forms.TextBox();
            this.lblSrcPath = new System.Windows.Forms.Label();
            this.lblSrcDomain = new System.Windows.Forms.Label();
            this.lblSrcPassword = new System.Windows.Forms.Label();
            this.lblSrcUsername = new System.Windows.Forms.Label();
            this.lblSrcDefaultCred = new System.Windows.Forms.Label();
            this.lblSrcVersion = new System.Windows.Forms.Label();
            this.lblSrcUrl = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusStrip.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 562);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(917, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(800, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.btnSrcRefreshReports);
            this.grpSource.Controls.Add(this.lstSrcReports);
            this.grpSource.Controls.Add(this.cboSrcVersion);
            this.grpSource.Controls.Add(this.txtSrcPath);
            this.grpSource.Controls.Add(this.txtSrcDomain);
            this.grpSource.Controls.Add(this.txtSrcPassword);
            this.grpSource.Controls.Add(this.txtSrcUsername);
            this.grpSource.Controls.Add(this.cboSrcDefaultCred);
            this.grpSource.Controls.Add(this.txtSrcUrl);
            this.grpSource.Controls.Add(this.lblSrcPath);
            this.grpSource.Controls.Add(this.lblSrcDomain);
            this.grpSource.Controls.Add(this.lblSrcPassword);
            this.grpSource.Controls.Add(this.lblSrcUsername);
            this.grpSource.Controls.Add(this.lblSrcDefaultCred);
            this.grpSource.Controls.Add(this.lblSrcVersion);
            this.grpSource.Controls.Add(this.lblSrcUrl);
            this.grpSource.Location = new System.Drawing.Point(12, 12);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(441, 513);
            this.grpSource.TabIndex = 1;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Server";
            // 
            // btnSrcRefreshReports
            // 
            this.btnSrcRefreshReports.Location = new System.Drawing.Point(284, 478);
            this.btnSrcRefreshReports.Name = "btnSrcRefreshReports";
            this.btnSrcRefreshReports.Size = new System.Drawing.Size(138, 23);
            this.btnSrcRefreshReports.TabIndex = 15;
            this.btnSrcRefreshReports.Text = "Refresh Reports List";
            this.btnSrcRefreshReports.UseVisualStyleBackColor = true;
            this.btnSrcRefreshReports.Click += new System.EventHandler(this.btnSrcRefreshReports_Click);
            // 
            // lstSrcReports
            // 
            this.lstSrcReports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSrcName,
            this.colSrcPath});
            this.lstSrcReports.FullRowSelect = true;
            this.lstSrcReports.GridLines = true;
            this.lstSrcReports.Location = new System.Drawing.Point(21, 193);
            this.lstSrcReports.MultiSelect = false;
            this.lstSrcReports.Name = "lstSrcReports";
            this.lstSrcReports.Size = new System.Drawing.Size(401, 279);
            this.lstSrcReports.TabIndex = 14;
            this.lstSrcReports.UseCompatibleStateImageBehavior = false;
            this.lstSrcReports.View = System.Windows.Forms.View.Details;
            // 
            // colSrcName
            // 
            this.colSrcName.Text = "Name";
            this.colSrcName.Width = 161;
            // 
            // colSrcPath
            // 
            this.colSrcPath.Text = "Path";
            this.colSrcPath.Width = 234;
            // 
            // cboSrcVersion
            // 
            this.cboSrcVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSrcVersion.FormattingEnabled = true;
            this.cboSrcVersion.Items.AddRange(new object[] {
            "SQL Server 2008",
            "SQL Server 2008 R2"});
            this.cboSrcVersion.Location = new System.Drawing.Point(310, 51);
            this.cboSrcVersion.Name = "cboSrcVersion";
            this.cboSrcVersion.Size = new System.Drawing.Size(112, 21);
            this.cboSrcVersion.TabIndex = 13;
            // 
            // txtSrcPath
            // 
            this.txtSrcPath.Location = new System.Drawing.Point(123, 157);
            this.txtSrcPath.Name = "txtSrcPath";
            this.txtSrcPath.Size = new System.Drawing.Size(121, 20);
            this.txtSrcPath.TabIndex = 12;
            this.txtSrcPath.Text = "/";
            // 
            // txtSrcDomain
            // 
            this.txtSrcDomain.Location = new System.Drawing.Point(123, 131);
            this.txtSrcDomain.Name = "txtSrcDomain";
            this.txtSrcDomain.Size = new System.Drawing.Size(121, 20);
            this.txtSrcDomain.TabIndex = 11;
            // 
            // txtSrcPassword
            // 
            this.txtSrcPassword.Location = new System.Drawing.Point(123, 105);
            this.txtSrcPassword.Name = "txtSrcPassword";
            this.txtSrcPassword.Size = new System.Drawing.Size(121, 20);
            this.txtSrcPassword.TabIndex = 10;
            // 
            // txtSrcUsername
            // 
            this.txtSrcUsername.Location = new System.Drawing.Point(123, 78);
            this.txtSrcUsername.Name = "txtSrcUsername";
            this.txtSrcUsername.Size = new System.Drawing.Size(121, 20);
            this.txtSrcUsername.TabIndex = 9;
            // 
            // cboSrcDefaultCred
            // 
            this.cboSrcDefaultCred.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSrcDefaultCred.FormattingEnabled = true;
            this.cboSrcDefaultCred.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cboSrcDefaultCred.Location = new System.Drawing.Point(123, 51);
            this.cboSrcDefaultCred.Name = "cboSrcDefaultCred";
            this.cboSrcDefaultCred.Size = new System.Drawing.Size(121, 21);
            this.cboSrcDefaultCred.TabIndex = 8;
            this.cboSrcDefaultCred.SelectedIndexChanged += new System.EventHandler(this.cboSrcDefaultCred_SelectedIndexChanged);
            // 
            // txtSrcUrl
            // 
            this.txtSrcUrl.Location = new System.Drawing.Point(123, 25);
            this.txtSrcUrl.Name = "txtSrcUrl";
            this.txtSrcUrl.Size = new System.Drawing.Size(299, 20);
            this.txtSrcUrl.TabIndex = 7;
            this.txtSrcUrl.Text = "http://localhost/ReportServer";
            // 
            // lblSrcPath
            // 
            this.lblSrcPath.AutoSize = true;
            this.lblSrcPath.Location = new System.Drawing.Point(83, 160);
            this.lblSrcPath.Name = "lblSrcPath";
            this.lblSrcPath.Size = new System.Drawing.Size(32, 13);
            this.lblSrcPath.TabIndex = 6;
            this.lblSrcPath.Text = "Path:";
            // 
            // lblSrcDomain
            // 
            this.lblSrcDomain.AutoSize = true;
            this.lblSrcDomain.Location = new System.Drawing.Point(69, 134);
            this.lblSrcDomain.Name = "lblSrcDomain";
            this.lblSrcDomain.Size = new System.Drawing.Size(46, 13);
            this.lblSrcDomain.TabIndex = 5;
            this.lblSrcDomain.Text = "Domain:";
            // 
            // lblSrcPassword
            // 
            this.lblSrcPassword.AutoSize = true;
            this.lblSrcPassword.Location = new System.Drawing.Point(59, 105);
            this.lblSrcPassword.Name = "lblSrcPassword";
            this.lblSrcPassword.Size = new System.Drawing.Size(56, 13);
            this.lblSrcPassword.TabIndex = 4;
            this.lblSrcPassword.Text = "Password:";
            // 
            // lblSrcUsername
            // 
            this.lblSrcUsername.AutoSize = true;
            this.lblSrcUsername.Location = new System.Drawing.Point(59, 81);
            this.lblSrcUsername.Name = "lblSrcUsername";
            this.lblSrcUsername.Size = new System.Drawing.Size(58, 13);
            this.lblSrcUsername.TabIndex = 3;
            this.lblSrcUsername.Text = "Username:";
            // 
            // lblSrcDefaultCred
            // 
            this.lblSrcDefaultCred.AutoSize = true;
            this.lblSrcDefaultCred.Location = new System.Drawing.Point(18, 54);
            this.lblSrcDefaultCred.Name = "lblSrcDefaultCred";
            this.lblSrcDefaultCred.Size = new System.Drawing.Size(99, 13);
            this.lblSrcDefaultCred.TabIndex = 2;
            this.lblSrcDefaultCred.Text = "Default Credentials:";
            // 
            // lblSrcVersion
            // 
            this.lblSrcVersion.AutoSize = true;
            this.lblSrcVersion.Location = new System.Drawing.Point(259, 54);
            this.lblSrcVersion.Name = "lblSrcVersion";
            this.lblSrcVersion.Size = new System.Drawing.Size(45, 13);
            this.lblSrcVersion.TabIndex = 1;
            this.lblSrcVersion.Text = "Version:";
            // 
            // lblSrcUrl
            // 
            this.lblSrcUrl.AutoSize = true;
            this.lblSrcUrl.Location = new System.Drawing.Point(29, 28);
            this.lblSrcUrl.Name = "lblSrcUrl";
            this.lblSrcUrl.Size = new System.Drawing.Size(88, 13);
            this.lblSrcUrl.TabIndex = 0;
            this.lblSrcUrl.Text = "Web Service Url:";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(459, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(441, 397);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Destination Server";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(917, 584);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSource);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SSRSMigrate";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.grpSource.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.ComboBox cboSrcVersion;
        private System.Windows.Forms.TextBox txtSrcPath;
        private System.Windows.Forms.TextBox txtSrcDomain;
        private System.Windows.Forms.TextBox txtSrcPassword;
        private System.Windows.Forms.TextBox txtSrcUsername;
        private System.Windows.Forms.ComboBox cboSrcDefaultCred;
        private System.Windows.Forms.TextBox txtSrcUrl;
        private System.Windows.Forms.Label lblSrcPath;
        private System.Windows.Forms.Label lblSrcDomain;
        private System.Windows.Forms.Label lblSrcPassword;
        private System.Windows.Forms.Label lblSrcUsername;
        private System.Windows.Forms.Label lblSrcDefaultCred;
        private System.Windows.Forms.Label lblSrcVersion;
        private System.Windows.Forms.Label lblSrcUrl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lstSrcReports;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcPath;
        private System.Windows.Forms.Button btnSrcRefreshReports;
    }
}

