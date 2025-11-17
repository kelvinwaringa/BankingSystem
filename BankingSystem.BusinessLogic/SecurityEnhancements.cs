using System;
using System.Text.RegularExpressions;

namespace BankingSystem.BusinessLogic
{
    public class SecurityEnhancements
    {
        public static PasswordValidationResult ValidatePasswordStrength(string password)
        {
            var result = new PasswordValidationResult { IsValid = true, Errors = new System.Collections.Generic.List<string>() };

            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password cannot be empty");
                return result;
            }

            if (password.Length < 8)
            {
                result.IsValid = false;
                result.Errors.Add("Password must be at least 8 characters long");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one uppercase letter");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one lowercase letter");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one number");
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?\"":{}|<>]"))
            {
                result.IsValid = false;
                result.Errors.Add("Password must contain at least one special character");
            }

            return result;
        }

        public static bool IsValidAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                return false;
            
            return Regex.IsMatch(accountNumber, @"^ACC\d{17}$");
        }

        public static bool IsValidAmount(decimal amount)
        {
            return amount > 0 && amount <= 1000000; // Max transaction limit
        }
    }

    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public System.Collections.Generic.List<string> Errors { get; set; }
    }
}

