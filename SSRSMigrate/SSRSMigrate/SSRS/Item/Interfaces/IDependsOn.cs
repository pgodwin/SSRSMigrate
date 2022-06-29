using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    /// <summary>
    /// Defines an item that can have items which depend on it (eg Reports, Data Sources, Datasets)
    /// </summary>
    public interface IDependsOn
    {
        List<ItemReferenceDefinition> DependsOn { get; set; }
    }
}
