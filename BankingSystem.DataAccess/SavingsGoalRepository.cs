using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class SavingsGoalRepository
    {
        public int CreateSavingsGoal(SavingsGoal goal)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO SavingsGoals (UserId, AccountId, GoalName, TargetAmount, CurrentAmount, TargetDate, Description, IsCompleted, CreatedDate) " +
                    "VALUES (@UserId, @AccountId, @GoalName, @TargetAmount, @CurrentAmount, @TargetDate, @Description, @IsCompleted, GETDATE()); " +
                    "SELECT CAST(SCOPE_IDENTITY() AS INT);", connection))
                {
                    command.Parameters.AddWithValue("@UserId", goal.UserId);
                    command.Parameters.AddWithValue("@AccountId", goal.AccountId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GoalName", goal.GoalName);
                    command.Parameters.AddWithValue("@TargetAmount", goal.TargetAmount);
                    command.Parameters.AddWithValue("@CurrentAmount", goal.CurrentAmount);
                    command.Parameters.AddWithValue("@TargetDate", goal.TargetDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", goal.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsCompleted", goal.IsCompleted);
                    
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public List<SavingsGoal> GetSavingsGoalsByUserId(int userId)
        {
            var goals = new List<SavingsGoal>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM SavingsGoals WHERE UserId = @UserId ORDER BY IsCompleted, TargetDate", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            goals.Add(new SavingsGoal
                            {
                                SavingsGoalId = (int)reader["SavingsGoalId"],
                                UserId = (int)reader["UserId"],
                                AccountId = reader["AccountId"] == DBNull.Value ? null : (int?)reader["AccountId"],
                                GoalName = (string)reader["GoalName"],
                                TargetAmount = (decimal)reader["TargetAmount"],
                                CurrentAmount = (decimal)reader["CurrentAmount"],
                                TargetDate = reader["TargetDate"] == DBNull.Value ? null : (DateTime?)reader["TargetDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsCompleted = (bool)reader["IsCompleted"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return goals;
        }

        public SavingsGoal GetSavingsGoalById(int savingsGoalId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM SavingsGoals WHERE SavingsGoalId = @SavingsGoalId", connection))
                {
                    command.Parameters.AddWithValue("@SavingsGoalId", savingsGoalId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new SavingsGoal
                            {
                                SavingsGoalId = (int)reader["SavingsGoalId"],
                                UserId = (int)reader["UserId"],
                                AccountId = reader["AccountId"] == DBNull.Value ? null : (int?)reader["AccountId"],
                                GoalName = (string)reader["GoalName"],
                                TargetAmount = (decimal)reader["TargetAmount"],
                                CurrentAmount = (decimal)reader["CurrentAmount"],
                                TargetDate = reader["TargetDate"] == DBNull.Value ? null : (DateTime?)reader["TargetDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsCompleted = (bool)reader["IsCompleted"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateSavingsGoal(SavingsGoal goal)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE SavingsGoals SET " +
                    "AccountId = @AccountId, " +
                    "GoalName = @GoalName, " +
                    "TargetAmount = @TargetAmount, " +
                    "CurrentAmount = @CurrentAmount, " +
                    "TargetDate = @TargetDate, " +
                    "Description = @Description, " +
                    "IsCompleted = @IsCompleted " +
                    "WHERE SavingsGoalId = @SavingsGoalId", connection))
                {
                    command.Parameters.AddWithValue("@SavingsGoalId", goal.SavingsGoalId);
                    command.Parameters.AddWithValue("@AccountId", goal.AccountId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GoalName", goal.GoalName);
                    command.Parameters.AddWithValue("@TargetAmount", goal.TargetAmount);
                    command.Parameters.AddWithValue("@CurrentAmount", goal.CurrentAmount);
                    command.Parameters.AddWithValue("@TargetDate", goal.TargetDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Description", goal.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@IsCompleted", goal.IsCompleted);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteSavingsGoal(int savingsGoalId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "DELETE FROM SavingsGoals WHERE SavingsGoalId = @SavingsGoalId", connection))
                {
                    command.Parameters.AddWithValue("@SavingsGoalId", savingsGoalId);
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<SavingsGoal> GetActiveSavingsGoalsByUserId(int userId)
        {
            var goals = new List<SavingsGoal>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM SavingsGoals WHERE UserId = @UserId AND IsCompleted = 0 " +
                    "ORDER BY TargetDate", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            goals.Add(new SavingsGoal
                            {
                                SavingsGoalId = (int)reader["SavingsGoalId"],
                                UserId = (int)reader["UserId"],
                                AccountId = reader["AccountId"] == DBNull.Value ? null : (int?)reader["AccountId"],
                                GoalName = (string)reader["GoalName"],
                                TargetAmount = (decimal)reader["TargetAmount"],
                                CurrentAmount = (decimal)reader["CurrentAmount"],
                                TargetDate = reader["TargetDate"] == DBNull.Value ? null : (DateTime?)reader["TargetDate"],
                                Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
                                IsCompleted = (bool)reader["IsCompleted"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            });
                        }
                    }
                }
            }
            return goals;
        }
    }
}

