using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    /// <summary>
    /// Query Definition from a Dataset Item
    /// </summary>
    public class QueryDefinition : BaseSSRSItem
    {
        public string CommandText { get; set; }
        public string CommandType { get; set; }
        public int? Timeout { get; set; } 
    }
}
