
using SSRSMigrate.SSRS.Item.Interfaces;
using System;
using System.Collections.Generic;

namespace SSRSMigrate.SSRS.Item
{
    public class DataSourceItem : 
        ReportServerItem, 
        IEquatable<DataSourceItem>,
        IPolicies,
        ISubscriptions
    {

        /// <summary>
        /// Gets or sets the connection string for a data source.
        /// </summary>
        public string ConnectString { get; set; }
        /// <summary>
        /// Gets or sets a value that indicates the way in which the report server retrieves data source credentials.
        /// </summary>
        public string CredentialsRetrieval { get; set; }
        /// <summary>
        /// Gets a value that indicates whether a data source is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the Enabled property is specified.
        /// </summary>
        public bool EnabledSpecified { get; set; }

        /// <summary>
        /// Gets or sets the name of the data source extension: SQL, OLEDB, ODBC, or a custom extension.
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the report server tries to impersonate a user by using stored credentials.
        /// </summary>
        public bool ImpersonateUser { get; set; }
        /// <summary>
        /// Gets or sets a value that indicates whether the ImpersonateUser property is specified.
        /// </summary>
        public bool ImpersonateUserSpecified { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the original connection string for the data source was expression-based.
        /// </summary>
        public bool OriginalConnectStringExpressionBased { get; set; }
        /// <summary>
        /// Sets the password the report server uses to connect to a data source.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the prompt that the report server displays to the user when it prompts for credentials.
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Gets or sets a value that specifies whether the data source should revert to the original connection string.
        /// </summary>
        public bool UseOriginalConnectString { get; set; }
        /// <summary>
        /// Gets or sets the user name the report server uses to connect to a data source.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the report server passes user-provided or stored credentials as Windows credentials when it connects to a data source.
        /// </summary>
        public bool WindowsCredentials { get; set; }
        /// <inheritdoc />
        public List<SubscriptionDefinition> Subscriptions { get; set; }

        public DataSourceItem()
        {
            this.Subscriptions = new List<SubscriptionDefinition>();   
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
