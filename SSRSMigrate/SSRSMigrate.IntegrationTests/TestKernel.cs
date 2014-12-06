using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Extensions.Logging.Log4net;

namespace SSRSMigrate.IntegrationTests
{
    public sealed class TestKernel
    {
        private static volatile TestKernel mInstance;
        private static object syncRoot = new object();

        private IKernel mKernel = null;

        private TestKernel()
        {
            var settings = new NinjectSettings()
            {
                LoadExtensions = false
            };

            mKernel = new StandardKernel(
                settings,
                new Log4NetModule(),
                new DependencyModule());

            //mKernel.Load<FuncModule>();
        }

        public static TestKernel Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (mInstance == null)
                            mInstance = new TestKernel();
                    }
                }

                return mInstance;
            }
        }

        public T Get<T>()
        {
            return mKernel.Get<T>();
        }

        public T Get<T>(string name)
        {
            return mKernel.Get<T>(name);
        }
    }
}
