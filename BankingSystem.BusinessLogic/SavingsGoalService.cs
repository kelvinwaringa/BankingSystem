using System;
using System.Collections.Generic;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class SavingsGoalService
    {
        private readonly SavingsGoalRepository _repository;
        private readonly AccountRepository _accountRepository;

        public SavingsGoalService()
        {
            _repository = new SavingsGoalRepository();
            _accountRepository = new AccountRepository();
        }

        public SavingsGoal CreateSavingsGoal(int userId, int? accountId, string goalName, decimal targetAmount, 
            DateTime? targetDate, string description)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");
            if (string.IsNullOrWhiteSpace(goalName))
                throw new ArgumentException("Goal name is required.");
            if (targetAmount <= 0)
                throw new ArgumentException("Target amount must be greater than zero.");

            if (accountId.HasValue)
            {
                var account = _accountRepository.GetAccountById(accountId.Value);
                if (account == null || account.UserId != userId)
                    throw new ArgumentException("Invalid account for this user.");
            }

            var goal = new SavingsGoal
            {
                UserId = userId,
                AccountId = accountId,
                GoalName = goalName,
                TargetAmount = targetAmount,
                CurrentAmount = 0,
                TargetDate = targetDate,
                Description = description,
                IsCompleted = false,
                CreatedDate = DateTime.Now
            };

            var goalId = _repository.CreateSavingsGoal(goal);
            goal.SavingsGoalId = goalId;
            
            AuditService.LogAction(userId, "SavingsGoalCreated", "SavingsGoal", goalId, 
                $"Created savings goal: {goalName}, Target: {targetAmount:C}");

            return goal;
        }

        public List<SavingsGoal> GetSavingsGoalsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetSavingsGoalsByUserId(userId);
        }

        public List<SavingsGoal> GetActiveSavingsGoalsByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            return _repository.GetActiveSavingsGoalsByUserId(userId);
        }

        public SavingsGoal GetSavingsGoalById(int savingsGoalId)
        {
            if (savingsGoalId <= 0)
                throw new ArgumentException("Invalid savings goal ID.");

            return _repository.GetSavingsGoalById(savingsGoalId);
        }

        public bool UpdateSavingsGoal(SavingsGoal goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));
            if (goal.SavingsGoalId <= 0)
                throw new ArgumentException("Invalid savings goal ID.");

            var existingGoal = _repository.GetSavingsGoalById(goal.SavingsGoalId);
            if (existingGoal == null)
                throw new ArgumentException("Savings goal not found.");

            // Check if goal is completed
            if (goal.CurrentAmount >= goal.TargetAmount && !goal.IsCompleted)
            {
                goal.IsCompleted = true;
            }

            var result = _repository.UpdateSavingsGoal(goal);
            
            if (result)
            {
                AuditService.LogAction(goal.UserId, "SavingsGoalUpdated", "SavingsGoal", goal.SavingsGoalId, 
                    $"Updated savings goal: {goal.GoalName}");
            }

            return result;
        }

        public bool DeleteSavingsGoal(int savingsGoalId)
        {
            if (savingsGoalId <= 0)
                throw new ArgumentException("Invalid savings goal ID.");

            var goal = _repository.GetSavingsGoalById(savingsGoalId);
            if (goal == null)
                throw new ArgumentException("Savings goal not found.");

            var result = _repository.DeleteSavingsGoal(savingsGoalId);
            
            if (result)
            {
                AuditService.LogAction(goal.UserId, "SavingsGoalDeleted", "SavingsGoal", savingsGoalId, 
                    $"Deleted savings goal: {goal.GoalName}");
            }

            return result;
        }

        public bool AddToSavingsGoal(int savingsGoalId, decimal amount)
        {
            if (savingsGoalId <= 0)
                throw new ArgumentException("Invalid savings goal ID.");
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var goal = _repository.GetSavingsGoalById(savingsGoalId);
            if (goal == null)
                throw new ArgumentException("Savings goal not found.");

            if (goal.IsCompleted)
                throw new InvalidOperationException("Cannot add to a completed savings goal.");

            goal.CurrentAmount += amount;

            // Check if goal is now completed
            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                goal.IsCompleted = true;
            }

            return UpdateSavingsGoal(goal);
        }

        public bool WithdrawFromSavingsGoal(int savingsGoalId, decimal amount)
        {
            if (savingsGoalId <= 0)
                throw new ArgumentException("Invalid savings goal ID.");
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var goal = _repository.GetSavingsGoalById(savingsGoalId);
            if (goal == null)
                throw new ArgumentException("Savings goal not found.");

            if (goal.CurrentAmount < amount)
                throw new InvalidOperationException("Insufficient amount in savings goal.");

            goal.CurrentAmount -= amount;
            goal.IsCompleted = false; // Reset completion status if withdrawing

            return UpdateSavingsGoal(goal);
        }

        public decimal GetProgressPercentage(SavingsGoal goal)
        {
            if (goal == null || goal.TargetAmount <= 0)
                return 0;

            return Math.Min(100, (goal.CurrentAmount / goal.TargetAmount) * 100);
        }

        public int GetDaysRemaining(SavingsGoal goal)
        {
            if (goal == null || !goal.TargetDate.HasValue)
                return -1; // No target date

            var days = (goal.TargetDate.Value - DateTime.Now).Days;
            return Math.Max(0, days);
        }

        public decimal GetRequiredMonthlySavings(SavingsGoal goal)
        {
            if (goal == null || !goal.TargetDate.HasValue)
                return 0;

            var daysRemaining = GetDaysRemaining(goal);
            if (daysRemaining <= 0)
                return 0;

            var monthsRemaining = daysRemaining / 30.0;
            var remainingAmount = goal.TargetAmount - goal.CurrentAmount;

            if (monthsRemaining <= 0)
                return remainingAmount;

            return remainingAmount / (decimal)monthsRemaining;
        }
    }
}

