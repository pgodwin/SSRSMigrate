using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Wrappers
{
    public interface ISystemIOWrapper
    {
        bool FileExists(string fileName);
        bool DirectoryExists(string path);
    }
}
