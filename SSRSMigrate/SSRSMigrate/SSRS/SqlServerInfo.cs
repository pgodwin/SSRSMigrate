using System;
using SSRSMigrate.Enum;

namespace SSRSMigrate.SSRS
{
    public class SqlServerInfo : IComparable<SqlServerInfo>
    {
        public string RepositoryTag { get; set; }
        public SSRSVersion SsrsVersion { get; set; }
        public string Version { get; set; }
        public string FullVersion { get; set; }

        #region IComparable
        public int CompareTo(SqlServerInfo other)
        {
            // If other object is null, this object is greater
            if (object.ReferenceEquals(other, null)) // Do this to avoid recursive death
                return 1;

            // Using decimal because SqlServerInfo.Version will contain a decimal (e.g. '16.00')
            decimal version;

            bool success = decimal.TryParse(this.Version, out version);

            if (success)
            {
                decimal otherVersion;

                bool otherSuccess = decimal.TryParse(other.Version, out otherVersion);

                if (otherSuccess)
                {
                    if (version > otherVersion)
                    {
                        return 1;
                    }
                    else if (version < otherVersion)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    // If other object is not a valid decimal, this object is greater
                    return 1;
                }
            }
            else
            {
                // If this object is not a valid decimal, the other object is greater
                return -1;
            }
        }

        public static bool operator > (SqlServerInfo info1, SqlServerInfo info2)
        {
            return info1.CompareTo(info2) == 1;
        }

        public static bool operator < (SqlServerInfo info1, SqlServerInfo info2)
        {
            return info1.CompareTo(info2) == -1;
        }

        public static bool operator >= (SqlServerInfo info1, SqlServerInfo info2)
        {
            return info1.CompareTo(info2) >= 0;
        }

        public static bool operator <= (SqlServerInfo info1, SqlServerInfo info2)
        {
            return info1.CompareTo(info2) <= 0;
        }

        public static bool operator == (SqlServerInfo info1, SqlServerInfo info2)
        {
            return info1.CompareTo(info2) == 0;
        }

        public static bool operator != (SqlServerInfo info1, SqlServerInfo info2)
        {
            return info1.CompareTo(info2) != 0;
        }
        #endregion
    }
}