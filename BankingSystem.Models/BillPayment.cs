using System;

namespace BankingSystem.Models
{
    public class BillPayment
    {
        public int BillPaymentId { get; set; }
        public int AccountId { get; set; }
        public string PayeeName { get; set; }
        public string PayeeAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Status { get; set; } // Pending, Paid, Overdue, Cancelled
        public string Description { get; set; }
        public bool IsRecurring { get; set; }
        public int? RecurringPaymentId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

