

using System;
using System.Data.SqlClient;

namespace SSRSMigrate.Utility
{
    public static class SQLUtil
    {
        private static Func<string> getConnectionStringFunc;

        public static void Initialize(Func<string> connectionStringFunc)
        {
            if (connectionStringFunc == null)
                throw new ArgumentException("connectionStringFunc");

            getConnectionStringFunc = connectionStringFunc;
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = getConnectionStringFunc();

            SqlConnection conn = new SqlConnection(connectionString);

            return conn;
        }

        public static T ExecuteScalar<T>(string query)
        {
            return Execute<T>(c => c.ExecuteScalar<T>(query));
        }

        public static int ExecuteNonQuery(string query)
        {
            return Execute<int>(c => c.ExecuteNonQuery(query));
        }

        private static T Execute<T>(Func<SqlConnection, T> func)
        {
            if (getConnectionStringFunc == null)
                throw new NullReferenceException("SQLUtil must be initialized first by calling Initialize.");

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();

                return func(conn);
            }
        }

        #region SqlConnection Extensions
        public static T ExecuteScalar<T>(this SqlConnection conn, string query)
        {
            using (var command = new SqlCommand(query, conn))
            {
                return (T)command.ExecuteScalar();
            }
        }

        public static int ExecuteNonQuery(this SqlConnection conn, string query)
        {
            using (var command = new SqlCommand(query, conn))
            {
                return command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
