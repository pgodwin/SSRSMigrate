using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    /// <summary>
    /// Represents an Catalog Item Property. 
    /// </summary>
    [DebuggerDisplay("Name = {Name} / Value = {Value}")]
    public class ItemProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }

    }
}
