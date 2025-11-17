using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class BillPaymentRepository
    {
        public int CreateBillPayment(BillPayment billPayment)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO BillPayments (AccountId, PayeeName, PayeeAccountNumber, Amount, DueDate, PaymentDate, Status, Description, IsRecurring, RecurringPaymentId, CreatedDate) " +
                    "VALUES (@AccountId, @PayeeName, @PayeeAccountNumber, @Amount, @DueDate, @PaymentDate, @Status, @Description, @IsRecurring, @RecurringPaymentId, GETDATE()); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                {
                    command.Parameters.AddWithValue("@AccountId", billPayment.AccountId);
                    command.Parameters.AddWithValue("@PayeeName", billPayment.PayeeName);
                    command.Parameters.AddWithValue("@PayeeAccountNumber", billPayment.PayeeAccountNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Amount", billPayment.Amount);
                    command.Parameters.AddWithValue("@DueDate", billPayment.DueDate);
                    command.Parameters.AddWithValue("@PaymentDate", billPayment.PaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", billPayment.Status);
                    command.Parameters.AddWithValue("@Description", billPayment.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsRecurring", billPayment.IsRecurring);
                    command.Parameters.AddWithValue("@RecurringPaymentId", billPayment.RecurringPaymentId ?? (object)DBNull.Value);
                    
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public List<BillPayment> GetBillPaymentsByAccountId(int accountId)
        {
            var billPayments = new List<BillPayment>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM BillPayments WHERE AccountId = @AccountId ORDER BY DueDate DESC", connection))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            billPayments.Add(new BillPayment
                            {
                                BillPaymentId = (int)reader["BillPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                PayeeName = (string)reader["PayeeName"],
                                PayeeAccountNumber = reader["PayeeAccountNumber"] == DBNull.Value ? null : (string)reader["PayeeAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                DueDate = (DateTime)reader["DueDate"],
                                PaymentDate = reader["PaymentDate"] == DBNull.Value ? null : (DateTime?)reader["PaymentDate"],
                                Status = (string)reader["Status"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsRecurring = (bool)reader["IsRecurring"],
                                RecurringPaymentId = reader["RecurringPaymentId"] == DBNull.Value ? null : (int?)reader["RecurringPaymentId"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return billPayments;
        }

        public List<BillPayment> GetBillPaymentsByUserId(int userId)
        {
            var billPayments = new List<BillPayment>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT bp.* FROM BillPayments bp " +
                    "INNER JOIN Accounts a ON bp.AccountId = a.AccountId " +
                    "WHERE a.UserId = @UserId ORDER BY bp.DueDate DESC", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            billPayments.Add(new BillPayment
                            {
                                BillPaymentId = (int)reader["BillPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                PayeeName = (string)reader["PayeeName"],
                                PayeeAccountNumber = reader["PayeeAccountNumber"] == DBNull.Value ? null : (string)reader["PayeeAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                DueDate = (DateTime)reader["DueDate"],
                                PaymentDate = reader["PaymentDate"] == DBNull.Value ? null : (DateTime?)reader["PaymentDate"],
                                Status = (string)reader["Status"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsRecurring = (bool)reader["IsRecurring"],
                                RecurringPaymentId = reader["RecurringPaymentId"] == DBNull.Value ? null : (int?)reader["RecurringPaymentId"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return billPayments;
        }

        public BillPayment GetBillPaymentById(int billPaymentId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM BillPayments WHERE BillPaymentId = @BillPaymentId", connection))
                {
                    command.Parameters.AddWithValue("@BillPaymentId", billPaymentId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new BillPayment
                            {
                                BillPaymentId = (int)reader["BillPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                PayeeName = (string)reader["PayeeName"],
                                PayeeAccountNumber = reader["PayeeAccountNumber"] == DBNull.Value ? null : (string)reader["PayeeAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                DueDate = (DateTime)reader["DueDate"],
                                PaymentDate = reader["PaymentDate"] == DBNull.Value ? null : (DateTime?)reader["PaymentDate"],
                                Status = (string)reader["Status"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsRecurring = (bool)reader["IsRecurring"],
                                RecurringPaymentId = reader["RecurringPaymentId"] == DBNull.Value ? null : (int?)reader["RecurringPaymentId"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateBillPayment(BillPayment billPayment)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE BillPayments SET " +
                    "PayeeName = @PayeeName, " +
                    "PayeeAccountNumber = @PayeeAccountNumber, " +
                    "Amount = @Amount, " +
                    "DueDate = @DueDate, " +
                    "PaymentDate = @PaymentDate, " +
                    "Status = @Status, " +
                    "Description = @Description, " +
                    "IsRecurring = @IsRecurring, " +
                    "RecurringPaymentId = @RecurringPaymentId " +
                    "WHERE BillPaymentId = @BillPaymentId", connection))
                {
                    command.Parameters.AddWithValue("@BillPaymentId", billPayment.BillPaymentId);
                    command.Parameters.AddWithValue("@PayeeName", billPayment.PayeeName);
                    command.Parameters.AddWithValue("@PayeeAccountNumber", billPayment.PayeeAccountNumber ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Amount", billPayment.Amount);
                    command.Parameters.AddWithValue("@DueDate", billPayment.DueDate);
                    command.Parameters.AddWithValue("@PaymentDate", billPayment.PaymentDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", billPayment.Status);
                    command.Parameters.AddWithValue("@Description", billPayment.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsRecurring", billPayment.IsRecurring);
                    command.Parameters.AddWithValue("@RecurringPaymentId", billPayment.RecurringPaymentId ?? (object)DBNull.Value);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteBillPayment(int billPaymentId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "DELETE FROM BillPayments WHERE BillPaymentId = @BillPaymentId", connection))
                {
                    command.Parameters.AddWithValue("@BillPaymentId", billPaymentId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<BillPayment> GetOverdueBillPayments(int userId)
        {
            var billPayments = new List<BillPayment>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT bp.* FROM BillPayments bp " +
                    "INNER JOIN Accounts a ON bp.AccountId = a.AccountId " +
                    "WHERE a.UserId = @UserId AND bp.Status = 'Pending' AND bp.DueDate < GETDATE() " +
                    "ORDER BY bp.DueDate", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            billPayments.Add(new BillPayment
                            {
                                BillPaymentId = (int)reader["BillPaymentId"],
                                AccountId = (int)reader["AccountId"],
                                PayeeName = (string)reader["PayeeName"],
                                PayeeAccountNumber = reader["PayeeAccountNumber"] == DBNull.Value ? null : (string)reader["PayeeAccountNumber"],
                                Amount = (decimal)reader["Amount"],
                                DueDate = (DateTime)reader["DueDate"],
                                PaymentDate = reader["PaymentDate"] == DBNull.Value ? null : (DateTime?)reader["PaymentDate"],
                                Status = (string)reader["Status"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsRecurring = (bool)reader["IsRecurring"],
                                RecurringPaymentId = reader["RecurringPaymentId"] == DBNull.Value ? null : (int?)reader["RecurringPaymentId"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return billPayments;
        }
    }
}

