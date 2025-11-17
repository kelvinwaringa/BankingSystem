using System;

namespace BankingSystem.Models
{
    public class RecurringPayment
    {
        public int RecurringPaymentId { get; set; }
        public int AccountId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Frequency { get; set; } // Daily, Weekly, Monthly, Yearly
        public DateTime NextPaymentDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

