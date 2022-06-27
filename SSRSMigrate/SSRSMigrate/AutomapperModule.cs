using AutoMapper;
using Ninject;
using Ninject.Modules;
using SSRSMigrate.SSRS.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SSRSMigrate
{
    public class AutoMapperModule : NinjectModule
    {
        public IKernel Kernel { get; set; }

        public override void Load()
        {
            var mapperConfiguration = CreateConfiguration();
            this.Bind<IMapper>().ToConstructor(c => new AutoMapper.Mapper(mapperConfiguration)).InSingletonScope();
        }

     
        public IMapper GetMapper()
        {
            return Kernel.Get<IMapper>();
        }

        private MapperConfiguration CreateConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // Map the base type to the inherited types to make transformations easier
                cfg.CreateMap(typeof(ReportServerItem), typeof(DatasetItem));
                cfg.CreateMap(typeof(ReportServerItem), typeof(ReportItem));
                cfg.CreateMap(typeof(ReportServerItem), typeof(FolderItem));
                cfg.CreateMap(typeof(ReportServerItem), typeof(DataSourceItem));

                // TODO add any others
            });

            return config;
        }

    }
}
