using System;

namespace BankingSystem.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public int UserId { get; set; }
        public string Category { get; set; }
        public decimal BudgetAmount { get; set; }
        public string Period { get; set; } // Monthly, Weekly, Yearly
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal SpentAmount { get; set; } // Calculated field
        public decimal RemainingAmount { get; set; } // Calculated field
    }
}

