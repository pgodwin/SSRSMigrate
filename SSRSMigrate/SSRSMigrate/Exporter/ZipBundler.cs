using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using SSRSMigrate.Wrappers;

namespace SSRSMigrate.Exporter
{
    public class ZipBundler : IZipBundler
    {
        private readonly IZipFileWrapper mZipFileWrapper = null;

        public ZipBundler(IZipFileWrapper zipFileWrapper)
        {
            if (zipFileWrapper == null)
                throw new ArgumentException("zipFileWrapper");

            this.mZipFileWrapper = zipFileWrapper;
        }

        ~ZipBundler()
        {
            this.mZipFileWrapper.Dispose();
        }
    }
}
