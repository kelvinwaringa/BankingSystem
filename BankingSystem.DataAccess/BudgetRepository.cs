using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class BudgetRepository
    {
        public int CreateBudget(Budget budget)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO Budgets (UserId, Category, BudgetAmount, Period, StartDate, EndDate, IsActive, CreatedDate) " +
                    "VALUES (@UserId, @Category, @BudgetAmount, @Period, @StartDate, @EndDate, @IsActive, GETDATE()); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                {
                    command.Parameters.AddWithValue("@UserId", budget.UserId);
                    command.Parameters.AddWithValue("@Category", budget.Category);
                    command.Parameters.AddWithValue("@BudgetAmount", budget.BudgetAmount);
                    command.Parameters.AddWithValue("@Period", budget.Period);
                    command.Parameters.AddWithValue("@StartDate", budget.StartDate);
                    command.Parameters.AddWithValue("@EndDate", budget.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", budget.IsActive);
                    
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public List<Budget> GetBudgetsByUserId(int userId)
        {
            var budgets = new List<Budget>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM Budgets WHERE UserId = @UserId ORDER BY CreatedDate DESC", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            budgets.Add(new Budget
                            {
                                BudgetId = (int)reader["BudgetId"],
                                UserId = (int)reader["UserId"],
                                Category = (string)reader["Category"],
                                BudgetAmount = (decimal)reader["BudgetAmount"],
                                Period = (string)reader["Period"],
                                StartDate = (DateTime)reader["StartDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return budgets;
        }

        public Budget GetBudgetById(int budgetId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM Budgets WHERE BudgetId = @BudgetId", connection))
                {
                    command.Parameters.AddWithValue("@BudgetId", budgetId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Budget
                            {
                                BudgetId = (int)reader["BudgetId"],
                                UserId = (int)reader["UserId"],
                                Category = (string)reader["Category"],
                                BudgetAmount = (decimal)reader["BudgetAmount"],
                                Period = (string)reader["Period"],
                                StartDate = (DateTime)reader["StartDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateBudget(Budget budget)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE Budgets SET " +
                    "Category = @Category, " +
                    "BudgetAmount = @BudgetAmount, " +
                    "Period = @Period, " +
                    "StartDate = @StartDate, " +
                    "EndDate = @EndDate, " +
                    "IsActive = @IsActive " +
                    "WHERE BudgetId = @BudgetId", connection))
                {
                    command.Parameters.AddWithValue("@BudgetId", budget.BudgetId);
                    command.Parameters.AddWithValue("@Category", budget.Category);
                    command.Parameters.AddWithValue("@BudgetAmount", budget.BudgetAmount);
                    command.Parameters.AddWithValue("@Period", budget.Period);
                    command.Parameters.AddWithValue("@StartDate", budget.StartDate);
                    command.Parameters.AddWithValue("@EndDate", budget.EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", budget.IsActive);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteBudget(int budgetId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "DELETE FROM Budgets WHERE BudgetId = @BudgetId", connection))
                {
                    command.Parameters.AddWithValue("@BudgetId", budgetId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Budget> GetActiveBudgetsByUserId(int userId)
        {
            var budgets = new List<Budget>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM Budgets WHERE UserId = @UserId AND IsActive = 1 " +
                    "AND (EndDate IS NULL OR EndDate >= GETDATE()) " +
                    "ORDER BY Category", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            budgets.Add(new Budget
                            {
                                BudgetId = (int)reader["BudgetId"],
                                UserId = (int)reader["UserId"],
                                Category = (string)reader["Category"],
                                BudgetAmount = (decimal)reader["BudgetAmount"],
                                Period = (string)reader["Period"],
                                StartDate = (DateTime)reader["StartDate"],
                                EndDate = reader["EndDate"] == DBNull.Value ? null : (DateTime?)reader["EndDate"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return budgets;
        }
    }
}

