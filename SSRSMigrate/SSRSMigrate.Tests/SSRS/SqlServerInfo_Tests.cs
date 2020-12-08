using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SSRSMigrate.Enum;
using SSRSMigrate.SSRS;
using SSRSMigrate.Utility;

namespace SSRSMigrate.Tests.SSRS
{
    [TestFixture, Category("good")]
    [CoverageExcludeAttribute]
    class SqlServerInfo_Tests
    {
        #region IComparable Tests
        [Test]
        public void SqlServerInfo_Equals()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            Assert.AreEqual(a == b, true);
        }

        [Test]
        public void SqlServerInfo_NotEqual()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 15.01.200.8",
                Version = "15.01"
            };

            Assert.AreEqual(a != b, true);
        }

        [Test]
        public void SqlServerInfo_LessThan()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 14.01.200.8",
                Version = "14.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            Assert.AreEqual(a < b, true);
        }

        [Test]
        public void SqlServerInfo_LessThanOrEqualTo_Equal()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            Assert.AreEqual(a <= b, true);
        }

        [Test]
        public void SqlServerInfo_LessThanOrEqualTo_LessThan()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 14.01.200.8",
                Version = "14.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            Assert.AreEqual(a <= b, true);
        }

        [Test]
        public void SqlServerInfo_GreaterThan()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 14.01.200.8",
                Version = "14.01"
            };

            Assert.AreEqual(a > b, true);
        }

        [Test]
        public void SqlServerInfo_GreaterThanOrEqualTo_Equal()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            Assert.AreEqual(a >= b, true);
        }

        [Test]
        public void SqlServerInfo_GreaterThanOrEqualTo_Greater()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 17.01.200.8",
                Version = "17.01"
            };

            SqlServerInfo b = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 16.01.200.8",
                Version = "16.01"
            };

            Assert.AreEqual(a >= b, true);
        }
        #endregion

        #region ToString Tests
        [Test]
        public void SqlServerInfo_ToString()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 15.00.200.8",
                Version = "15.00",
                SsrsVersion = SSRSVersion.SqlServer2019
            };

            string expected = "SQL Server 2019 - 15.00.200.8";
            string actual = a.ToString();

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void SqlServerInfo_ToString_Unknown()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = "Microsoft SQL Server Reporting Services Version 17.00.200.8",
                Version = "17.00",
                SsrsVersion = SSRSVersion.Unknown
            };

            string expected = "SQL Server Unknown - 17.00.200.8";
            string actual = a.ToString();

            Assert.AreEqual(actual, expected);
        }

        [Test]
        public void SqlServerInfo_ToString_UnknownUnknown()
        {
            SqlServerInfo a = new SqlServerInfo()
            {
                FullVersion = null,
                Version = null,
                SsrsVersion = SSRSVersion.Unknown
            };

            string expected = "SQL Server Unknown - Unknown";
            string actual = a.ToString();

            Assert.AreEqual(actual, expected);
        }
        #endregion  
    }
}
