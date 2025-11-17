using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class RecurringPaymentRepository
    {
        public int CreateRecurringPayment(RecurringPayment payment)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO RecurringPayments (AccountId, RecipientName, RecipientAccountNumber, Amount, Frequency, NextPaymentDate, LastPaymentDate, EndDate, Description, IsActive, CreatedDate) " +
                    "VALUES (@AccountId, @RecipientName, @RecipientAccountNumber, @Amount, @Frequency, @NextPaymentDate, @LastPaymentDate, @EndDate, @Description, @IsActive, GETDATE()); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                {
                    command.Parameters.AddWithValue("@AccountId", payment.AccountId);
                    command.Parameters.AddWithValue("@RecipientName", payment.RecipientName);
                    command.Parameters.AddWithValue("@RecipientAccountNumber", payment.RecipientAccountNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);
                    command.Parameters.AddWithValue("@Frequency", payment.Frequency);
                    command.Parameters.AddWithValue("@NextPaymentDate", payment.NextPaymentDate);
                    command.Parameters.AddWithValue("@LastPaymentDate", payment.LastPaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", payment.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", payment.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", payment.IsActive);
                    
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public List<RecurringPayment> GetRecurringPaymentsByAccountId(int accountId)
        {
            var payments = new List<RecurringPayment>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM RecurringPayments WHERE AccountId = @AccountId ORDER BY NextPaymentDate", connection))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new RecurringPayment
                            {
                                RecurringPaymentId = (int)reader["RecurringPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                RecipientName = (string)reader["RecipientName"],
                                RecipientAccountNumber = reader["RecipientAccountNumber"] == DBNull.Value ? null : (string)reader["RecipientAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                Frequency = (string)reader["Frequency"],
                                NextPaymentDate = (DateTime)reader["NextPaymentDate"],
                                LastPaymentDate = reader["LastPaymentDate"] == DBNull.Value ? null : (DateTime?)reader["LastPaymentDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return payments;
        }

        public List<RecurringPayment> GetRecurringPaymentsByUserId(int userId)
        {
            var payments = new List<RecurringPayment>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT rp.* FROM RecurringPayments rp " +
                    "INNER JOIN Accounts a ON rp.AccountId = a.AccountId " +
                    "WHERE a.UserId = @UserId ORDER BY rp.NextPaymentDate", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new RecurringPayment
                            {
                                RecurringPaymentId = (int)reader["RecurringPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                RecipientName = (string)reader["RecipientName"],
                                RecipientAccountNumber = reader["RecipientAccountNumber"] == DBNull.Value ? null : (string)reader["RecipientAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                Frequency = (string)reader["Frequency"],
                                NextPaymentDate = (DateTime)reader["NextPaymentDate"],
                                LastPaymentDate = reader["LastPaymentDate"] == DBNull.Value ? null : (DateTime?)reader["LastPaymentDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return payments;
        }

        public RecurringPayment GetRecurringPaymentById(int recurringPaymentId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM RecurringPayments WHERE RecurringPaymentId = @RecurringPaymentId", connection))
                {
                    command.Parameters.AddWithValue("@RecurringPaymentId", recurringPaymentId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new RecurringPayment
                            {
                                RecurringPaymentId = (int)reader["RecurringPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                RecipientName = (string)reader["RecipientName"],
                                RecipientAccountNumber = reader["RecipientAccountNumber"] == DBNull.Value ? null : (string)reader["RecipientAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                Frequency = (string)reader["Frequency"],
                                NextPaymentDate = (DateTime)reader["NextPaymentDate"],
                                LastPaymentDate = reader["LastPaymentDate"] == DBNull.Value ? null : (DateTime?)reader["LastPaymentDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateRecurringPayment(RecurringPayment payment)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE RecurringPayments SET " +
                    "RecipientName = @RecipientName, " +
                    "RecipientAccountNumber = @RecipientAccountNumber, " +
                    "Amount = @Amount, " +
                    "Frequency = @Frequency, " +
                    "NextPaymentDate = @NextPaymentDate, " +
                    "LastPaymentDate = @LastPaymentDate, " +
                    "EndDate = @EndDate, " +
                    "Description = @Description, " +
                    "IsActive = @IsActive " +
                    "WHERE RecurringPaymentId = @RecurringPaymentId", connection))
                {
                    command.Parameters.AddWithValue("@RecurringPaymentId", payment.RecurringPaymentId);
                    command.Parameters.AddWithValue("@RecipientName", payment.RecipientName);
                    command.Parameters.AddWithValue("@RecipientAccountNumber", payment.RecipientAccountNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Amount", payment.Amount);
                    command.Parameters.AddWithValue("@Frequency", payment.Frequency);
                    command.Parameters.AddWithValue("@NextPaymentDate", payment.NextPaymentDate);
                    command.Parameters.AddWithValue("@LastPaymentDate", payment.LastPaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", payment.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", payment.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", payment.IsActive);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteRecurringPayment(int recurringPaymentId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "DELETE FROM RecurringPayments WHERE RecurringPaymentId = @RecurringPaymentId", connection))
                {
                    command.Parameters.AddWithValue("@RecurringPaymentId", recurringPaymentId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<RecurringPayment> GetDueRecurringPayments()
        {
            var payments = new List<RecurringPayment>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM RecurringPayments " +
                    "WHERE IsActive = 1 AND NextPaymentDate <= GETDATE() " +
                    "AND (EndDate IS NULL OR EndDate >= GETDATE()) " +
                    "ORDER BY NextPaymentDate", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            payments.Add(new RecurringPayment
                            {
                                RecurringPaymentId = (int)reader["RecurringPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                RecipientName = (string)reader["RecipientName"],
                                RecipientAccountNumber = reader["RecipientAccountNumber"] == DBNull.Value ? null : (string)reader["RecipientAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                Frequency = (string)reader["Frequency"],
                                NextPaymentDate = (DateTime)reader["NextPaymentDate"],
                                LastPaymentDate = reader["LastPaymentDate"] == DBNull.Value ? null : (DateTime?)reader["LastPaymentDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return payments;
        }
    }
}

