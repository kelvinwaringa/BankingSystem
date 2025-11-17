using System;

namespace BankingSystem.Models
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; } // Login, Logout, Transaction, AccountCreated, etc.
        public string EntityType { get; set; } // User, Account, Transaction, Loan, etc.
        public int? EntityId { get; set; }
        public string Details { get; set; }
        public string IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

