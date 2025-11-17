using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class AccountRepository
    {
        public int CreateAccount(Account account)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_CreateAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserId", account.UserId);
                    command.Parameters.AddWithValue("@AccountTypeId", account.AccountTypeId);
                    command.Parameters.AddWithValue("@AccountNumber", account.AccountNumber);
                    command.Parameters.AddWithValue("@InitialBalance", account.Balance);
                    
                    var accountIdParam = new SqlParameter("@AccountId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(accountIdParam);
                    
                    command.ExecuteNonQuery();
                    return (int)accountIdParam.Value;
                }
            }
        }

        public List<Account> GetAccountsByUserId(int userId)
        {
            var accounts = new List<Account>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT a.*, at.TypeName, at.Description, at.InterestRate, at.MinimumBalance " +
                    "FROM Accounts a " +
                    "INNER JOIN AccountTypes at ON a.AccountTypeId = at.AccountTypeId " +
                    "WHERE a.UserId = @UserId AND a.IsActive = 1", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                AccountId = (int)reader["AccountId"],
                                UserId = (int)reader["UserId"],
                                AccountTypeId = (int)reader["AccountTypeId"],
                                AccountNumber = (string)reader["AccountNumber"],
                                Balance = (decimal)reader["Balance"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                LastModifiedDate = (DateTime)reader["LastModifiedDate"],
                                AccountType = new AccountType
                                {
                                    AccountTypeId = (int)reader["AccountTypeId"],
                                    TypeName = (string)reader["TypeName"],
                                    Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                    InterestRate = (decimal)reader["InterestRate"],
                                    MinimumBalance = (decimal)reader["MinimumBalance"]
                                }
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        public Account GetAccountByAccountNumber(string accountNumber)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT a.*, at.TypeName, at.Description, at.InterestRate, at.MinimumBalance " +
                    "FROM Accounts a " +
                    "INNER JOIN AccountTypes at ON a.AccountTypeId = at.AccountTypeId " +
                    "WHERE a.AccountNumber = @AccountNumber AND a.IsActive = 1", connection))
                {
                    command.Parameters.AddWithValue("@AccountNumber", accountNumber);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Account
                            {
                                AccountId = (int)reader["AccountId"],
                                UserId = (int)reader["UserId"],
                                AccountTypeId = (int)reader["AccountTypeId"],
                                AccountNumber = (string)reader["AccountNumber"],
                                Balance = (decimal)reader["Balance"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                LastModifiedDate = (DateTime)reader["LastModifiedDate"],
                                AccountType = new AccountType
                                {
                                    AccountTypeId = (int)reader["AccountTypeId"],
                                    TypeName = (string)reader["TypeName"],
                                    Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                    InterestRate = (decimal)reader["InterestRate"],
                                    MinimumBalance = (decimal)reader["MinimumBalance"]
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Account GetAccountById(int accountId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT a.*, at.TypeName, at.Description, at.InterestRate, at.MinimumBalance " +
                    "FROM Accounts a " +
                    "INNER JOIN AccountTypes at ON a.AccountTypeId = at.AccountTypeId " +
                    "WHERE a.AccountId = @AccountId AND a.IsActive = 1", connection))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Account
                            {
                                AccountId = (int)reader["AccountId"],
                                UserId = (int)reader["UserId"],
                                AccountTypeId = (int)reader["AccountTypeId"],
                                AccountNumber = (string)reader["AccountNumber"],
                                Balance = (decimal)reader["Balance"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                LastModifiedDate = (DateTime)reader["LastModifiedDate"],
                                AccountType = new AccountType
                                {
                                    AccountTypeId = (int)reader["AccountTypeId"],
                                    TypeName = (string)reader["TypeName"],
                                    Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                    InterestRate = (decimal)reader["InterestRate"],
                                    MinimumBalance = (decimal)reader["MinimumBalance"]
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<AccountType> GetAllAccountTypes()
        {
            var accountTypes = new List<AccountType>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM AccountTypes", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accountTypes.Add(new AccountType
                            {
                                AccountTypeId = (int)reader["AccountTypeId"],
                                TypeName = (string)reader["TypeName"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                InterestRate = (decimal)reader["InterestRate"],
                                MinimumBalance = (decimal)reader["MinimumBalance"]
                            });
                        }
                    }
                }
            }
            return accountTypes;
        }

        public List<Account> GetAllAccounts()
        {
            var accounts = new List<Account>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT a.*, at.TypeName, at.Description, at.InterestRate, at.MinimumBalance " +
                    "FROM Accounts a " +
                    "INNER JOIN AccountTypes at ON a.AccountTypeId = at.AccountTypeId " +
                    "WHERE a.IsActive = 1", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                AccountId = (int)reader["AccountId"],
                                UserId = (int)reader["UserId"],
                                AccountTypeId = (int)reader["AccountTypeId"],
                                AccountNumber = (string)reader["AccountNumber"],
                                Balance = (decimal)reader["Balance"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                LastModifiedDate = (DateTime)reader["LastModifiedDate"],
                                AccountType = new AccountType
                                {
                                    AccountTypeId = (int)reader["AccountTypeId"],
                                    TypeName = (string)reader["TypeName"],
                                    Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                    InterestRate = (decimal)reader["InterestRate"],
                                    MinimumBalance = (decimal)reader["MinimumBalance"]
                                }
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        public bool CloseAccount(int accountId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE Accounts SET IsActive = 0, LastModifiedDate = GETDATE() WHERE AccountId = @AccountId AND Balance = 0", connection))
                {
                    command.Parameters.AddWithValue("@AccountId", accountId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public string GenerateAccountNumber()
        {
            // Generate a unique account number
            return "ACC" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }
    }
}
