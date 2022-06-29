using SSRSMigrate.SSRS.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.Extensions
{
    public static class ItemExtensions
    {
        public static DataSetItem ToDatasetItem(this ReportServerItem sourceItem)
        {
            return DataMapper.InheritanceMapping.Instance.ToDataSetItem(sourceItem);
        }


    }
}
