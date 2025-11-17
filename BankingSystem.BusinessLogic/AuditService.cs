using System;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.DataAccess;

namespace BankingSystem.BusinessLogic
{
    public class AuditService
    {
        public static void LogAction(int? userId, string action, string entityType = null, int? entityId = null, string details = null, string ipAddress = null)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var command = new SqlCommand(
                        "INSERT INTO AuditLogs (UserId, Action, EntityType, EntityId, Details, IpAddress, Timestamp) " +
                        "VALUES (@UserId, @Action, @EntityType, @EntityId, @Details, @IpAddress, GETDATE())", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Action", action);
                        command.Parameters.AddWithValue("@EntityType", entityType ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@EntityId", entityId.HasValue ? (object)entityId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@Details", details ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IpAddress", ipAddress ?? (object)DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Silent fail - audit logging should not break the application
            }
        }

        public static string GetClientIpAddress()
        {
            try
            {
                return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}

