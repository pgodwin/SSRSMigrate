using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    public interface IPolicies
    {
        /// <summary>
        /// Policies (or permissions/security settings) for the item.
        /// </summary>
        List<PolicyDefinition> Policies { get; set; }
    }
}
