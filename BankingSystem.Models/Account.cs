using System;
using System.Collections.Generic;

namespace BankingSystem.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public int AccountTypeId { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        
        // Navigation properties
        public User User { get; set; }
        public AccountType AccountType { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}

