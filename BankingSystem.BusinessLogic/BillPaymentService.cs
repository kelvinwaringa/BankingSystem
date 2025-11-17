using System;
using System.Collections.Generic;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class BillPaymentService
    {
        private readonly BillPaymentRepository _repository;
        private readonly AccountRepository _accountRepository;
        private readonly TransactionService _transactionService;

        public BillPaymentService()
        {
            _repository = new BillPaymentRepository();
            _accountRepository = new AccountRepository();
            _transactionService = new TransactionService();
        }

        public BillPayment CreateBillPayment(int accountId, string payeeName, string payeeAccountNumber, 
            decimal amount, DateTime dueDate, string description, bool isRecurring = false, int? recurringPaymentId = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");
            if (string.IsNullOrWhiteSpace(payeeName))
                throw new ArgumentException("Payee name is required.");
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var account = _accountRepository.GetAccountById(accountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var billPayment = new BillPayment
            {
                AccountId = accountId,
                PayeeName = payeeName,
                PayeeAccountNumber = payeeAccountNumber,
                Amount = amount,
                DueDate = dueDate,
                Status = "Pending",
                Description = description,
                IsRecurring = isRecurring,
                RecurringPaymentId = recurringPaymentId,
                CreatedDate = DateTime.Now
            };

            var billPaymentId = _repository.CreateBillPayment(billPayment);
            billPayment.BillPaymentId = billPaymentId;
            
            AuditService.LogAction(account.UserId, "BillPaymentCreated", "BillPayment", billPaymentId, 
                $"Created bill payment: {payeeName}, Amount: {amount:C}, Due: {dueDate:yyyy-MM-dd}");

            return billPayment;
        }

        public List<BillPayment> GetBillPaymentsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetBillPaymentsByUserId(userId);
        }

        public List<BillPayment> GetBillPaymentsByAccountId(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");

            return _repository.GetBillPaymentsByAccountId(accountId);
        }

        public List<BillPayment> GetOverdueBillPayments(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetOverdueBillPayments(userId);
        }

        public bool PayBill(int billPaymentId, int accountId)
        {
            if (billPaymentId <= 0)
                throw new ArgumentException("Invalid bill payment ID.");
            if (accountId <= 0)
                throw new ArgumentException("Invalid account ID.");

            var billPayment = _repository.GetBillPaymentById(billPaymentId);
            if (billPayment == null)
                throw new ArgumentException("Bill payment not found.");

            if (billPayment.Status == "Paid")
                throw new InvalidOperationException("This bill has already been paid.");

            var account = _accountRepository.GetAccountById(accountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            if (account.Balance < billPayment.Amount)
                throw new InvalidOperationException("Insufficient funds to pay this bill.");

            // Process the payment as a withdrawal
            _transactionService.ProcessWithdrawal(accountId, billPayment.Amount, 
                $"Bill payment to {billPayment.PayeeName}");

            // Update bill payment status
            billPayment.Status = "Paid";
            billPayment.PaymentDate = DateTime.Now;
            var result = _repository.UpdateBillPayment(billPayment);

            if (result)
            {
                AuditService.LogAction(account.UserId, "BillPaymentPaid", "BillPayment", billPaymentId, 
                    $"Paid bill: {billPayment.PayeeName}, Amount: {billPayment.Amount:C}");
            }

            return result;
        }

        public bool UpdateBillPayment(BillPayment billPayment)
        {
            if (billPayment == null)
                throw new ArgumentNullException(nameof(billPayment));
            if (billPayment.BillPaymentId <= 0)
                throw new ArgumentException("Invalid bill payment ID.");

            var existingBill = _repository.GetBillPaymentById(billPayment.BillPaymentId);
            if (existingBill == null)
                throw new ArgumentException("Bill payment not found.");

            var account = _accountRepository.GetAccountById(existingBill.AccountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var result = _repository.UpdateBillPayment(billPayment);
            
            if (result)
            {
                AuditService.LogAction(account.UserId, "BillPaymentUpdated", "BillPayment", billPayment.BillPaymentId, 
                    $"Updated bill payment: {billPayment.PayeeName}");
            }

            return result;
        }

        public bool CancelBillPayment(int billPaymentId)
        {
            if (billPaymentId <= 0)
                throw new ArgumentException("Invalid bill payment ID.");

            var billPayment = _repository.GetBillPaymentById(billPaymentId);
            if (billPayment == null)
                throw new ArgumentException("Bill payment not found.");

            if (billPayment.Status == "Paid")
                throw new InvalidOperationException("Cannot cancel a paid bill.");

            billPayment.Status = "Cancelled";
            var result = _repository.UpdateBillPayment(billPayment);

            if (result)
            {
                var account = _accountRepository.GetAccountById(billPayment.AccountId);
                if (account != null)
                {
                    AuditService.LogAction(account.UserId, "BillPaymentCancelled", "BillPayment", billPaymentId, 
                        $"Cancelled bill payment: {billPayment.PayeeName}");
                }
            }

            return result;
        }

        public bool DeleteBillPayment(int billPaymentId)
        {
            if (billPaymentId <= 0)
                throw new ArgumentException("Invalid bill payment ID.");

            var billPayment = _repository.GetBillPaymentById(billPaymentId);
            if (billPayment == null)
                throw new ArgumentException("Bill payment not found.");

            var account = _accountRepository.GetAccountById(billPayment.AccountId);
            if (account == null)
                throw new ArgumentException("Account not found.");

            var result = _repository.DeleteBillPayment(billPaymentId);
            
            if (result)
            {
                AuditService.LogAction(account.UserId, "BillPaymentDeleted", "BillPayment", billPaymentId, 
                    $"Deleted bill payment: {billPayment.PayeeName}");
            }

            return result;
        }
    }
}

