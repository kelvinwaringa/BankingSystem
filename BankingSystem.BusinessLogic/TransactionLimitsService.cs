using System;
using System.Linq;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class TransactionLimitsService
    {
        private readonly AccountRepository _accountRepository;
        private readonly TransactionRepository _transactionRepository;

        private const decimal DAILY_WITHDRAWAL_LIMIT = 5000.00m;
        private const decimal DAILY_TRANSFER_LIMIT = 10000.00m;
        private const decimal MAX_TRANSACTION_AMOUNT = 50000.00m;

        public TransactionLimitsService()
        {
            _accountRepository = new AccountRepository();
            _transactionRepository = new TransactionRepository();
        }

        public ValidationResult ValidateWithdrawal(int accountId, decimal amount)
        {
            var result = new ValidationResult { IsValid = true, Message = "" };

            if (amount > MAX_TRANSACTION_AMOUNT)
            {
                result.IsValid = false;
                result.Message = $"Transaction amount exceeds maximum limit of ${MAX_TRANSACTION_AMOUNT:F2}";
                return result;
            }

            // Check daily withdrawal limit
            var today = DateTime.Today;
            var todayWithdrawals = _transactionRepository.GetTransactionHistory(
                accountId, 
                today, 
                today.AddDays(1),
                _transactionRepository.GetTransactionTypeIdByName("Withdrawal")
            );

            var dailyTotal = todayWithdrawals.Sum(t => t.Amount);
            if (dailyTotal + amount > DAILY_WITHDRAWAL_LIMIT)
            {
                result.IsValid = false;
                result.Message = $"Daily withdrawal limit of ${DAILY_WITHDRAWAL_LIMIT:F2} exceeded. " +
                    $"You have already withdrawn ${dailyTotal:F2} today.";
                return result;
            }

            // Check account balance
            var account = _accountRepository.GetAccountById(accountId);
            if (account.Balance < amount)
            {
                result.IsValid = false;
                result.Message = "Insufficient funds.";
                return result;
            }

            // Check minimum balance requirement
            if (account.Balance - amount < account.AccountType.MinimumBalance)
            {
                result.IsValid = false;
                result.Message = $"Withdrawal would violate minimum balance requirement of ${account.AccountType.MinimumBalance:F2}";
                return result;
            }

            return result;
        }

        public ValidationResult ValidateTransfer(int accountId, decimal amount)
        {
            var result = new ValidationResult { IsValid = true, Message = "" };

            if (amount > MAX_TRANSACTION_AMOUNT)
            {
                result.IsValid = false;
                result.Message = $"Transaction amount exceeds maximum limit of ${MAX_TRANSACTION_AMOUNT:F2}";
                return result;
            }

            // Check daily transfer limit
            var today = DateTime.Today;
            var todayTransfers = _transactionRepository.GetTransactionHistory(
                accountId, 
                today, 
                today.AddDays(1),
                _transactionRepository.GetTransactionTypeIdByName("Transfer")
            );

            var dailyTotal = todayTransfers.Sum(t => t.Amount);
            if (dailyTotal + amount > DAILY_TRANSFER_LIMIT)
            {
                result.IsValid = false;
                result.Message = $"Daily transfer limit of ${DAILY_TRANSFER_LIMIT:F2} exceeded. " +
                    $"You have already transferred ${dailyTotal:F2} today.";
                return result;
            }

            // Check account balance
            var account = _accountRepository.GetAccountById(accountId);
            if (account.Balance < amount)
            {
                result.IsValid = false;
                result.Message = "Insufficient funds for transfer.";
                return result;
            }

            return result;
        }

        public ValidationResult ValidateDeposit(int accountId, decimal amount)
        {
            var result = new ValidationResult { IsValid = true, Message = "" };

            if (amount > MAX_TRANSACTION_AMOUNT)
            {
                result.IsValid = false;
                result.Message = $"Deposit amount exceeds maximum limit of ${MAX_TRANSACTION_AMOUNT:F2}";
                return result;
            }

            if (amount <= 0)
            {
                result.IsValid = false;
                result.Message = "Deposit amount must be greater than zero.";
                return result;
            }

            return result;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}

