using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class TransactionRepository
    {
        public int ProcessTransaction(Transaction transaction)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_ProcessTransaction", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AccountId", transaction.AccountId);
                    command.Parameters.AddWithValue("@TransactionTypeId", transaction.TransactionTypeId);
                    command.Parameters.AddWithValue("@Amount", transaction.Amount);
                    command.Parameters.AddWithValue("@Description", (object)transaction.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@RelatedAccountId", (object)transaction.RelatedAccountId ?? DBNull.Value);
                    
                    var transactionIdParam = new SqlParameter("@TransactionId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(transactionIdParam);
                    
                    try
                    {
                        command.ExecuteNonQuery();
                        return (int)transactionIdParam.Value;
                    }
                    catch (SqlException ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
        }

        public List<Transaction> GetTransactionHistory(int accountId, DateTime? startDate = null, DateTime? endDate = null, int? transactionTypeId = null)
        {
            var transactions = new List<Transaction>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_GetTransactionHistory", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    command.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", (object)endDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TransactionTypeId", (object)transactionTypeId ?? DBNull.Value);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new Transaction
                            {
                                TransactionId = (int)reader["TransactionId"],
                                AccountId = (int)reader["AccountId"],
                                TransactionTypeId = (int)reader["TransactionTypeId"],
                                Amount = (decimal)reader["Amount"],
                                BalanceAfterTransaction = (decimal)reader["BalanceAfterTransaction"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                RelatedAccountId = reader["RelatedAccountId"] == DBNull.Value ? (int?)null : (int)reader["RelatedAccountId"],
                                TransactionDate = (DateTime)reader["TransactionDate"],
                                TransactionType = new TransactionType
                                {
                                    TransactionTypeId = (int)reader["TransactionTypeId"],
                                    TypeName = (string)reader["TransactionType"]
                                }
                            });
                        }
                    }
                }
            }
            return transactions;
        }

        public List<TransactionType> GetAllTransactionTypes()
        {
            var transactionTypes = new List<TransactionType>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM TransactionTypes", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactionTypes.Add(new TransactionType
                            {
                                TransactionTypeId = (int)reader["TransactionTypeId"],
                                TypeName = (string)reader["TypeName"]
                            });
                        }
                    }
                }
            }
            return transactionTypes;
        }

        public int GetTransactionTypeIdByName(string typeName)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT TransactionTypeId FROM TransactionTypes WHERE TypeName = @TypeName", connection))
                {
                    command.Parameters.AddWithValue("@TypeName", typeName);
                    var result = command.ExecuteScalar();
                    return result != null ? (int)result : 0;
                }
            }
        }
    }
}

