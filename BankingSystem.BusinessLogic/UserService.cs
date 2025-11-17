using System;
using BankingSystem.DataAccess;
using BankingSystem.Models;

namespace BankingSystem.BusinessLogic
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public int RegisterUser(string username, string email, string password, string firstName, 
            string lastName, string phoneNumber = null, string address = null, DateTime? dateOfBirth = null)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");
            if (!SecurityService.IsValidEmail(email))
                throw new ArgumentException("Invalid email format.");
            // Enhanced password validation
            var passwordValidation = SecurityEnhancements.ValidatePasswordStrength(password);
            if (!passwordValidation.IsValid)
            {
                throw new ArgumentException("Password does not meet requirements:\n" + 
                    string.Join("\n", passwordValidation.Errors));
            }
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.");

            // Check if username already exists
            var existingUser = _userRepository.GetUserByUsername(username);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists.");

            // Create new user
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = SecurityService.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = address,
                DateOfBirth = dateOfBirth,
                Role = "Customer",
                IsActive = true
            };

            return _userRepository.CreateUser(user);
        }

        public User Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Username and password are required.");

            var user = _userRepository.GetUserByUsername(username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is inactive.");

            if (!SecurityService.VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid username or password.");

            return user;
        }

        public User GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }

        public bool UpdateUserProfile(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return _userRepository.UpdateUser(user);
        }
    }
}

