using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item.Interfaces
{
    /// <summary>
    /// Inherited objects have DataSources
    /// </summary>
    public interface IDataSources
    {
        List<DataSourceItem> DataSources { get; set; }
    }
}
