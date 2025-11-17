using System;

namespace BankingSystem.Models
{
    public class SavingsGoal
    {
        public int SavingsGoalId { get; set; }
        public int UserId { get; set; }
        public int? AccountId { get; set; }
        public string GoalName { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal ProgressPercentage { get; set; } // Calculated field
        public int DaysRemaining { get; set; } // Calculated field
    }
}

