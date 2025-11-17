using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class TransactionService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly AccountRepository _accountRepository;

        public TransactionService()
        {
            _transactionRepository = new TransactionRepository();
            _accountRepository = new AccountRepository();
        }

        public Transaction ProcessDeposit(int accountId, decimal amount, string description = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.");

            // Validate transaction limits
            var limitsService = new TransactionLimitsService();
            var validation = limitsService.ValidateDeposit(accountId, amount);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            var depositTypeId = _transactionRepository.GetTransactionTypeIdByName("Deposit");
            if (depositTypeId == 0)
                throw new InvalidOperationException("Deposit transaction type not found.");

            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionTypeId = depositTypeId,
                Amount = amount,
                Description = description ?? "Deposit"
            };

            var transactionId = _transactionRepository.ProcessTransaction(transaction);
            transaction.TransactionId = transactionId;

            // Get updated account balance
            var account = _accountRepository.GetAccountById(accountId);
            transaction.BalanceAfterTransaction = account.Balance;

            // Log transaction in audit trail
            AuditService.LogAction(
                account.UserId,
                "Deposit",
                "Transaction",
                transactionId,
                $"Deposit of ${amount:F2} to account {account.AccountNumber}",
                AuditService.GetClientIpAddress()
            );

            return transaction;
        }

        public Transaction ProcessWithdrawal(int accountId, decimal amount, string description = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.");

            // Validate transaction limits
            var limitsService = new TransactionLimitsService();
            var validation = limitsService.ValidateWithdrawal(accountId, amount);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            var withdrawalTypeId = _transactionRepository.GetTransactionTypeIdByName("Withdrawal");
            if (withdrawalTypeId == 0)
                throw new InvalidOperationException("Withdrawal transaction type not found.");

            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionTypeId = withdrawalTypeId,
                Amount = amount,
                Description = description ?? "Withdrawal"
            };

            var transactionId = _transactionRepository.ProcessTransaction(transaction);
            transaction.TransactionId = transactionId;

            // Get updated account balance
            var account = _accountRepository.GetAccountById(accountId);
            transaction.BalanceAfterTransaction = account.Balance;

            // Log transaction in audit trail
            AuditService.LogAction(
                account.UserId,
                "Withdrawal",
                "Transaction",
                transactionId,
                $"Withdrawal of ${amount:F2} from account {account.AccountNumber}",
                AuditService.GetClientIpAddress()
            );

            return transaction;
        }

        public Transaction ProcessTransfer(int fromAccountId, int toAccountId, decimal amount, string description = null)
        {
            if (fromAccountId <= 0 || toAccountId <= 0)
                throw new ArgumentException("Invalid account ID.");
            if (fromAccountId == toAccountId)
                throw new ArgumentException("Cannot transfer to the same account.");
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.");

            var fromAccount = _accountRepository.GetAccountById(fromAccountId);
            if (fromAccount == null)
                throw new InvalidOperationException("Source account not found.");

            var toAccount = _accountRepository.GetAccountById(toAccountId);
            if (toAccount == null)
                throw new InvalidOperationException("Destination account not found.");

            // Validate transaction limits
            var limitsService = new TransactionLimitsService();
            var validation = limitsService.ValidateTransfer(fromAccountId, amount);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            var transferTypeId = _transactionRepository.GetTransactionTypeIdByName("Transfer");
            if (transferTypeId == 0)
                throw new InvalidOperationException("Transfer transaction type not found.");

            var transaction = new Transaction
            {
                AccountId = fromAccountId,
                TransactionTypeId = transferTypeId,
                Amount = amount,
                Description = description ?? $"Transfer to Account {toAccount.AccountNumber}",
                RelatedAccountId = toAccountId
            };

            var transactionId = _transactionRepository.ProcessTransaction(transaction);
            transaction.TransactionId = transactionId;

            // Get updated account balance
            fromAccount = _accountRepository.GetAccountById(fromAccountId);
            transaction.BalanceAfterTransaction = fromAccount.Balance;

            // Log transaction in audit trail
            AuditService.LogAction(
                fromAccount.UserId,
                "Transfer",
                "Transaction",
                transactionId,
                $"Transfer of ${amount:F2} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}",
                AuditService.GetClientIpAddress()
            );

            return transaction;
        }

        public List<Transaction> GetTransactionHistory(int accountId, DateTime? startDate = null, DateTime? endDate = null, string transactionType = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");

            int? transactionTypeId = null;
            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                transactionTypeId = _transactionRepository.GetTransactionTypeIdByName(transactionType);
            }

            return _transactionRepository.GetTransactionHistory(accountId, startDate, endDate, transactionTypeId);
        }

        public List<TransactionType> GetAllTransactionTypes()
        {
            return _transactionRepository.GetAllTransactionTypes();
        }
    }
}

