using SSRSMigrate.Enum;

namespace SSRSMigrate.SSRS
{
    public class SqlServerInfo
    {
        public string RepositoryTag { get; set; }
        public SSRSVersion SsrsVersion { get; set; }
        public string Version { get; set; }
        public string FullVersion { get; set; }
    }
}