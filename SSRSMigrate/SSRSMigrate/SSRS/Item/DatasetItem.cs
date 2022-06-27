using AutoMapper;
using Ninject;
using SSRSMigrate.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.SSRS.Item
{
    public class DatasetItem : ReportServerItem
    {
        
        public List<DatasetField> Fields { get; set; }

        public QueryDefinition QueryDefinition { get; set; }
        public byte[] Defintion { get; internal set; }

        public DatasetItem()
        {
            Fields = new List<DatasetField>();
        }

        
    }
}
