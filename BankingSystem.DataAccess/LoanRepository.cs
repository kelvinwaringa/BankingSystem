using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class LoanRepository
    {
        public int CreateLoan(Loan loan)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "INSERT INTO Loans (UserId, AccountId, LoanAmount, InterestRate, MonthlyPayment, RemainingBalance, LoanStatus, ApplicationDate, DueDate) " +
                    "VALUES (@UserId, @AccountId, @LoanAmount, @InterestRate, @MonthlyPayment, @RemainingBalance, @LoanStatus, @ApplicationDate, @DueDate); " +
                    "SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@UserId", loan.UserId);
                    command.Parameters.AddWithValue("@AccountId", (object)loan.AccountId ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LoanAmount", loan.LoanAmount);
                    command.Parameters.AddWithValue("@InterestRate", loan.InterestRate);
                    command.Parameters.AddWithValue("@MonthlyPayment", loan.MonthlyPayment);
                    command.Parameters.AddWithValue("@RemainingBalance", loan.RemainingBalance);
                    command.Parameters.AddWithValue("@LoanStatus", loan.LoanStatus);
                    command.Parameters.AddWithValue("@ApplicationDate", loan.ApplicationDate);
                    command.Parameters.AddWithValue("@DueDate", (object)loan.DueDate ?? DBNull.Value);
                    
                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        public List<Loan> GetLoansByUserId(int userId)
        {
            var loans = new List<Loan>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "SELECT * FROM Loans WHERE UserId = @UserId ORDER BY ApplicationDate DESC", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            loans.Add(new Loan
                            {
                                LoanId = (int)reader["LoanId"],
                                UserId = (int)reader["UserId"],
                                AccountId = reader["AccountId"] == DBNull.Value ? (int?)null : (int)reader["AccountId"],
                                LoanAmount = (decimal)reader["LoanAmount"],
                                InterestRate = (decimal)reader["InterestRate"],
                                MonthlyPayment = (decimal)reader["MonthlyPayment"],
                                RemainingBalance = (decimal)reader["RemainingBalance"],
                                LoanStatus = (string)reader["LoanStatus"],
                                ApplicationDate = (DateTime)reader["ApplicationDate"],
                                ApprovalDate = reader["ApprovalDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ApprovalDate"],
                                DueDate = reader["DueDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DueDate"]
                            });
                        }
                    }
                }
            }
            return loans;
        }

        public Loan GetLoanById(int loanId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Loans WHERE LoanId = @LoanId", connection))
                {
                    command.Parameters.AddWithValue("@LoanId", loanId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Loan
                            {
                                LoanId = (int)reader["LoanId"],
                                UserId = (int)reader["UserId"],
                                AccountId = reader["AccountId"] == DBNull.Value ? (int?)null : (int)reader["AccountId"],
                                LoanAmount = (decimal)reader["LoanAmount"],
                                InterestRate = (decimal)reader["InterestRate"],
                                MonthlyPayment = (decimal)reader["MonthlyPayment"],
                                RemainingBalance = (decimal)reader["RemainingBalance"],
                                LoanStatus = (string)reader["LoanStatus"],
                                ApplicationDate = (DateTime)reader["ApplicationDate"],
                                ApprovalDate = reader["ApprovalDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ApprovalDate"],
                                DueDate = reader["DueDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DueDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateLoan(Loan loan)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE Loans SET LoanStatus = @LoanStatus, RemainingBalance = @RemainingBalance, " +
                    "ApprovalDate = @ApprovalDate, DueDate = @DueDate WHERE LoanId = @LoanId", connection))
                {
                    command.Parameters.AddWithValue("@LoanId", loan.LoanId);
                    command.Parameters.AddWithValue("@LoanStatus", loan.LoanStatus);
                    command.Parameters.AddWithValue("@RemainingBalance", loan.RemainingBalance);
                    command.Parameters.AddWithValue("@ApprovalDate", (object)loan.ApprovalDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DueDate", (object)loan.DueDate ?? DBNull.Value);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ProcessLoanPayment(int loanId, decimal paymentAmount)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get loan details
                        var loan = GetLoanById(loanId);
                        if (loan == null || loan.RemainingBalance <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        // Update remaining balance
                        var newBalance = loan.RemainingBalance - paymentAmount;
                        if (newBalance < 0) newBalance = 0;

                        using (var command = new SqlCommand(
                            "UPDATE Loans SET RemainingBalance = @RemainingBalance, " +
                            "LoanStatus = CASE WHEN @RemainingBalance <= 0 THEN 'Paid' ELSE LoanStatus END " +
                            "WHERE LoanId = @LoanId", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@LoanId", loanId);
                            command.Parameters.AddWithValue("@RemainingBalance", newBalance);
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Loan> GetAllLoans()
        {
            var loans = new List<Loan>();
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Loans ORDER BY ApplicationDate DESC", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            loans.Add(new Loan
                            {
                                LoanId = (int)reader["LoanId"],
                                UserId = (int)reader["UserId"],
                                AccountId = reader["AccountId"] == DBNull.Value ? (int?)null : (int)reader["AccountId"],
                                LoanAmount = (decimal)reader["LoanAmount"],
                                InterestRate = (decimal)reader["InterestRate"],
                                MonthlyPayment = (decimal)reader["MonthlyPayment"],
                                RemainingBalance = (decimal)reader["RemainingBalance"],
                                LoanStatus = (string)reader["LoanStatus"],
                                ApplicationDate = (DateTime)reader["ApplicationDate"],
                                ApprovalDate = reader["ApprovalDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ApprovalDate"],
                                DueDate = reader["DueDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DueDate"]
                            });
                        }
                    }
                }
            }
            return loans;
        }
    }
}

