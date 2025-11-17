using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class AccountInterestService
    {
        private readonly AccountRepository _accountRepository;
        private readonly TransactionRepository _transactionRepository;

        public AccountInterestService()
        {
            _accountRepository = new AccountRepository();
            _transactionRepository = new TransactionRepository();
        }

        public void CalculateAndApplyMonthlyInterest()
        {
            // Get all savings accounts
            var allAccountTypes = _accountRepository.GetAllAccountTypes();
            var savingsTypeId = allAccountTypes.FirstOrDefault(at => at.TypeName == "Savings")?.AccountTypeId ?? 0;
            
            if (savingsTypeId == 0) return;

            var accounts = new List<Account>();
            using (var connection = BankingSystem.DataAccess.DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new System.Data.SqlClient.SqlCommand(
                    "SELECT a.*, at.TypeName, at.Description, at.InterestRate, at.MinimumBalance " +
                    "FROM Accounts a " +
                    "INNER JOIN AccountTypes at ON a.AccountTypeId = at.AccountTypeId " +
                    "WHERE a.AccountTypeId = @AccountTypeId AND a.IsActive = 1", connection))
                {
                    command.Parameters.AddWithValue("@AccountTypeId", savingsTypeId);
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
                                    Description = reader["Description"] == System.DBNull.Value ? null : (string)reader["Description"],
                                    InterestRate = (decimal)reader["InterestRate"],
                                    MinimumBalance = (decimal)reader["MinimumBalance"]
                                }
                            });
                        }
                    }
                }
            }
            foreach (var account in accounts)
            {
                if (account.AccountType.TypeName == "Savings" && account.Balance > 0 && account.IsActive)
                {
                    var monthlyInterest = account.Balance * (account.AccountType.InterestRate / 100 / 12);
                    if (monthlyInterest > 0)
                    {
                        var transaction = new Transaction
                        {
                            AccountId = account.AccountId,
                            TransactionTypeId = _transactionRepository.GetTransactionTypeIdByName("Deposit"),
                            Amount = monthlyInterest,
                            Description = $"Monthly Interest - {DateTime.Now:MMMM yyyy}"
                        };
                        _transactionRepository.ProcessTransaction(transaction);
                    }
                }
            }
        }

        public decimal CalculateProjectedInterest(int accountId, int months)
        {
            var account = _accountRepository.GetAccountById(accountId);
            if (account == null || account.AccountType.TypeName != "Savings")
                return 0;

            var annualRate = account.AccountType.InterestRate / 100;
            var monthlyRate = annualRate / 12;
            return account.Balance * monthlyRate * months;
        }
    }
}

