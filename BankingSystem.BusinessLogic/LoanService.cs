using System;
using System.Collections.Generic;
using System.Linq;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class LoanService
    {
        private readonly LoanRepository _loanRepository;
        private readonly AccountRepository _accountRepository;
        private readonly UserRepository _userRepository;

        public LoanService()
        {
            _loanRepository = new LoanRepository();
            _accountRepository = new AccountRepository();
            _userRepository = new UserRepository();
        }

        public LoanEligibilityResult CheckLoanEligibility(int userId, decimal requestedAmount, int loanTermMonths)
        {
            var result = new LoanEligibilityResult
            {
                IsEligible = false,
                Reasons = new List<string>(),
                RecommendedAmount = 0,
                InterestRate = 5.0m // Base interest rate
            };

            try
            {
                // Get user accounts
                var accounts = _accountRepository.GetAccountsByUserId(userId);
                if (accounts == null || accounts.Count == 0)
                {
                    result.Reasons.Add("No active accounts found. You need at least one account to apply for a loan.");
                    return result;
                }

                // Calculate total balance
                var totalBalance = accounts.Sum(a => a.Balance);

                // Check existing loans
                var existingLoans = _loanRepository.GetLoansByUserId(userId);
                var activeLoans = existingLoans.Where(l => l.LoanStatus == "Active" || l.LoanStatus == "Approved").ToList();
                var totalOutstanding = activeLoans.Sum(l => l.RemainingBalance);

                // AI-based eligibility rules
                // Rule 1: Minimum balance requirement (10% of requested amount)
                var minBalanceRequired = requestedAmount * 0.10m;
                if (totalBalance < minBalanceRequired)
                {
                    result.Reasons.Add($"Insufficient account balance. Minimum required: ${minBalanceRequired:F2}");
                }

                // Rule 2: Debt-to-income ratio (simplified - using balance as income proxy)
                var debtToBalanceRatio = totalOutstanding / (totalBalance > 0 ? totalBalance : 1);
                if (debtToBalanceRatio > 0.5m)
                {
                    result.Reasons.Add("High existing debt. Your debt-to-balance ratio exceeds 50%.");
                }

                // Rule 3: Maximum loan amount (5x current balance)
                var maxLoanAmount = totalBalance * 5;
                if (requestedAmount > maxLoanAmount)
                {
                    result.Reasons.Add($"Requested amount exceeds maximum allowed (${maxLoanAmount:F2})");
                    result.RecommendedAmount = maxLoanAmount;
                }

                // Rule 4: Minimum loan amount
                if (requestedAmount < 100)
                {
                    result.Reasons.Add("Minimum loan amount is $100");
                }

                // Rule 5: Loan term validation
                if (loanTermMonths < 6 || loanTermMonths > 60)
                {
                    result.Reasons.Add("Loan term must be between 6 and 60 months");
                }

                // Calculate interest rate based on risk factors
                if (totalBalance >= requestedAmount * 0.5m)
                {
                    result.InterestRate = 4.5m; // Lower risk
                }
                else if (totalBalance >= requestedAmount * 0.25m)
                {
                    result.InterestRate = 5.5m; // Medium risk
                }
                else
                {
                    result.InterestRate = 6.5m; // Higher risk
                }

                // If no reasons, eligible
                if (result.Reasons.Count == 0)
                {
                    result.IsEligible = true;
                    result.RecommendedAmount = requestedAmount;
                }
                else if (result.RecommendedAmount == 0)
                {
                    result.RecommendedAmount = Math.Min(requestedAmount, maxLoanAmount);
                }
            }
            catch (Exception ex)
            {
                result.Reasons.Add($"Error checking eligibility: {ex.Message}");
            }

            return result;
        }

        public Loan ApplyForLoan(int userId, decimal loanAmount, int loanTermMonths, int? accountId = null)
        {
            if (loanAmount <= 0)
                throw new ArgumentException("Loan amount must be greater than zero.");
            if (loanTermMonths < 6 || loanTermMonths > 60)
                throw new ArgumentException("Loan term must be between 6 and 60 months.");

            // Check eligibility
            var eligibility = CheckLoanEligibility(userId, loanAmount, loanTermMonths);
            if (!eligibility.IsEligible)
            {
                throw new InvalidOperationException($"Loan application not eligible. Reasons: {string.Join(", ", eligibility.Reasons)}");
            }

            // Calculate monthly payment using amortization formula
            var annualInterestRate = eligibility.InterestRate / 100;
            var monthlyInterestRate = annualInterestRate / 12;
            var powFactor = (double)Math.Pow(1 + (double)monthlyInterestRate, loanTermMonths);
            var monthlyPayment = loanAmount * (monthlyInterestRate * (decimal)powFactor) / ((decimal)powFactor - 1);

            var loan = new Loan
            {
                UserId = userId,
                AccountId = accountId,
                LoanAmount = loanAmount,
                InterestRate = eligibility.InterestRate,
                MonthlyPayment = monthlyPayment,
                RemainingBalance = loanAmount,
                LoanStatus = "Pending",
                ApplicationDate = DateTime.Now,
                DueDate = DateTime.Now.AddMonths(loanTermMonths)
            };

            var loanId = _loanRepository.CreateLoan(loan);
            loan.LoanId = loanId;

            return loan;
        }

        public List<Loan> GetUserLoans(int userId)
        {
            return _loanRepository.GetLoansByUserId(userId);
        }

        public Loan GetLoanById(int loanId)
        {
            return _loanRepository.GetLoanById(loanId);
        }

        public bool ApproveLoan(int loanId)
        {
            var loan = _loanRepository.GetLoanById(loanId);
            if (loan == null)
                return false;

            // If loan has a linked account, deposit the loan amount
            if (loan.AccountId.HasValue)
            {
                try
                {
                    var transactionService = new TransactionService();
                    transactionService.ProcessDeposit(
                        loan.AccountId.Value, 
                        loan.LoanAmount, 
                        $"Loan Disbursement - Loan ID: {loanId}"
                    );
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to disburse loan to account: {ex.Message}", ex);
                }
            }

            loan.LoanStatus = "Active";
            loan.ApprovalDate = DateTime.Now;

            return _loanRepository.UpdateLoan(loan);
        }

        public bool RejectLoan(int loanId)
        {
            var loan = _loanRepository.GetLoanById(loanId);
            if (loan == null)
                return false;

            loan.LoanStatus = "Rejected";
            return _loanRepository.UpdateLoan(loan);
        }

        public bool ProcessLoanPayment(int loanId, int accountId, decimal paymentAmount)
        {
            if (paymentAmount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero.");

            var account = _accountRepository.GetAccountById(accountId);
            if (account == null)
                throw new InvalidOperationException("Account not found.");

            if (account.Balance < paymentAmount)
                throw new InvalidOperationException("Insufficient funds for loan payment.");

            // Process payment from account
            var transactionService = new TransactionService();
            transactionService.ProcessWithdrawal(accountId, paymentAmount, "Loan Payment");

            // Update loan balance
            return _loanRepository.ProcessLoanPayment(loanId, paymentAmount);
        }

        public List<Loan> GetAllLoans()
        {
            return _loanRepository.GetAllLoans();
        }
    }

    public class LoanEligibilityResult
    {
        public bool IsEligible { get; set; }
        public List<string> Reasons { get; set; }
        public decimal RecommendedAmount { get; set; }
        public decimal InterestRate { get; set; }
    }
}

