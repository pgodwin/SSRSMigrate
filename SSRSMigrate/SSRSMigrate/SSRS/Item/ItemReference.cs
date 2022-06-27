using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    /// <summary>
    /// Represents a reference to another item on the report server
    /// </summary>
    public class ItemReferenceDefinition : BaseSSRSItem
    {
        /// <summary>
        /// Gets or sets the name of the referenced catalog item in an item definition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The fully qualified URL of the referenced item including the file name(and extension in SharePoint mode).
        /// </summary>
        public string Reference { get; set; }

        public string ReferenceType { get; set; }
    }
}
