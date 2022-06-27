using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    [Serializable]
    public class DatasetField : BaseSSRSItem
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
