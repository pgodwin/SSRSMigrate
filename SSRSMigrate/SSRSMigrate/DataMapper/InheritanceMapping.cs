using AutoMapper;
using Ninject;
using SSRSMigrate.SSRS.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate.DataMapper
{
    public class InheritanceMapping
    {
        private readonly IMapper _mapper;
        public InheritanceMapping(IMapper mapper)
        {
            _mapper = mapper;
        }

        private static InheritanceMapping _instance;
        

        public static InheritanceMapping Instance
        {
            get
            {
                if (_instance == null)
                {
                    IKernel kernel = new StandardKernel(new AutoMapperModule());
                    var mapper = kernel.Get<IMapper>();
                    _instance = new InheritanceMapping(mapper);
                }
                return _instance; ;
            }
        }


        public DataSetItem ToDataSetItem(ReportServerItem item)
        {
            return _mapper.Map<DataSetItem>(item);
        }

        
    }
}
