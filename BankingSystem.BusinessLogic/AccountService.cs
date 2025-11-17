using System;
using System.Collections.Generic;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;

        public AccountService()
        {
            _accountRepository = new AccountRepository();
        }

        public Account CreateAccount(int userId, int accountTypeId, decimal initialBalance = 0)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");
            if (accountTypeId <= 0)
                throw new ArgumentException("Invalid account type ID.");

            var accountNumber = _accountRepository.GenerateAccountNumber();
            var account = new Account
            {
                UserId = userId,
                AccountTypeId = accountTypeId,
                AccountNumber = accountNumber,
                Balance = initialBalance,
                IsActive = true
            };

            var accountId = _accountRepository.CreateAccount(account);
            account.AccountId = accountId;
            return account;
        }

        public List<Account> GetUserAccounts(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _accountRepository.GetAccountsByUserId(userId);
        }

        public Account GetAccountByAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.");

            return _accountRepository.GetAccountByAccountNumber(accountNumber);
        }

        public Account GetAccountById(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");

            return _accountRepository.GetAccountById(accountId);
        }

        public List<AccountType> GetAllAccountTypes()
        {
            return _accountRepository.GetAllAccountTypes();
        }
    }
}

