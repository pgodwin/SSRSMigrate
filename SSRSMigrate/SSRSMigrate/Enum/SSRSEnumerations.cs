using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRSMigrate.Enum
{
    /// <summary>
    /// SSRS Version Enumeration
    /// </summary>
    public enum SSRSVersion
    {
        Unknown = 0,
        SqlServer7,
        SqlServer2000,
        SqlServer2005,
        SqlServer2008,
        SqlServer2008R2,
        SqlServer2012
    }
}
