using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public abstract class BaseSSRSItem
    {
        /// <summary>
        /// Source SSRS Object that this item is based upon
        /// </summary>
        [JsonIgnore]
        public object SourceObject { get; set; }
    }
}
