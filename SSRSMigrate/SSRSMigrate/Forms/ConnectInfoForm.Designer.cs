namespace SSRSMigrate.Forms
{
    partial class ConnectInfoForm
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
            this.grpMigrateMethod = new System.Windows.Forms.GroupBox();
            this.rdoMethodExportDisk = new System.Windows.Forms.RadioButton();
            this.lblMethodInfo = new System.Windows.Forms.Label();
            this.rdoMethodImportZip = new System.Windows.Forms.RadioButton();
            this.rdoMethodExportZip = new System.Windows.Forms.RadioButton();
            this.rdoMethodDirect = new System.Windows.Forms.RadioButton();
            this.grpDestServer = new System.Windows.Forms.GroupBox();
            this.txtDestSqlVersion = new System.Windows.Forms.TextBox();
            this.lblDestSqlVersion = new System.Windows.Forms.Label();
            this.btnBrowseScript = new System.Windows.Forms.Button();
            this.txtScriptPath = new System.Windows.Forms.TextBox();
            this.chkExecuteScript = new System.Windows.Forms.CheckBox();
            this.btnDestTest = new System.Windows.Forms.Button();
            this.cbkDestOverwrite = new System.Windows.Forms.CheckBox();
            this.txtDestPath = new System.Windows.Forms.TextBox();
            this.txtDestDomain = new System.Windows.Forms.TextBox();
            this.txtDestPassword = new System.Windows.Forms.TextBox();
            this.txtDestUsername = new System.Windows.Forms.TextBox();
            this.cboDestDefaultCred = new System.Windows.Forms.ComboBox();
            this.txtDestUrl = new System.Windows.Forms.TextBox();
            this.lblDestPath = new System.Windows.Forms.Label();
            this.lblDestDomain = new System.Windows.Forms.Label();
            this.lblDestPassword = new System.Windows.Forms.Label();
            this.lblDestUsername = new System.Windows.Forms.Label();
            this.lblDestDefaultCred = new System.Windows.Forms.Label();
            this.lblDestUrl = new System.Windows.Forms.Label();
            this.grpExportDisk = new System.Windows.Forms.GroupBox();
            this.btnExportDiskFolderBrowse = new System.Windows.Forms.Button();
            this.txtExportDiskFolderName = new System.Windows.Forms.TextBox();
            this.lblExportDiskFolderName = new System.Windows.Forms.Label();
            this.grpExportZip = new System.Windows.Forms.GroupBox();
            this.btnExportZipFileBrowse = new System.Windows.Forms.Button();
            this.txtExportZipFilename = new System.Windows.Forms.TextBox();
            this.lblExportZipFilename = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblSrcUrl = new System.Windows.Forms.Label();
            this.lblSrcDefaultCred = new System.Windows.Forms.Label();
            this.lblSrcUsername = new System.Windows.Forms.Label();
            this.lblSrcPassword = new System.Windows.Forms.Label();
            this.lblSrcDomain = new System.Windows.Forms.Label();
            this.lblSrcPath = new System.Windows.Forms.Label();
            this.txtSrcUrl = new System.Windows.Forms.TextBox();
            this.cboSrcDefaultCred = new System.Windows.Forms.ComboBox();
            this.txtSrcUsername = new System.Windows.Forms.TextBox();
            this.txtSrcPassword = new System.Windows.Forms.TextBox();
            this.txtSrcDomain = new System.Windows.Forms.TextBox();
            this.txtSrcPath = new System.Windows.Forms.TextBox();
            this.grpSrcServer = new System.Windows.Forms.GroupBox();
            this.txtSrcSqlVersion = new System.Windows.Forms.TextBox();
            this.lblSrcSqlVersion = new System.Windows.Forms.Label();
            this.btnSrcTest = new System.Windows.Forms.Button();
            this.grpImportZip = new System.Windows.Forms.GroupBox();
            this.btnImportZipBrowse = new System.Windows.Forms.Button();
            this.txtImportZipFilename = new System.Windows.Forms.TextBox();
            this.lblImportZipFilename = new System.Windows.Forms.Label();
            this.grpMigrateMethod.SuspendLayout();
            this.grpDestServer.SuspendLayout();
            this.grpExportDisk.SuspendLayout();
            this.grpExportZip.SuspendLayout();
            this.grpSrcServer.SuspendLayout();
            this.grpImportZip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMigrateMethod
            // 
            this.grpMigrateMethod.Controls.Add(this.rdoMethodExportDisk);
            this.grpMigrateMethod.Controls.Add(this.lblMethodInfo);
            this.grpMigrateMethod.Controls.Add(this.rdoMethodImportZip);
            this.grpMigrateMethod.Controls.Add(this.rdoMethodExportZip);
            this.grpMigrateMethod.Controls.Add(this.rdoMethodDirect);
            this.grpMigrateMethod.Location = new System.Drawing.Point(12, 12);
            this.grpMigrateMethod.Name = "grpMigrateMethod";
            this.grpMigrateMethod.Size = new System.Drawing.Size(189, 291);
            this.grpMigrateMethod.TabIndex = 0;
            this.grpMigrateMethod.TabStop = false;
            this.grpMigrateMethod.Text = "Migration Method";
            // 
            // rdoMethodExportDisk
            // 
            this.rdoMethodExportDisk.AutoSize = true;
            this.rdoMethodExportDisk.Location = new System.Drawing.Point(19, 38);
            this.rdoMethodExportDisk.Name = "rdoMethodExportDisk";
            this.rdoMethodExportDisk.Size = new System.Drawing.Size(89, 17);
            this.rdoMethodExportDisk.TabIndex = 4;
            this.rdoMethodExportDisk.Text = "Export to disk";
            this.rdoMethodExportDisk.UseVisualStyleBackColor = true;
            this.rdoMethodExportDisk.CheckedChanged += new System.EventHandler(this.rdoMethodExportDisk_CheckedChanged);
            this.rdoMethodExportDisk.MouseLeave += new System.EventHandler(this.rdoMethodExportDisk_MouseLeave);
            this.rdoMethodExportDisk.MouseHover += new System.EventHandler(this.rdoMethodExportDisk_MouseHover);
            // 
            // lblMethodInfo
            // 
            this.lblMethodInfo.Location = new System.Drawing.Point(11, 102);
            this.lblMethodInfo.Name = "lblMethodInfo";
            this.lblMethodInfo.Size = new System.Drawing.Size(166, 114);
            this.lblMethodInfo.TabIndex = 3;
            // 
            // rdoMethodImportZip
            // 
            this.rdoMethodImportZip.AutoSize = true;
            this.rdoMethodImportZip.Location = new System.Drawing.Point(19, 74);
            this.rdoMethodImportZip.Name = "rdoMethodImportZip";
            this.rdoMethodImportZip.Size = new System.Drawing.Size(135, 17);
            this.rdoMethodImportZip.TabIndex = 2;
            this.rdoMethodImportZip.Text = "Import from ZIP archive";
            this.rdoMethodImportZip.UseVisualStyleBackColor = true;
            this.rdoMethodImportZip.CheckedChanged += new System.EventHandler(this.rdoMethodImportZip_CheckedChanged);
            this.rdoMethodImportZip.MouseLeave += new System.EventHandler(this.rdoMethodImportZip_MouseLeave);
            this.rdoMethodImportZip.MouseHover += new System.EventHandler(this.rdoMethodImportZip_MouseHover);
            // 
            // rdoMethodExportZip
            // 
            this.rdoMethodExportZip.AutoSize = true;
            this.rdoMethodExportZip.Location = new System.Drawing.Point(19, 56);
            this.rdoMethodExportZip.Name = "rdoMethodExportZip";
            this.rdoMethodExportZip.Size = new System.Drawing.Size(125, 17);
            this.rdoMethodExportZip.TabIndex = 1;
            this.rdoMethodExportZip.Text = "Export to ZIP archive";
            this.rdoMethodExportZip.UseVisualStyleBackColor = true;
            this.rdoMethodExportZip.CheckedChanged += new System.EventHandler(this.rdoMethodExportZip_CheckedChanged);
            this.rdoMethodExportZip.MouseLeave += new System.EventHandler(this.rdoMethodExportZip_MouseLeave);
            this.rdoMethodExportZip.MouseHover += new System.EventHandler(this.rdoMethodExportZip_MouseHover);
            // 
            // rdoMethodDirect
            // 
            this.rdoMethodDirect.AutoSize = true;
            this.rdoMethodDirect.Checked = true;
            this.rdoMethodDirect.Location = new System.Drawing.Point(19, 21);
            this.rdoMethodDirect.Name = "rdoMethodDirect";
            this.rdoMethodDirect.Size = new System.Drawing.Size(148, 17);
            this.rdoMethodDirect.TabIndex = 0;
            this.rdoMethodDirect.TabStop = true;
            this.rdoMethodDirect.Text = "Server-to-Server Migration";
            this.rdoMethodDirect.UseVisualStyleBackColor = true;
            this.rdoMethodDirect.CheckedChanged += new System.EventHandler(this.rdoMethodDirect_CheckedChanged);
            this.rdoMethodDirect.MouseLeave += new System.EventHandler(this.rdoMethodDirect_MouseLeave);
            this.rdoMethodDirect.MouseHover += new System.EventHandler(this.rdoMethodDirect_MouseHover);
            // 
            // grpDestServer
            // 
            this.grpDestServer.Controls.Add(this.txtDestSqlVersion);
            this.grpDestServer.Controls.Add(this.lblDestSqlVersion);
            this.grpDestServer.Controls.Add(this.btnBrowseScript);
            this.grpDestServer.Controls.Add(this.txtScriptPath);
            this.grpDestServer.Controls.Add(this.chkExecuteScript);
            this.grpDestServer.Controls.Add(this.btnDestTest);
            this.grpDestServer.Controls.Add(this.cbkDestOverwrite);
            this.grpDestServer.Controls.Add(this.txtDestPath);
            this.grpDestServer.Controls.Add(this.txtDestDomain);
            this.grpDestServer.Controls.Add(this.txtDestPassword);
            this.grpDestServer.Controls.Add(this.txtDestUsername);
            this.grpDestServer.Controls.Add(this.cboDestDefaultCred);
            this.grpDestServer.Controls.Add(this.txtDestUrl);
            this.grpDestServer.Controls.Add(this.lblDestPath);
            this.grpDestServer.Controls.Add(this.lblDestDomain);
            this.grpDestServer.Controls.Add(this.lblDestPassword);
            this.grpDestServer.Controls.Add(this.lblDestUsername);
            this.grpDestServer.Controls.Add(this.lblDestDefaultCred);
            this.grpDestServer.Controls.Add(this.lblDestUrl);
            this.grpDestServer.Location = new System.Drawing.Point(572, 12);
            this.grpDestServer.Name = "grpDestServer";
            this.grpDestServer.Size = new System.Drawing.Size(350, 295);
            this.grpDestServer.TabIndex = 2;
            this.grpDestServer.TabStop = false;
            this.grpDestServer.Text = "Destination Server";
            // 
            // txtDestSqlVersion
            // 
            this.txtDestSqlVersion.Enabled = false;
            this.txtDestSqlVersion.Location = new System.Drawing.Point(113, 199);
            this.txtDestSqlVersion.Name = "txtDestSqlVersion";
            this.txtDestSqlVersion.ReadOnly = true;
            this.txtDestSqlVersion.Size = new System.Drawing.Size(220, 20);
            this.txtDestSqlVersion.TabIndex = 34;
            // 
            // lblDestSqlVersion
            // 
            this.lblDestSqlVersion.AutoSize = true;
            this.lblDestSqlVersion.Location = new System.Drawing.Point(59, 202);
            this.lblDestSqlVersion.Name = "lblDestSqlVersion";
            this.lblDestSqlVersion.Size = new System.Drawing.Size(45, 13);
            this.lblDestSqlVersion.TabIndex = 33;
            this.lblDestSqlVersion.Text = "Version:";
            // 
            // btnBrowseScript
            // 
            this.btnBrowseScript.Location = new System.Drawing.Point(309, 248);
            this.btnBrowseScript.Name = "btnBrowseScript";
            this.btnBrowseScript.Size = new System.Drawing.Size(24, 23);
            this.btnBrowseScript.TabIndex = 32;
            this.btnBrowseScript.Text = "...";
            this.btnBrowseScript.UseVisualStyleBackColor = true;
            this.btnBrowseScript.Visible = false;
            this.btnBrowseScript.Click += new System.EventHandler(this.btnBrowseScript_Click);
            // 
            // txtScriptPath
            // 
            this.txtScriptPath.Location = new System.Drawing.Point(21, 251);
            this.txtScriptPath.Name = "txtScriptPath";
            this.txtScriptPath.Size = new System.Drawing.Size(282, 20);
            this.txtScriptPath.TabIndex = 31;
            this.txtScriptPath.Visible = false;
            // 
            // chkExecuteScript
            // 
            this.chkExecuteScript.AutoSize = true;
            this.chkExecuteScript.Location = new System.Drawing.Point(21, 227);
            this.chkExecuteScript.Name = "chkExecuteScript";
            this.chkExecuteScript.Size = new System.Drawing.Size(95, 17);
            this.chkExecuteScript.TabIndex = 30;
            this.chkExecuteScript.Text = "Execute Script";
            this.chkExecuteScript.UseVisualStyleBackColor = true;
            this.chkExecuteScript.CheckedChanged += new System.EventHandler(this.chkExecuteScript_CheckedChanged);
            // 
            // btnDestTest
            // 
            this.btnDestTest.Location = new System.Drawing.Point(280, 149);
            this.btnDestTest.Name = "btnDestTest";
            this.btnDestTest.Size = new System.Drawing.Size(53, 23);
            this.btnDestTest.TabIndex = 29;
            this.btnDestTest.Text = "Test";
            this.btnDestTest.UseVisualStyleBackColor = true;
            this.btnDestTest.Click += new System.EventHandler(this.btnDestTest_Click);
            // 
            // cbkDestOverwrite
            // 
            this.cbkDestOverwrite.AutoSize = true;
            this.cbkDestOverwrite.Location = new System.Drawing.Point(113, 176);
            this.cbkDestOverwrite.Name = "cbkDestOverwrite";
            this.cbkDestOverwrite.Size = new System.Drawing.Size(71, 17);
            this.cbkDestOverwrite.TabIndex = 28;
            this.cbkDestOverwrite.Text = "Overwrite";
            this.cbkDestOverwrite.UseVisualStyleBackColor = true;
            // 
            // txtDestPath
            // 
            this.txtDestPath.Location = new System.Drawing.Point(113, 152);
            this.txtDestPath.Name = "txtDestPath";
            this.txtDestPath.Size = new System.Drawing.Size(161, 20);
            this.txtDestPath.TabIndex = 26;
            this.txtDestPath.Text = "/";
            // 
            // txtDestDomain
            // 
            this.txtDestDomain.Location = new System.Drawing.Point(113, 126);
            this.txtDestDomain.Name = "txtDestDomain";
            this.txtDestDomain.Size = new System.Drawing.Size(121, 20);
            this.txtDestDomain.TabIndex = 25;
            // 
            // txtDestPassword
            // 
            this.txtDestPassword.Location = new System.Drawing.Point(113, 100);
            this.txtDestPassword.Name = "txtDestPassword";
            this.txtDestPassword.PasswordChar = '*';
            this.txtDestPassword.Size = new System.Drawing.Size(121, 20);
            this.txtDestPassword.TabIndex = 24;
            // 
            // txtDestUsername
            // 
            this.txtDestUsername.Location = new System.Drawing.Point(113, 73);
            this.txtDestUsername.Name = "txtDestUsername";
            this.txtDestUsername.Size = new System.Drawing.Size(121, 20);
            this.txtDestUsername.TabIndex = 23;
            // 
            // cboDestDefaultCred
            // 
            this.cboDestDefaultCred.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDestDefaultCred.FormattingEnabled = true;
            this.cboDestDefaultCred.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cboDestDefaultCred.Location = new System.Drawing.Point(113, 46);
            this.cboDestDefaultCred.Name = "cboDestDefaultCred";
            this.cboDestDefaultCred.Size = new System.Drawing.Size(121, 21);
            this.cboDestDefaultCred.TabIndex = 22;
            this.cboDestDefaultCred.SelectedIndexChanged += new System.EventHandler(this.cboDestDefaultCred_SelectedIndexChanged);
            // 
            // txtDestUrl
            // 
            this.txtDestUrl.Location = new System.Drawing.Point(113, 20);
            this.txtDestUrl.Name = "txtDestUrl";
            this.txtDestUrl.Size = new System.Drawing.Size(220, 20);
            this.txtDestUrl.TabIndex = 21;
            this.txtDestUrl.Text = "http://localhost/ReportServer";
            // 
            // lblDestPath
            // 
            this.lblDestPath.AutoSize = true;
            this.lblDestPath.Location = new System.Drawing.Point(73, 155);
            this.lblDestPath.Name = "lblDestPath";
            this.lblDestPath.Size = new System.Drawing.Size(32, 13);
            this.lblDestPath.TabIndex = 20;
            this.lblDestPath.Text = "Path:";
            // 
            // lblDestDomain
            // 
            this.lblDestDomain.AutoSize = true;
            this.lblDestDomain.Location = new System.Drawing.Point(59, 129);
            this.lblDestDomain.Name = "lblDestDomain";
            this.lblDestDomain.Size = new System.Drawing.Size(46, 13);
            this.lblDestDomain.TabIndex = 19;
            this.lblDestDomain.Text = "Domain:";
            // 
            // lblDestPassword
            // 
            this.lblDestPassword.AutoSize = true;
            this.lblDestPassword.Location = new System.Drawing.Point(49, 100);
            this.lblDestPassword.Name = "lblDestPassword";
            this.lblDestPassword.Size = new System.Drawing.Size(56, 13);
            this.lblDestPassword.TabIndex = 18;
            this.lblDestPassword.Text = "Password:";
            // 
            // lblDestUsername
            // 
            this.lblDestUsername.AutoSize = true;
            this.lblDestUsername.Location = new System.Drawing.Point(49, 76);
            this.lblDestUsername.Name = "lblDestUsername";
            this.lblDestUsername.Size = new System.Drawing.Size(58, 13);
            this.lblDestUsername.TabIndex = 17;
            this.lblDestUsername.Text = "Username:";
            // 
            // lblDestDefaultCred
            // 
            this.lblDestDefaultCred.AutoSize = true;
            this.lblDestDefaultCred.Location = new System.Drawing.Point(8, 49);
            this.lblDestDefaultCred.Name = "lblDestDefaultCred";
            this.lblDestDefaultCred.Size = new System.Drawing.Size(99, 13);
            this.lblDestDefaultCred.TabIndex = 16;
            this.lblDestDefaultCred.Text = "Default Credentials:";
            // 
            // lblDestUrl
            // 
            this.lblDestUrl.AutoSize = true;
            this.lblDestUrl.Location = new System.Drawing.Point(19, 23);
            this.lblDestUrl.Name = "lblDestUrl";
            this.lblDestUrl.Size = new System.Drawing.Size(88, 13);
            this.lblDestUrl.TabIndex = 14;
            this.lblDestUrl.Text = "Web Service Url:";
            // 
            // grpExportDisk
            // 
            this.grpExportDisk.Controls.Add(this.btnExportDiskFolderBrowse);
            this.grpExportDisk.Controls.Add(this.txtExportDiskFolderName);
            this.grpExportDisk.Controls.Add(this.lblExportDiskFolderName);
            this.grpExportDisk.Location = new System.Drawing.Point(572, 12);
            this.grpExportDisk.Name = "grpExportDisk";
            this.grpExportDisk.Size = new System.Drawing.Size(350, 295);
            this.grpExportDisk.TabIndex = 31;
            this.grpExportDisk.TabStop = false;
            this.grpExportDisk.Text = "Export to disk";
            this.grpExportDisk.Visible = false;
            // 
            // btnExportDiskFolderBrowse
            // 
            this.btnExportDiskFolderBrowse.Location = new System.Drawing.Point(303, 23);
            this.btnExportDiskFolderBrowse.Name = "btnExportDiskFolderBrowse";
            this.btnExportDiskFolderBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnExportDiskFolderBrowse.TabIndex = 2;
            this.btnExportDiskFolderBrowse.Text = "...";
            this.btnExportDiskFolderBrowse.UseVisualStyleBackColor = true;
            this.btnExportDiskFolderBrowse.Click += new System.EventHandler(this.btnExportDiskFolderBrowse_Click);
            // 
            // txtExportDiskFolderName
            // 
            this.txtExportDiskFolderName.Location = new System.Drawing.Point(76, 26);
            this.txtExportDiskFolderName.Name = "txtExportDiskFolderName";
            this.txtExportDiskFolderName.Size = new System.Drawing.Size(221, 20);
            this.txtExportDiskFolderName.TabIndex = 1;
            // 
            // lblExportDiskFolderName
            // 
            this.lblExportDiskFolderName.AutoSize = true;
            this.lblExportDiskFolderName.Location = new System.Drawing.Point(19, 29);
            this.lblExportDiskFolderName.Name = "lblExportDiskFolderName";
            this.lblExportDiskFolderName.Size = new System.Drawing.Size(39, 13);
            this.lblExportDiskFolderName.TabIndex = 0;
            this.lblExportDiskFolderName.Text = "Folder:";
            // 
            // grpExportZip
            // 
            this.grpExportZip.Controls.Add(this.btnExportZipFileBrowse);
            this.grpExportZip.Controls.Add(this.txtExportZipFilename);
            this.grpExportZip.Controls.Add(this.lblExportZipFilename);
            this.grpExportZip.Location = new System.Drawing.Point(572, 12);
            this.grpExportZip.Name = "grpExportZip";
            this.grpExportZip.Size = new System.Drawing.Size(350, 295);
            this.grpExportZip.TabIndex = 29;
            this.grpExportZip.TabStop = false;
            this.grpExportZip.Text = "Export to ZIP Archive";
            this.grpExportZip.Visible = false;
            // 
            // btnExportZipFileBrowse
            // 
            this.btnExportZipFileBrowse.Location = new System.Drawing.Point(303, 23);
            this.btnExportZipFileBrowse.Name = "btnExportZipFileBrowse";
            this.btnExportZipFileBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnExportZipFileBrowse.TabIndex = 2;
            this.btnExportZipFileBrowse.Text = "...";
            this.btnExportZipFileBrowse.UseVisualStyleBackColor = true;
            this.btnExportZipFileBrowse.Click += new System.EventHandler(this.btnExportZipFileBrowse_Click);
            // 
            // txtExportZipFilename
            // 
            this.txtExportZipFilename.Location = new System.Drawing.Point(76, 26);
            this.txtExportZipFilename.Name = "txtExportZipFilename";
            this.txtExportZipFilename.Size = new System.Drawing.Size(221, 20);
            this.txtExportZipFilename.TabIndex = 1;
            // 
            // lblExportZipFilename
            // 
            this.lblExportZipFilename.AutoSize = true;
            this.lblExportZipFilename.Location = new System.Drawing.Point(19, 29);
            this.lblExportZipFilename.Name = "lblExportZipFilename";
            this.lblExportZipFilename.Size = new System.Drawing.Size(52, 13);
            this.lblExportZipFilename.TabIndex = 0;
            this.lblExportZipFilename.Text = "Filename:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(805, 313);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(117, 31);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblSrcUrl
            // 
            this.lblSrcUrl.AutoSize = true;
            this.lblSrcUrl.Location = new System.Drawing.Point(19, 23);
            this.lblSrcUrl.Name = "lblSrcUrl";
            this.lblSrcUrl.Size = new System.Drawing.Size(88, 13);
            this.lblSrcUrl.TabIndex = 14;
            this.lblSrcUrl.Text = "Web Service Url:";
            // 
            // lblSrcDefaultCred
            // 
            this.lblSrcDefaultCred.AutoSize = true;
            this.lblSrcDefaultCred.Location = new System.Drawing.Point(8, 49);
            this.lblSrcDefaultCred.Name = "lblSrcDefaultCred";
            this.lblSrcDefaultCred.Size = new System.Drawing.Size(99, 13);
            this.lblSrcDefaultCred.TabIndex = 16;
            this.lblSrcDefaultCred.Text = "Default Credentials:";
            // 
            // lblSrcUsername
            // 
            this.lblSrcUsername.AutoSize = true;
            this.lblSrcUsername.Location = new System.Drawing.Point(49, 76);
            this.lblSrcUsername.Name = "lblSrcUsername";
            this.lblSrcUsername.Size = new System.Drawing.Size(58, 13);
            this.lblSrcUsername.TabIndex = 17;
            this.lblSrcUsername.Text = "Username:";
            // 
            // lblSrcPassword
            // 
            this.lblSrcPassword.AutoSize = true;
            this.lblSrcPassword.Location = new System.Drawing.Point(49, 100);
            this.lblSrcPassword.Name = "lblSrcPassword";
            this.lblSrcPassword.Size = new System.Drawing.Size(56, 13);
            this.lblSrcPassword.TabIndex = 18;
            this.lblSrcPassword.Text = "Password:";
            // 
            // lblSrcDomain
            // 
            this.lblSrcDomain.AutoSize = true;
            this.lblSrcDomain.Location = new System.Drawing.Point(59, 129);
            this.lblSrcDomain.Name = "lblSrcDomain";
            this.lblSrcDomain.Size = new System.Drawing.Size(46, 13);
            this.lblSrcDomain.TabIndex = 19;
            this.lblSrcDomain.Text = "Domain:";
            // 
            // lblSrcPath
            // 
            this.lblSrcPath.AutoSize = true;
            this.lblSrcPath.Location = new System.Drawing.Point(73, 155);
            this.lblSrcPath.Name = "lblSrcPath";
            this.lblSrcPath.Size = new System.Drawing.Size(32, 13);
            this.lblSrcPath.TabIndex = 20;
            this.lblSrcPath.Text = "Path:";
            // 
            // txtSrcUrl
            // 
            this.txtSrcUrl.Location = new System.Drawing.Point(113, 20);
            this.txtSrcUrl.Name = "txtSrcUrl";
            this.txtSrcUrl.Size = new System.Drawing.Size(220, 20);
            this.txtSrcUrl.TabIndex = 21;
            this.txtSrcUrl.Text = "http://localhost/ReportServer";
            // 
            // cboSrcDefaultCred
            // 
            this.cboSrcDefaultCred.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSrcDefaultCred.FormattingEnabled = true;
            this.cboSrcDefaultCred.Items.AddRange(new object[] {
            "True",
            "False"});
            this.cboSrcDefaultCred.Location = new System.Drawing.Point(113, 46);
            this.cboSrcDefaultCred.Name = "cboSrcDefaultCred";
            this.cboSrcDefaultCred.Size = new System.Drawing.Size(121, 21);
            this.cboSrcDefaultCred.TabIndex = 22;
            this.cboSrcDefaultCred.SelectedIndexChanged += new System.EventHandler(this.cboSrcDefaultCred_SelectedIndexChanged);
            // 
            // txtSrcUsername
            // 
            this.txtSrcUsername.Location = new System.Drawing.Point(113, 73);
            this.txtSrcUsername.Name = "txtSrcUsername";
            this.txtSrcUsername.Size = new System.Drawing.Size(121, 20);
            this.txtSrcUsername.TabIndex = 23;
            // 
            // txtSrcPassword
            // 
            this.txtSrcPassword.Location = new System.Drawing.Point(113, 100);
            this.txtSrcPassword.Name = "txtSrcPassword";
            this.txtSrcPassword.PasswordChar = '*';
            this.txtSrcPassword.Size = new System.Drawing.Size(121, 20);
            this.txtSrcPassword.TabIndex = 24;
            // 
            // txtSrcDomain
            // 
            this.txtSrcDomain.Location = new System.Drawing.Point(113, 126);
            this.txtSrcDomain.Name = "txtSrcDomain";
            this.txtSrcDomain.Size = new System.Drawing.Size(121, 20);
            this.txtSrcDomain.TabIndex = 25;
            // 
            // txtSrcPath
            // 
            this.txtSrcPath.Location = new System.Drawing.Point(113, 152);
            this.txtSrcPath.Name = "txtSrcPath";
            this.txtSrcPath.Size = new System.Drawing.Size(161, 20);
            this.txtSrcPath.TabIndex = 26;
            this.txtSrcPath.Text = "/";
            // 
            // grpSrcServer
            // 
            this.grpSrcServer.Controls.Add(this.txtSrcSqlVersion);
            this.grpSrcServer.Controls.Add(this.lblSrcSqlVersion);
            this.grpSrcServer.Controls.Add(this.btnSrcTest);
            this.grpSrcServer.Controls.Add(this.txtSrcPath);
            this.grpSrcServer.Controls.Add(this.txtSrcDomain);
            this.grpSrcServer.Controls.Add(this.txtSrcPassword);
            this.grpSrcServer.Controls.Add(this.txtSrcUsername);
            this.grpSrcServer.Controls.Add(this.cboSrcDefaultCred);
            this.grpSrcServer.Controls.Add(this.txtSrcUrl);
            this.grpSrcServer.Controls.Add(this.lblSrcPath);
            this.grpSrcServer.Controls.Add(this.lblSrcDomain);
            this.grpSrcServer.Controls.Add(this.lblSrcPassword);
            this.grpSrcServer.Controls.Add(this.lblSrcUsername);
            this.grpSrcServer.Controls.Add(this.lblSrcDefaultCred);
            this.grpSrcServer.Controls.Add(this.lblSrcUrl);
            this.grpSrcServer.Location = new System.Drawing.Point(207, 12);
            this.grpSrcServer.Name = "grpSrcServer";
            this.grpSrcServer.Size = new System.Drawing.Size(359, 295);
            this.grpSrcServer.TabIndex = 1;
            this.grpSrcServer.TabStop = false;
            this.grpSrcServer.Text = "Source Server";
            // 
            // txtSrcSqlVersion
            // 
            this.txtSrcSqlVersion.Enabled = false;
            this.txtSrcSqlVersion.Location = new System.Drawing.Point(113, 180);
            this.txtSrcSqlVersion.Name = "txtSrcSqlVersion";
            this.txtSrcSqlVersion.ReadOnly = true;
            this.txtSrcSqlVersion.Size = new System.Drawing.Size(220, 20);
            this.txtSrcSqlVersion.TabIndex = 36;
            // 
            // lblSrcSqlVersion
            // 
            this.lblSrcSqlVersion.AutoSize = true;
            this.lblSrcSqlVersion.Location = new System.Drawing.Point(59, 183);
            this.lblSrcSqlVersion.Name = "lblSrcSqlVersion";
            this.lblSrcSqlVersion.Size = new System.Drawing.Size(45, 13);
            this.lblSrcSqlVersion.TabIndex = 35;
            this.lblSrcSqlVersion.Text = "Version:";
            // 
            // btnSrcTest
            // 
            this.btnSrcTest.Location = new System.Drawing.Point(280, 151);
            this.btnSrcTest.Name = "btnSrcTest";
            this.btnSrcTest.Size = new System.Drawing.Size(53, 23);
            this.btnSrcTest.TabIndex = 28;
            this.btnSrcTest.Text = "Test";
            this.btnSrcTest.UseVisualStyleBackColor = true;
            this.btnSrcTest.Click += new System.EventHandler(this.btnSrcTest_Click);
            // 
            // grpImportZip
            // 
            this.grpImportZip.Controls.Add(this.btnImportZipBrowse);
            this.grpImportZip.Controls.Add(this.txtImportZipFilename);
            this.grpImportZip.Controls.Add(this.lblImportZipFilename);
            this.grpImportZip.Location = new System.Drawing.Point(207, 12);
            this.grpImportZip.Name = "grpImportZip";
            this.grpImportZip.Size = new System.Drawing.Size(359, 295);
            this.grpImportZip.TabIndex = 33;
            this.grpImportZip.TabStop = false;
            this.grpImportZip.Text = "Import from ZIP archive";
            this.grpImportZip.Visible = false;
            // 
            // btnImportZipBrowse
            // 
            this.btnImportZipBrowse.Location = new System.Drawing.Point(309, 20);
            this.btnImportZipBrowse.Name = "btnImportZipBrowse";
            this.btnImportZipBrowse.Size = new System.Drawing.Size(24, 23);
            this.btnImportZipBrowse.TabIndex = 2;
            this.btnImportZipBrowse.Text = "...";
            this.btnImportZipBrowse.UseVisualStyleBackColor = true;
            this.btnImportZipBrowse.Click += new System.EventHandler(this.btnImportZipBrowse_Click);
            // 
            // txtImportZipFilename
            // 
            this.txtImportZipFilename.Location = new System.Drawing.Point(84, 22);
            this.txtImportZipFilename.Name = "txtImportZipFilename";
            this.txtImportZipFilename.Size = new System.Drawing.Size(219, 20);
            this.txtImportZipFilename.TabIndex = 1;
            // 
            // lblImportZipFilename
            // 
            this.lblImportZipFilename.AutoSize = true;
            this.lblImportZipFilename.Location = new System.Drawing.Point(11, 25);
            this.lblImportZipFilename.Name = "lblImportZipFilename";
            this.lblImportZipFilename.Size = new System.Drawing.Size(67, 13);
            this.lblImportZipFilename.TabIndex = 0;
            this.lblImportZipFilename.Text = "Zip filename:";
            // 
            // ConnectInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 353);
            this.Controls.Add(this.grpImportZip);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.grpExportZip);
            this.Controls.Add(this.grpDestServer);
            this.Controls.Add(this.grpExportDisk);
            this.Controls.Add(this.grpSrcServer);
            this.Controls.Add(this.grpMigrateMethod);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ConnectInfoForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SSRSMigrate";
            this.Load += new System.EventHandler(this.ConnectInfoForm_Load);
            this.grpMigrateMethod.ResumeLayout(false);
            this.grpMigrateMethod.PerformLayout();
            this.grpDestServer.ResumeLayout(false);
            this.grpDestServer.PerformLayout();
            this.grpExportDisk.ResumeLayout(false);
            this.grpExportDisk.PerformLayout();
            this.grpExportZip.ResumeLayout(false);
            this.grpExportZip.PerformLayout();
            this.grpSrcServer.ResumeLayout(false);
            this.grpSrcServer.PerformLayout();
            this.grpImportZip.ResumeLayout(false);
            this.grpImportZip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMigrateMethod;
        private System.Windows.Forms.RadioButton rdoMethodDirect;
        private System.Windows.Forms.RadioButton rdoMethodImportZip;
        private System.Windows.Forms.RadioButton rdoMethodExportZip;
        private System.Windows.Forms.Label lblMethodInfo;
        private System.Windows.Forms.GroupBox grpDestServer;
        private System.Windows.Forms.CheckBox cbkDestOverwrite;
        private System.Windows.Forms.TextBox txtDestPath;
        private System.Windows.Forms.TextBox txtDestDomain;
        private System.Windows.Forms.TextBox txtDestPassword;
        private System.Windows.Forms.TextBox txtDestUsername;
        private System.Windows.Forms.ComboBox cboDestDefaultCred;
        private System.Windows.Forms.TextBox txtDestUrl;
        private System.Windows.Forms.Label lblDestPath;
        private System.Windows.Forms.Label lblDestDomain;
        private System.Windows.Forms.Label lblDestPassword;
        private System.Windows.Forms.Label lblDestUsername;
        private System.Windows.Forms.Label lblDestDefaultCred;
        private System.Windows.Forms.Label lblDestUrl;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox grpExportZip;
        private System.Windows.Forms.TextBox txtExportZipFilename;
        private System.Windows.Forms.Label lblExportZipFilename;
        private System.Windows.Forms.RadioButton rdoMethodExportDisk;
        private System.Windows.Forms.Button btnExportZipFileBrowse;
        private System.Windows.Forms.GroupBox grpExportDisk;
        private System.Windows.Forms.Button btnExportDiskFolderBrowse;
        private System.Windows.Forms.TextBox txtExportDiskFolderName;
        private System.Windows.Forms.Label lblExportDiskFolderName;
        private System.Windows.Forms.Label lblSrcUrl;
        private System.Windows.Forms.Label lblSrcDefaultCred;
        private System.Windows.Forms.Label lblSrcUsername;
        private System.Windows.Forms.Label lblSrcPassword;
        private System.Windows.Forms.Label lblSrcDomain;
        private System.Windows.Forms.Label lblSrcPath;
        private System.Windows.Forms.TextBox txtSrcUrl;
        private System.Windows.Forms.ComboBox cboSrcDefaultCred;
        private System.Windows.Forms.TextBox txtSrcUsername;
        private System.Windows.Forms.TextBox txtSrcPassword;
        private System.Windows.Forms.TextBox txtSrcDomain;
        private System.Windows.Forms.TextBox txtSrcPath;
        private System.Windows.Forms.GroupBox grpSrcServer;
        private System.Windows.Forms.GroupBox grpImportZip;
        private System.Windows.Forms.Button btnImportZipBrowse;
        private System.Windows.Forms.TextBox txtImportZipFilename;
        private System.Windows.Forms.Label lblImportZipFilename;
        private System.Windows.Forms.Button btnSrcTest;
        private System.Windows.Forms.Button btnDestTest;
        private System.Windows.Forms.Button btnBrowseScript;
        private System.Windows.Forms.TextBox txtScriptPath;
        private System.Windows.Forms.CheckBox chkExecuteScript;
        private System.Windows.Forms.TextBox txtDestSqlVersion;
        private System.Windows.Forms.Label lblDestSqlVersion;
        private System.Windows.Forms.TextBox txtSrcSqlVersion;
        private System.Windows.Forms.Label lblSrcSqlVersion;
    }
}