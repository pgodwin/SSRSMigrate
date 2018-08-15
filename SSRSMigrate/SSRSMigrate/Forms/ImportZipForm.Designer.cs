namespace SSRSMigrate.Forms
{
    partial class ImportZipForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPerformImport = new System.Windows.Forms.Button();
            this.lstDestReports = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnDebug = new System.Windows.Forms.Button();
            this.btnSrcRefreshReports = new System.Windows.Forms.Button();
            this.lstSrcReports = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colError = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colZipPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colExtractedTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuSourceItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.groupBox1.SuspendLayout();
            this.grpSource.SuspendLayout();
            this.mnuSourceItems.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnPerformImport);
            this.groupBox1.Controls.Add(this.lstDestReports);
            this.groupBox1.Location = new System.Drawing.Point(482, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(443, 500);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Destination Server";
            // 
            // btnPerformImport
            // 
            this.btnPerformImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPerformImport.Enabled = false;
            this.btnPerformImport.Location = new System.Drawing.Point(18, 465);
            this.btnPerformImport.Name = "btnPerformImport";
            this.btnPerformImport.Size = new System.Drawing.Size(138, 23);
            this.btnPerformImport.TabIndex = 16;
            this.btnPerformImport.Text = "Perform Import";
            this.btnPerformImport.UseVisualStyleBackColor = true;
            this.btnPerformImport.Click += new System.EventHandler(this.btnPerformImport_Click);
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
            listViewGroup1.Header = "Folders";
            listViewGroup1.Name = "foldersGroup";
            listViewGroup2.Header = "Data Sources";
            listViewGroup2.Name = "dataSourcesGroup";
            listViewGroup3.Header = "Reports";
            listViewGroup3.Name = "reportsGroup";
            this.lstDestReports.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.lstDestReports.Location = new System.Drawing.Point(18, 19);
            this.lstDestReports.MultiSelect = false;
            this.lstDestReports.Name = "lstDestReports";
            this.lstDestReports.ShowItemToolTips = true;
            this.lstDestReports.Size = new System.Drawing.Size(403, 440);
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
            // grpSource
            // 
            this.grpSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSource.Controls.Add(this.btnDebug);
            this.grpSource.Controls.Add(this.btnSrcRefreshReports);
            this.grpSource.Controls.Add(this.lstSrcReports);
            this.grpSource.Location = new System.Drawing.Point(12, 12);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(464, 500);
            this.grpSource.TabIndex = 3;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Archive";
            // 
            // btnDebug
            // 
            this.btnDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDebug.Image = global::SSRSMigrate.Properties.Resources.bug_go;
            this.btnDebug.Location = new System.Drawing.Point(21, 465);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(32, 23);
            this.btnDebug.TabIndex = 17;
            this.btnDebug.UseVisualStyleBackColor = true;
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // btnSrcRefreshReports
            // 
            this.btnSrcRefreshReports.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSrcRefreshReports.Location = new System.Drawing.Point(307, 465);
            this.btnSrcRefreshReports.Name = "btnSrcRefreshReports";
            this.btnSrcRefreshReports.Size = new System.Drawing.Size(138, 23);
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
            this.colSrcPath,
            this.colError,
            this.colZipPath,
            this.colExtractedTo});
            this.lstSrcReports.ContextMenuStrip = this.mnuSourceItems;
            this.lstSrcReports.FullRowSelect = true;
            this.lstSrcReports.GridLines = true;
            listViewGroup4.Header = "Folders";
            listViewGroup4.Name = "foldersGroup";
            listViewGroup5.Header = "Data Sources";
            listViewGroup5.Name = "dataSourcesGroup";
            listViewGroup6.Header = "Reports";
            listViewGroup6.Name = "reportsGroup";
            this.lstSrcReports.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup4,
            listViewGroup5,
            listViewGroup6});
            this.lstSrcReports.Location = new System.Drawing.Point(21, 19);
            this.lstSrcReports.MultiSelect = false;
            this.lstSrcReports.Name = "lstSrcReports";
            this.lstSrcReports.Size = new System.Drawing.Size(424, 440);
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
            this.colSrcPath.Width = 181;
            // 
            // colError
            // 
            this.colError.Text = "Error";
            this.colError.Width = 90;
            // 
            // colZipPath
            // 
            this.colZipPath.Text = "Zip Path";
            // 
            // colExtractedTo
            // 
            this.colExtractedTo.Text = "Extracted To";
            // 
            // mnuSourceItems
            // 
            this.mnuSourceItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editDataSourceToolStripMenuItem});
            this.mnuSourceItems.Name = "mnuSourceItems";
            this.mnuSourceItems.Size = new System.Drawing.Size(170, 26);
            this.mnuSourceItems.Opening += new System.ComponentModel.CancelEventHandler(this.mnuSourceItems_Opening);
            // 
            // editDataSourceToolStripMenuItem
            // 
            this.editDataSourceToolStripMenuItem.Name = "editDataSourceToolStripMenuItem";
            this.editDataSourceToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.editDataSourceToolStripMenuItem.Text = "Edit Data Source...";
            this.editDataSourceToolStripMenuItem.Click += new System.EventHandler(this.editDataSourceToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 515);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(937, 22);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(820, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // ImportZipForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 537);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ImportZipForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Import from Zip";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportZipForm_FormClosing);
            this.Load += new System.EventHandler(this.ImportZipForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.grpSource.ResumeLayout(false);
            this.mnuSourceItems.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPerformImport;
        private System.Windows.Forms.ListView lstDestReports;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Button btnDebug;
        private System.Windows.Forms.Button btnSrcRefreshReports;
        private System.Windows.Forms.ListView lstSrcReports;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcPath;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ColumnHeader colError;
        private System.Windows.Forms.ColumnHeader colZipPath;
        private System.Windows.Forms.ColumnHeader colExtractedTo;
        private System.Windows.Forms.ContextMenuStrip mnuSourceItems;
        private System.Windows.Forms.ToolStripMenuItem editDataSourceToolStripMenuItem;
    }
}