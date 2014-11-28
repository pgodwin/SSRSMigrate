using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSRSMigrate.Forms
{
    public partial class SummaryForm : Form
    {
        private int mSuccessfulItemCount = 0;
        private int mFailedItemCount = 0;
        private Dictionary<string, string> mFailedItems = new Dictionary<string, string>();

        #region Properties
        public int SuccessfulItemsCount
        {
            set { this.mSuccessfulItemCount = value;  }
        }
        #endregion

        public SummaryForm()
        {
            InitializeComponent();
        }

        private void SummaryForm_Load(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void LoadData()
        {
            this.AddFailedData();
            this.AddSuccessfulData();
        }

        private void AddSuccessfulData()
        {
            ListViewItem oSuccessItem = new ListViewItem("Successful Items:");
            oSuccessItem.SubItems.Add(this.mSuccessfulItemCount.ToString());
            oSuccessItem.Group = this.lstSummary.Groups["lstGrpSummary"];

            this.lstSummary.Items.Add(oSuccessItem);

            ListViewItem oFailedItem = new ListViewItem("Failed Items:");
            oFailedItem.SubItems.Add(this.mFailedItemCount.ToString());
            oFailedItem.Group = this.lstSummary.Groups["lstGrpSummary"];

            this.lstSummary.Items.Add(oFailedItem);

            oFailedItem.EnsureVisible();
        }

        private void AddFailedData()
        {
            foreach (KeyValuePair<string, string> kv in this.mFailedItems)
            {
                ListViewItem oItem = new ListViewItem(kv.Key);
                oItem.SubItems.Add(kv.Value);
                oItem.Group = this.lstSummary.Groups["lstGrpFailed"];

                this.lstSummary.Items.Add(oItem);
                oItem.EnsureVisible();
            }
        }

        public void AddFailedItem(string item, string message)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentException("item");

            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            if (this.mFailedItems.ContainsKey(item))
                return;

            this.mFailedItems.Add(item, message);
            this.mFailedItemCount += 1;
        }
    }
}
