
namespace SSRSMigrate.SSRS.Item
{
    public class DataSourceItem : ReportServerItem
    {
        public string ConnectString { get; set; }
        public string CredentialsRetrieval { get; set; }
        public bool Enabled { get; set; }
        public bool EnabledSpecified { get; set; }
        public string Extension { get; set; }
        public bool ImpersonateUser { get; set; }
        public bool ImpersonateUserSpecified { get; set; }
        public bool OriginalConnectStringExpressionBased { get; set; }
        public string Password { get; set; }
        public string Prompt { get; set; }
        public bool UseOriginalConnectString { get; set; }
        public string UserName { get; set; }
        public bool WindowsCredentials { get; set; }

        public DataSourceItem()
        {
            
        }
    }
}
