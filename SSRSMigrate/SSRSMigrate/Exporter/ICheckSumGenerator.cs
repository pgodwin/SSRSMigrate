using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SSRSMigrate.Exporter
{
    public interface ICheckSumGenerator
    {
        string CreateCheckSum(string fileName);
        string CreateCheckSum(Stream stream);
    }
}
