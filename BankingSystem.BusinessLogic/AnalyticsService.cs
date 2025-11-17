using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class AnalyticsService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly AccountRepository _accountRepository;

        public AnalyticsService()
        {
            _transactionRepository = new TransactionRepository();
            _accountRepository = new AccountRepository();
        }

        public SpendingAnalysis GetSpendingAnalysis(int userId, DateTime startDate, DateTime endDate)
        {
            var analysis = new SpendingAnalysis
            {
                TotalIncome = 0,
                TotalExpenses = 0,
                CategoryBreakdown = new Dictionary<string, decimal>(),
                MonthlyTrend = new Dictionary<string, decimal>()
            };

            try
            {
                var accounts = _accountRepository.GetAccountsByUserId(userId);
                var allTransactions = new List<Transaction>();

                foreach (var account in accounts)
                {
                    var transactions = _transactionRepository.GetTransactionHistory(
                        account.AccountId, startDate, endDate);
                    allTransactions.AddRange(transactions);
                }

                foreach (var transaction in allTransactions)
                {
                    var amount = transaction.Amount;
                    var type = transaction.TransactionType.TypeName;

                    if (type == "Deposit" || type == "Transfer" && transaction.RelatedAccountId == null)
                    {
                        analysis.TotalIncome += amount;
                    }
                    else if (type == "Withdrawal" || (type == "Transfer" && transaction.RelatedAccountId != null))
                    {
                        analysis.TotalExpenses += amount;
                        
                        // Category breakdown
                        var category = GetTransactionCategory(transaction);
                        if (!analysis.CategoryBreakdown.ContainsKey(category))
                            analysis.CategoryBreakdown[category] = 0;
                        analysis.CategoryBreakdown[category] += amount;
                    }

                    // Monthly trend
                    var monthKey = transaction.TransactionDate.ToString("yyyy-MM");
                    if (!analysis.MonthlyTrend.ContainsKey(monthKey))
                        analysis.MonthlyTrend[monthKey] = 0;
                    
                    if (type == "Withdrawal" || (type == "Transfer" && transaction.RelatedAccountId != null))
                        analysis.MonthlyTrend[monthKey] += amount;
                }

                analysis.NetSavings = analysis.TotalIncome - analysis.TotalExpenses;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating spending analysis: {ex.Message}", ex);
            }

            return analysis;
        }

        private string GetTransactionCategory(Transaction transaction)
        {
            // Try to get category from description or use default
            var description = transaction.Description?.ToLower() ?? "";
            
            if (description.Contains("food") || description.Contains("restaurant") || description.Contains("grocery"))
                return "Food & Dining";
            if (description.Contains("gas") || description.Contains("transport") || description.Contains("parking"))
                return "Transportation";
            if (description.Contains("shopping") || description.Contains("store") || description.Contains("purchase"))
                return "Shopping";
            if (description.Contains("bill") || description.Contains("utility") || description.Contains("electric"))
                return "Bills & Utilities";
            if (description.Contains("entertainment") || description.Contains("movie") || description.Contains("subscription"))
                return "Entertainment";
            if (description.Contains("medical") || description.Contains("health") || description.Contains("pharmacy"))
                return "Healthcare";
            if (description.Contains("education") || description.Contains("tuition") || description.Contains("course"))
                return "Education";
            if (description.Contains("travel") || description.Contains("hotel") || description.Contains("flight"))
                return "Travel";
            if (transaction.TransactionType.TypeName == "Transfer")
                return "Transfer";
            if (transaction.TransactionType.TypeName == "Deposit")
                return "Income";
            
            return "Other";
        }

        public Dictionary<string, decimal> GetCategorySpending(int userId, DateTime startDate, DateTime endDate)
        {
            var analysis = GetSpendingAnalysis(userId, startDate, endDate);
            return analysis.CategoryBreakdown;
        }

        public decimal GetAverageMonthlySpending(int userId, int months = 6)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-months);
            var analysis = GetSpendingAnalysis(userId, startDate, endDate);
            return analysis.MonthlyTrend.Count > 0 
                ? analysis.MonthlyTrend.Values.Average() 
                : 0;
        }
    }

    public class SpendingAnalysis
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetSavings { get; set; }
        public Dictionary<string, decimal> CategoryBreakdown { get; set; }
        public Dictionary<string, decimal> MonthlyTrend { get; set; }
    }
}

