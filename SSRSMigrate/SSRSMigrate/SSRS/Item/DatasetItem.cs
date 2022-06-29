using AutoMapper;
using Ninject;
using SSRSMigrate.DataMapper;
using SSRSMigrate.SSRS.Item.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public class DataSetItem : 
        ReportServerItem,
        IPolicies,
        IDataSources,
        ICacheOptions
    {
        
        public List<DatasetField> Fields { get; set; }

        public QueryDefinition QueryDefinition { get; set; }
        public byte[] Definition { get; set; }
        public List<DataSourceItem> DataSources { get; set; }
        public CacheDefinition CacheOptions { get; set; }

        public DataSetItem()
        {
            Fields = new List<DatasetField>();
            CacheOptions = new CacheDefinition();
            DataSources = new List<DataSourceItem>();
        }
        
    }
}
