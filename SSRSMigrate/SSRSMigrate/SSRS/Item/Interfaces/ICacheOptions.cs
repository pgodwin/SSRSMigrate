using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    public interface ICacheOptions
    {
        CacheDefinition CacheOptions { get; set; }
    }
}
