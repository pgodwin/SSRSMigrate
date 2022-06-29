using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public class SubscriptionDefinition : BaseSSRSItem
    {
        /// <summary>
        /// Gets the ID of the subscription.
        /// </summary>
        public string SubscriptionID { get; set; }

        /// <summary>
        /// Gets a description of the format and the delivery method for the reports that are associated with the subscription.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the type of event that triggers the subscription.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Gets the fully qualified URL of the report that is associated with the subscription.
        /// </summary>
        public string ItemPath { get; set; }

        /// <summary>
        /// Gets the name of the report that is associated with the subscription.
        /// </summary>
        public string ReportName { get; set; }

        /// <summary>
        /// Gets the status of a subscription.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets the user name for the owner of the subscription.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets the name of the user who last modified the subscription.
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Gets the date and time the user last modified the subscription.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets the date and time the report server last executed the report.
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the subscription is data-driven.
        /// </summary>
        public bool IsDataDriven { get; set; }

        public string VirtualPath { get; set; }

        public string ActiveState { get; set; }


    }
}
