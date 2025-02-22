using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epam.EmailManager.Domain.Entities;
using Epam.EmailManager.Infrastructure.Repository;
using Microsoft.Data.SqlClient;

namespace Epam.EmailManager.Application.Services
{
    public class UserService<T> : IUserDetailsRepository<T> where T : User, new()
    {
        private readonly string _connectionString;
        public UserService() { }
        public UserService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int AddUser(T user)
        {
            if (user is User)
            {
                string name = user.GetType().GetProperty("Name").GetValue(user).ToString();
                string email = user.GetType().GetProperty("Email").GetValue(user).ToString();
                try
                {
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        const string sqlQuery = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email); SELECT SCOPE_IDENTITY();";
                        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@Email", email);

                            Console.WriteLine("Opening connection...");
                            connection.Open();
                            Console.WriteLine("Connection opened successfully.");
                            int id = Convert.ToInt32(command.ExecuteScalar());
                            Console.WriteLine("Command executed. Inserted Id: " + id);
                            return id;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Database error: " + ex.Message);
                    return -1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.ToString());
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        public List<T> GetAllUsers()
        {
            List<T> users = new List<T>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    const string sqlQuery = "Select * FROM Users;";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        Console.WriteLine("Opening connection...");
                        connection.Open();
                        Console.WriteLine("Connection opened successfully.");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                T user = new T();
                                Console.WriteLine("Reading data...");
                                user.Name = reader["Name"].ToString();
                                user.Email = reader["Email"].ToString();
                                Console.WriteLine("Data read successfully.");
                                users.Add(user);
                            }
                        }
                        return users;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return null;
            }
        }
        public User GetUserById(int id)
        {
            try
            {
                Console.WriteLine($"Inside GetUserById, ID: {id}");
                Console.WriteLine(_connectionString);
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    const string sqlQuery = "SELECT * FROM Users WHERE id = @id;";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        Console.WriteLine("Opening connection...");
                        connection.Open();
                        Console.WriteLine("Connection opened successfully.");

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine("Reading data...");
                                User user = new User
                                {
                                    Name = reader["Name"]?.ToString(),  // Handle potential null values safely
                                    Email = reader["Email"]?.ToString()
                                };
                                Console.WriteLine("Data read successfully.");
                                return user;
                            }
                        }
                    }
                }
                Console.WriteLine("No user found.");
                return null;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex);
                return null;
            }
        }
    }
}
