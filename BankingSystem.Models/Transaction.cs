using System;

namespace BankingSystem.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public int TransactionTypeId { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public string Description { get; set; }
        public int? RelatedAccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        
        // Navigation properties
        public Account Account { get; set; }
        public TransactionType TransactionType { get; set; }
        public Account RelatedAccount { get; set; }
    }
}

