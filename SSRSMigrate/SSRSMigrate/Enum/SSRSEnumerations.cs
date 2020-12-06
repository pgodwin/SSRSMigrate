
using System.ComponentModel;

namespace SSRSMigrate.Enum
{
    /// <summary>
    /// SSRS Version Enumeration
    /// </summary>
    public enum SSRSVersion
    {
        [Description("SQL Server Unknown")]
        Unknown = 0,

        [Description("SQL Server 7")]
        SqlServer7,

        [Description("SQL Server 2000")]
        SqlServer2000,

        [Description("SQL Server 2005")]
        SqlServer2005,

        [Description("SQL Server 2008")]
        SqlServer2008,

        [Description("SQL Server 2008 R2")]
        SqlServer2008R2,

        [Description("SQL Server 2012")]
        SqlServer2012,

        [Description("SQL Server 2014")]
        SqlServer2014,

        [Description("SQL Server 2016")]
        SqlServer2016,

        [Description("SQL Server 2017")]
        SqlServer2017,

        [Description("SQL Server 2019")]
        SqlServer2019
    }
}
