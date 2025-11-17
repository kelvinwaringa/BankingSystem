using System;
using System.Data;
using System.Data.SqlClient;
using BankingSystem.Models;

namespace BankingSystem.DataAccess
{
    public class UserRepository
    {
        public int CreateUser(User user)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_CreateUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", (object)user.PhoneNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)user.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", (object)user.DateOfBirth ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Role", user.Role ?? "Customer");
                    
                    var userIdParam = new SqlParameter("@UserId", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(userIdParam);
                    
                    command.ExecuteNonQuery();
                    return (int)userIdParam.Value;
                }
            }
        }

        public User GetUserByUsername(string username)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("sp_GetUserByUsername", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Username", username);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = (int)reader["UserId"],
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                PasswordHash = (string)reader["PasswordHash"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : (string)reader["PhoneNumber"],
                                Address = reader["Address"] == DBNull.Value ? null : (string)reader["Address"],
                                DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DateOfBirth"],
                                Role = (string)reader["Role"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public User GetUserById(int userId)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM Users WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = (int)reader["UserId"],
                                Username = (string)reader["Username"],
                                Email = (string)reader["Email"],
                                PasswordHash = (string)reader["PasswordHash"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : (string)reader["PhoneNumber"],
                                Address = reader["Address"] == DBNull.Value ? null : (string)reader["Address"],
                                DateOfBirth = reader["DateOfBirth"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["DateOfBirth"],
                                Role = (string)reader["Role"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedDate = (DateTime)reader["CreatedDate"],
                                LastModifiedDate = (DateTime)reader["LastModifiedDate"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateUser(User user)
        {
            using (var connection = DatabaseConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(
                    "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, " +
                    "PhoneNumber = @PhoneNumber, Address = @Address, DateOfBirth = @DateOfBirth, " +
                    "LastModifiedDate = GETDATE() WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", (object)user.PhoneNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Address", (object)user.Address ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", (object)user.DateOfBirth ?? DBNull.Value);
                    
                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}

