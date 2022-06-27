using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public class CacheDefinition : BaseSSRSItem
    {
        /// <summary>
        /// Gets or sets the unique identifier for the cache refresh plan.
        /// </summary>
        public string CacheRefreshPlanID { get; set; }

        /// <summary>
        /// Gets or sets the description of for the cache refresh plan.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the cache refresh plan was last executed by the report server.
        /// </summary>
        public DateTime? LastExecuted { get; set; }

        /// <summary>
        /// Gets the most recent execution status of the cache refresh plan.
        /// </summary>
        public string LastRunStatus { get; set; }

        /// <summary>
        /// Gets the user that last modified the cache refresh plan.
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time the cache refresh plan was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the fully qualified URL of the item with which to associate the cache refresh plan, including the file name (and extension in SharePoint mode).
        /// </summary>
        public string ItemPath { get; set; }



    }
}
