using System;
using System.Collections.Generic;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class RecurringPaymentService
    {
        private readonly RecurringPaymentRepository _repository;
        private readonly AccountRepository _accountRepository;

        public RecurringPaymentService()
        {
            _repository = new RecurringPaymentRepository();
            _accountRepository = new AccountRepository();
        }

        public RecurringPayment CreateRecurringPayment(int accountId, string recipientName, string recipientAccountNumber, 
            decimal amount, string frequency, DateTime nextPaymentDate, DateTime? endDate, string description)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");
            if (string.IsNullOrWhiteSpace(recipientName))
                throw new ArgumentException("Recipient name is required.");
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Frequency is required.");
            if (!IsValidFrequency(frequency))
                throw new ArgumentException("Invalid frequency. Must be Daily, Weekly, Monthly, or Yearly.");

            var account = _accountRepository.GetAccountById(accountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var payment = new RecurringPayment
            {
                AccountId = accountId,
                RecipientName = recipientName,
                RecipientAccountNumber = recipientAccountNumber,
                Amount = amount,
                Frequency = frequency,
                NextPaymentDate = nextPaymentDate,
                EndDate = endDate,
                Description = description,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var paymentId = _repository.CreateRecurringPayment(payment);
            payment.RecurringPaymentId = paymentId;
            
            AuditService.LogAction(account.UserId, "RecurringPaymentCreated", "RecurringPayment", paymentId, 
                $"Created recurring payment: {recipientName}, Amount: {amount:C}, Frequency: {frequency}");

            return payment;
        }

        public List<RecurringPayment> GetRecurringPaymentsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetRecurringPaymentsByUserId(userId);
        }

        public List<RecurringPayment> GetRecurringPaymentsByAccountId(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");

            return _repository.GetRecurringPaymentsByAccountId(accountId);
        }

        public bool UpdateRecurringPayment(RecurringPayment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));
            if (payment.RecurringPaymentId <= 0)
                throw new ArgumentException("Invalid recurring payment ID.");

            var existingPayment = _repository.GetRecurringPaymentById(payment.RecurringPaymentId);
            if (existingPayment == null)
                throw new ArgumentException("Recurring payment not found.");

            var account = _accountRepository.GetAccountById(existingPayment.AccountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var result = _repository.UpdateRecurringPayment(payment);
            
            if (result)
            {
                AuditService.LogAction(account.UserId, "RecurringPaymentUpdated", "RecurringPayment", payment.RecurringPaymentId, 
                    $"Updated recurring payment: {payment.RecipientName}");
            }

            return result;
        }

        public bool DeleteRecurringPayment(int recurringPaymentId)
        {
            if (recurringPaymentId <= 0)
                throw new ArgumentException("Invalid recurring payment ID.");

            var payment = _repository.GetRecurringPaymentById(recurringPaymentId);
            if (payment == null)
                throw new ArgumentException("Recurring payment not found.");

            var account = _accountRepository.GetAccountById(payment.AccountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var result = _repository.DeleteRecurringPayment(recurringPaymentId);
            
            if (result)
            {
                AuditService.LogAction(account.UserId, "RecurringPaymentDeleted", "RecurringPayment", recurringPaymentId, 
                    $"Deleted recurring payment: {payment.RecipientName}");
            }

            return result;
        }

        public bool DeactivateRecurringPayment(int recurringPaymentId)
        {
            if (recurringPaymentId <= 0)
                throw new ArgumentException("Invalid recurring payment ID.");

            var payment = _repository.GetRecurringPaymentById(recurringPaymentId);
            if (payment == null)
                throw new ArgumentException("Recurring payment not found.");

            payment.IsActive = false;
            return UpdateRecurringPayment(payment);
        }

        private bool IsValidFrequency(string frequency)
        {
            return frequency.Equals("Daily", StringComparison.OrdinalIgnoreCase) ||
                   frequency.Equals("Weekly", StringComparison.OrdinalIgnoreCase) ||
                   frequency.Equals("Monthly", StringComparison.OrdinalIgnoreCase) ||
                   frequency.Equals("Yearly", StringComparison.OrdinalIgnoreCase);
        }

        public DateTime CalculateNextPaymentDate(DateTime currentDate, string frequency)
        {
            switch (frequency.ToLower())
            {
                case "daily":
                    return currentDate.AddDays(1);
                case "weekly":
                    return currentDate.AddDays(7);
                case "monthly":
                    return currentDate.AddMonths(1);
                case "yearly":
                    return currentDate.AddYears(1);
                default:
                    throw new ArgumentException("Invalid frequency.");
            }
        }
    }
}

