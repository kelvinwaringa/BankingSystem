using System;
using System.Configuration;
using System.Data.SqlClient;

namespace BankingSystem.DataAccess
{
    public class DatabaseConnection
    {
        private static string _connectionString;

        static DatabaseConnection()
        {
            // Default connection string - can be overridden in app.config
            _connectionString = ConfigurationManager.ConnectionStrings["BankingSystemDB"]?.ConnectionString
                ?? "Server=localhost\\SQLEXPRESS;Database=BankingSystemDB;Integrated Security=True;TrustServerCertificate=True;";
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}

