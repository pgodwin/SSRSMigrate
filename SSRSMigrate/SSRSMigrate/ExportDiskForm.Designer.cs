namespace SSRSMigrate
{
    partial class ExportDiskForm
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
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Folders", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup5 = new System.Windows.Forms.ListViewGroup("Data Sources", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup6 = new System.Windows.Forms.ListViewGroup("Reports", System.Windows.Forms.HorizontalAlignment.Left);
            this.grpSource = new System.Windows.Forms.GroupBox();
            this.btnSrcRefreshReports = new System.Windows.Forms.Button();
            this.lstSrcReports = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.btnExport = new System.Windows.Forms.Button();
            this.grpSource.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpSource
            // 
            this.grpSource.Controls.Add(this.btnSrcRefreshReports);
            this.grpSource.Controls.Add(this.lstSrcReports);
            this.grpSource.Location = new System.Drawing.Point(12, 12);
            this.grpSource.Name = "grpSource";
            this.grpSource.Size = new System.Drawing.Size(592, 513);
            this.grpSource.TabIndex = 2;
            this.grpSource.TabStop = false;
            this.grpSource.Text = "Source Server";
            // 
            // btnSrcRefreshReports
            // 
            this.btnSrcRefreshReports.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSrcRefreshReports.Location = new System.Drawing.Point(435, 478);
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
            this.colSrcPath});
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
            this.lstSrcReports.Size = new System.Drawing.Size(552, 453);
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
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 562);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(619, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(502, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // btnExport
            // 
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Location = new System.Drawing.Point(517, 531);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(87, 24);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // ExportDiskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 584);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.grpSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ExportDiskForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export to Disk";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportDiskForm_FormClosing);
            this.Load += new System.EventHandler(this.ExportDiskForm_Load);
            this.grpSource.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpSource;
        private System.Windows.Forms.Button btnSrcRefreshReports;
        private System.Windows.Forms.ListView lstSrcReports;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcPath;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Button btnExport;
    }
}