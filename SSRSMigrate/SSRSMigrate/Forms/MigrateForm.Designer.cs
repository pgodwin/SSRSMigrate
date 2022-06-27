namespace SSRSMigrate.Forms
{
    partial class MigrateForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Folders", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Data Sources", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Reports", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Folders", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Data Sources", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Reports", System.Windows.Forms.HorizontalAlignment.Left);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnDebug = new System.Windows.Forms.Button();
            this.btnSrcRefreshReports = new System.Windows.Forms.Button();
            this.lstSrcReports = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuSourceItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uncheckAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPerformMigration = new System.Windows.Forms.Button();
            this.lstDestReports = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.mnuSourceItems.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 796);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip.Size = new System.Drawing.Size(1406, 30);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1183, 23);
            this.lblStatus.Spring = true;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(150, 22);
            // 
            // grpSource
            // 
            this.grpSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSource.Controls.Add(this.btnDebug);
            this.grpSource.Controls.Add(this.btnSrcRefreshReports);
            this.grpSource.Controls.Add(this.lstSrcReports);
            this.grpSource.Location = new System.Drawing.Point(18, 14);
            this.grpSource.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpSource.Name = "grpSource";
            this.grpSource.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpSource.Size = new System.Drawing.Size(693, 774);
            this.grpSource.TabIndex = 1;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Server";
            // 
            // btnDebug
            // 
            this.btnDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDebug.Image = global::SSRSMigrate.Properties.Resources.bug_go;
            this.btnDebug.Location = new System.Drawing.Point(32, 720);
            this.btnDebug.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(48, 35);
            this.btnDebug.TabIndex = 17;
            this.btnDebug.UseVisualStyleBackColor = true;
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // btnSrcRefreshReports
            // 
            this.btnSrcRefreshReports.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSrcRefreshReports.Location = new System.Drawing.Point(458, 720);
            this.btnSrcRefreshReports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSrcRefreshReports.Name = "btnSrcRefreshReports";
            this.btnSrcRefreshReports.Size = new System.Drawing.Size(207, 35);
            this.btnSrcRefreshReports.TabIndex = 15;
            this.btnSrcRefreshReports.Text = "Refresh Reports List";
            this.btnSrcRefreshReports.UseVisualStyleBackColor = true;
            this.btnSrcRefreshReports.Click += new System.EventHandler(this.btnSrcRefreshReports_Click);
            // 
            // lstSrcReports
            // 
            this.lstSrcReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSrcReports.CheckBoxes = true;
            this.lstSrcReports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSrcName,
            this.colSrcPath});
            this.lstSrcReports.ContextMenuStrip = this.mnuSourceItems;
            this.lstSrcReports.FullRowSelect = true;
            this.lstSrcReports.GridLines = true;
            listViewGroup1.Header = "Folders";
            listViewGroup1.Name = "foldersGroup";
            listViewGroup2.Header = "Data Sources";
            listViewGroup2.Name = "dataSourcesGroup";
            listViewGroup3.Header = "Reports";
            listViewGroup3.Name = "reportsGroup";
            this.lstSrcReports.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.lstSrcReports.HideSelection = false;
            this.lstSrcReports.Location = new System.Drawing.Point(32, 29);
            this.lstSrcReports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstSrcReports.MultiSelect = false;
            this.lstSrcReports.Name = "lstSrcReports";
            this.lstSrcReports.Size = new System.Drawing.Size(631, 679);
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
            this.colSrcPath.Width = 387;
            // 
            // mnuSourceItems
            // 
            this.mnuSourceItems.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mnuSourceItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDataSourceToolStripMenuItem,
            this.toolStripSeparator1,
            this.checkAllToolStripMenuItem,
            this.uncheckAllToolStripMenuItem});
            this.mnuSourceItems.Name = "mnuSourceItems";
            this.mnuSourceItems.Size = new System.Drawing.Size(228, 106);
            this.mnuSourceItems.Opening += new System.ComponentModel.CancelEventHandler(this.mnuSourceItems_Opening);
            // 
            // editDataSourceToolStripMenuItem
            // 
            this.editDataSourceToolStripMenuItem.Name = "editDataSourceToolStripMenuItem";
            this.editDataSourceToolStripMenuItem.Size = new System.Drawing.Size(227, 32);
            this.editDataSourceToolStripMenuItem.Text = "Edit Data Source...";
            this.editDataSourceToolStripMenuItem.Click += new System.EventHandler(this.editDataSourceToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(224, 6);
            // 
            // checkAllToolStripMenuItem
            // 
            this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
            this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(227, 32);
            this.checkAllToolStripMenuItem.Text = "Check All";
            this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
            // 
            // uncheckAllToolStripMenuItem
            // 
            this.uncheckAllToolStripMenuItem.Name = "uncheckAllToolStripMenuItem";
            this.uncheckAllToolStripMenuItem.Size = new System.Drawing.Size(227, 32);
            this.uncheckAllToolStripMenuItem.Text = "Uncheck All";
            this.uncheckAllToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnPerformMigration);
            this.groupBox1.Controls.Add(this.lstDestReports);
            this.groupBox1.Location = new System.Drawing.Point(720, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(664, 769);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Destination Server";
            // 
            // btnPerformMigration
            // 
            this.btnPerformMigration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPerformMigration.Enabled = false;
            this.btnPerformMigration.Location = new System.Drawing.Point(27, 715);
            this.btnPerformMigration.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPerformMigration.Name = "btnPerformMigration";
            this.btnPerformMigration.Size = new System.Drawing.Size(207, 35);
            this.btnPerformMigration.TabIndex = 16;
            this.btnPerformMigration.Text = "Perform Migration";
            this.btnPerformMigration.UseVisualStyleBackColor = true;
            this.btnPerformMigration.Click += new System.EventHandler(this.btnPerformMigration_Click);
            // 
            // lstDestReports
            // 
            this.lstDestReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDestReports.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstDestReports.FullRowSelect = true;
            this.lstDestReports.GridLines = true;
            listViewGroup4.Header = "Folders";
            listViewGroup4.Name = "foldersGroup";
            listViewGroup5.Header = "Data Sources";
            listViewGroup5.Name = "dataSourcesGroup";
            listViewGroup6.Header = "Reports";
            listViewGroup6.Name = "reportsGroup";
            this.lstDestReports.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup4,
            listViewGroup5,
            listViewGroup6});
            this.lstDestReports.HideSelection = false;
            this.lstDestReports.Location = new System.Drawing.Point(27, 29);
            this.lstDestReports.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstDestReports.MultiSelect = false;
            this.lstDestReports.Name = "lstDestReports";
            this.lstDestReports.ShowItemToolTips = true;
            this.lstDestReports.Size = new System.Drawing.Size(602, 675);
            this.lstDestReports.TabIndex = 15;
            this.lstDestReports.UseCompatibleStateImageBehavior = false;
            this.lstDestReports.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 161;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Path";
            this.columnHeader2.Width = 234;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Status";
            this.columnHeader3.Width = 152;
            // 
            // MigrateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1406, 826);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSource);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MigrateForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server-to-Server Migration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.grpSource.ResumeLayout(false);
            this.mnuSourceItems.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lstSrcReports;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcPath;
        private System.Windows.Forms.Button btnSrcRefreshReports;
        private System.Windows.Forms.ListView lstDestReports;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnPerformMigration;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip mnuSourceItems;
        private System.Windows.Forms.ToolStripMenuItem checkAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uncheckAllToolStripMenuItem;
        private System.Windows.Forms.Button btnDebug;
        private System.Windows.Forms.ToolStripMenuItem editDataSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}

