using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.SSRS.Item
{
    public class ReportServerItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public string ID { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int Size { get; set; }
        public string VirtualPath { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has valid Name and Path properties.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has valid properties; otherwise, <c>false</c>.
        /// </value>
        public bool HasValidProperties
        {
            get
            {
                if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Path))
                    return false;
                else
                    return true;
            }
        }

        public bool ShouldSerializeHasValidProperties()
        {
            // Prevent HasValidProperties from being serialized
            return false;
        }
    }
}
