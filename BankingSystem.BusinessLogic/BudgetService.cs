using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class BudgetService
    {
        private readonly BudgetRepository _repository;
        private readonly TransactionRepository _transactionRepository;

        public BudgetService()
        {
            _repository = new BudgetRepository();
            _transactionRepository = new TransactionRepository();
        }

        public Budget CreateBudget(int userId, string category, decimal budgetAmount, string period, 
            DateTime startDate, DateTime? endDate)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required.");
            if (budgetAmount <= 0)
                throw new ArgumentException("Budget amount must be greater than zero.");
            if (string.IsNullOrWhiteSpace(period))
                throw new ArgumentException("Period is required.");
            if (!IsValidPeriod(period))
                throw new ArgumentException("Invalid period. Must be Monthly, Weekly, or Yearly.");

            var budget = new Budget
            {
                UserId = userId,
                Category = category,
                BudgetAmount = budgetAmount,
                Period = period,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var budgetId = _repository.CreateBudget(budget);
            budget.BudgetId = budgetId;
            
            AuditService.LogAction(userId, "BudgetCreated", "Budget", budgetId, 
                $"Created budget: {category}, Amount: {budgetAmount:C}, Period: {period}");

            return budget;
        }

        public List<Budget> GetBudgetsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetBudgetsByUserId(userId);
        }

        public List<Budget> GetActiveBudgetsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetActiveBudgetsByUserId(userId);
        }

        public Budget GetBudgetById(int budgetId)
        {
            if (budgetId <= 0)
                throw new ArgumentException("Invalid budget ID.");

            return _repository.GetBudgetById(budgetId);
        }

        public bool UpdateBudget(Budget budget)
        {
            if (budget == null)
                throw new ArgumentNullException(nameof(budget));
            if (budget.BudgetId <= 0)
                throw new ArgumentException("Invalid budget ID.");

            var existingBudget = _repository.GetBudgetById(budget.BudgetId);
            if (existingBudget == null)
                throw new ArgumentException("Budget not found.");

            var result = _repository.UpdateBudget(budget);
            
            if (result)
            {
                AuditService.LogAction(budget.UserId, "BudgetUpdated", "Budget", budget.BudgetId, 
                    $"Updated budget: {budget.Category}");
            }

            return result;
        }

        public bool DeleteBudget(int budgetId)
        {
            if (budgetId <= 0)
                throw new ArgumentException("Invalid budget ID.");

            var budget = _repository.GetBudgetById(budgetId);
            if (budget == null)
                throw new ArgumentException("Budget not found.");

            var result = _repository.DeleteBudget(budgetId);
            
            if (result)
            {
                AuditService.LogAction(budget.UserId, "BudgetDeleted", "Budget", budgetId, 
                    $"Deleted budget: {budget.Category}");
            }

            return result;
        }

        public BudgetStatus GetBudgetStatus(int budgetId)
        {
            if (budgetId <= 0)
                throw new ArgumentException("Invalid budget ID.");

            var budget = _repository.GetBudgetById(budgetId);
            if (budget == null)
                throw new ArgumentException("Budget not found.");

            // Get all accounts for the user
            var accountRepository = new AccountRepository();
            var accounts = accountRepository.GetAccountsByUserId(budget.UserId);

            // Calculate spending in this category for the current period
            var periodStart = GetPeriodStartDate(budget.StartDate, budget.Period);
            var periodEnd = GetPeriodEndDate(periodStart, budget.Period);

            decimal totalSpent = 0;
            foreach (var account in accounts)
            {
                var transactions = _transactionRepository.GetTransactionHistory(account.AccountId, periodStart, periodEnd);
                // Filter transactions by category (simplified - would need category mapping)
                // For now, we'll use a simple approach based on transaction descriptions
                totalSpent += transactions
                    .Where(t => t.Amount < 0 && MatchesCategory(t.Description, budget.Category))
                    .Sum(t => Math.Abs(t.Amount));
            }

            return new BudgetStatus
            {
                Budget = budget,
                TotalSpent = totalSpent,
                RemainingAmount = budget.BudgetAmount - totalSpent,
                PercentageUsed = budget.BudgetAmount > 0 ? (totalSpent / budget.BudgetAmount) * 100 : 0,
                IsOverBudget = totalSpent > budget.BudgetAmount
            };
        }

        private bool IsValidPeriod(string period)
        {
            return period.Equals("Monthly", StringComparison.OrdinalIgnoreCase) ||
                   period.Equals("Weekly", StringComparison.OrdinalIgnoreCase) ||
                   period.Equals("Yearly", StringComparison.OrdinalIgnoreCase);
        }

        private DateTime GetPeriodStartDate(DateTime startDate, string period)
        {
            var now = DateTime.Now;
            switch (period.ToLower())
            {
                case "monthly":
                    return new DateTime(now.Year, now.Month, 1);
                case "weekly":
                    var daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    return now.AddDays(-daysUntilMonday).Date;
                case "yearly":
                    return new DateTime(now.Year, 1, 1);
                default:
                    return startDate;
            }
        }

        private DateTime GetPeriodEndDate(DateTime periodStart, string period)
        {
            switch (period.ToLower())
            {
                case "monthly":
                    return periodStart.AddMonths(1).AddDays(-1);
                case "weekly":
                    return periodStart.AddDays(6);
                case "yearly":
                    return periodStart.AddYears(1).AddDays(-1);
                default:
                    return periodStart;
            }
        }

        private bool MatchesCategory(string description, string category)
        {
            if (string.IsNullOrWhiteSpace(description))
                return false;

            var descLower = description.ToLower();
            var catLower = category.ToLower();

            // Simple keyword matching - could be enhanced
            return descLower.Contains(catLower) || 
                   (catLower == "food" && (descLower.Contains("restaurant") || descLower.Contains("grocery") || descLower.Contains("food"))) ||
                   (catLower == "transportation" && (descLower.Contains("gas") || descLower.Contains("uber") || descLower.Contains("taxi"))) ||
                   (catLower == "entertainment" && (descLower.Contains("movie") || descLower.Contains("concert") || descLower.Contains("game")));
        }

        public class BudgetStatus
        {
            public Budget Budget { get; set; }
            public decimal TotalSpent { get; set; }
            public decimal RemainingAmount { get; set; }
            public decimal PercentageUsed { get; set; }
            public bool IsOverBudget { get; set; }
        }
    }
}

