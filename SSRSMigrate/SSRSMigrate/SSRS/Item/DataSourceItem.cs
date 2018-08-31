
using System;

namespace SSRSMigrate.SSRS.Item
{
    public class DataSourceItem : ReportServerItem, IEquatable<DataSourceItem>
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

        public bool Equals(DataSourceItem other)
        {
            if (other == null)
                return false;

            if (!string.IsNullOrEmpty(this.ConnectString))
            {
                if (!this.ConnectString.Equals(other.ConnectString))
                    return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(other.ConnectString))
                    return false;
            }

            if (!string.IsNullOrEmpty(this.Path))
                if (!this.Path.Equals(other.Path))
                    return false;

            if (!string.IsNullOrEmpty(this.Name))
                if (!this.Name.Equals(other.Name))
                    return false;

            if (this.WindowsCredentials != other.WindowsCredentials)
                return false;

            if (this.ImpersonateUser != other.ImpersonateUser)
                return false;

            if (!string.IsNullOrEmpty(this.UserName))
            {
                if (!this.UserName.Equals(other.UserName))
                    return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(other.UserName))
                    return false;
            }

            if (!string.IsNullOrEmpty(this.Password))
            {
                if (!this.Password.Equals(other.Password))
                    return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(other.Password))
                    return false;
            }

            if (!string.IsNullOrEmpty(this.CredentialsRetrieval))
            {
                if (!this.CredentialsRetrieval.Equals(other.CredentialsRetrieval))
                    return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(other.CredentialsRetrieval))
                    return false;
            }
                
           return true;
        }
    }
}
