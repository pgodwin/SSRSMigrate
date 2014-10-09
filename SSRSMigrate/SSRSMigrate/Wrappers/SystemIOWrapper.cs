using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Wrappers
{
    /// <summary>
    /// Simple wrapper for common System.IO methods so they can be mocked in tests.
    /// </summary>
    public class SystemIOWrapper : ISystemIOWrapper
    {
        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}
