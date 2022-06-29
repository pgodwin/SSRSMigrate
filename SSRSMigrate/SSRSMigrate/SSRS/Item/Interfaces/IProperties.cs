using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    public interface IProperties
    {
        /// <summary>
        /// An array of Property objects that represent the properties of the specified item.
        /// </summary>
        List<ItemProperty> Properties { get; set; }
    }
}
