using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace SSRSMigrate.IntegrationTests
{
    [CoverageExcludeAttribute]
    public sealed class DependencySingleton
    {
        private static volatile DependencySingleton mInstance;
        private static object syncRoot = new Object();

        private StandardKernel mKernel = null;

        private DependencySingleton()
        {
            mKernel = new StandardKernel(new DependencyModule(true));
        }

        public static DependencySingleton Instance
        {
            get
            {
                if (mInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (mInstance == null)
                            mInstance = new DependencySingleton();
                    }
                }

                return mInstance;
            }
        }

        public T Get<T>()
        {
            return mKernel.Get<T>();
        }
    }
}
