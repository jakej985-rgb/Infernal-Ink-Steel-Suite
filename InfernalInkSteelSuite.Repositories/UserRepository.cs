using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool CreateTable()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        username TEXT UNIQUE NOT NULL,
                        passwordHash TEXT NOT NULL,
                        role TEXT NOT NULL,
                        avatarPath TEXT DEFAULT '',
                        createdAt TEXT DEFAULT CURRENT_TIMESTAMP,
                        updatedAt TEXT DEFAULT CURRENT_TIMESTAMP
                    )";
                command.ExecuteNonQuery();
            }
            return true;
        }

        public string HashPassword(string plain)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plain));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string? GetUsernameById(int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT username FROM users WHERE id = @id";
                command.Parameters.AddWithValue("@id", userId);

                var result = command.ExecuteScalar();
                return result?.ToString();
            }
        }

        public bool AddUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO users (username, passwordHash, role, avatarPath, createdAt, updatedAt)
                    VALUES (@username, @passwordHash, @role, @avatarPath, @createdAt, @updatedAt)";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordHash", HashPassword(string.IsNullOrEmpty(user.PasswordHash) ? "password" : user.PasswordHash));
                command.Parameters.AddWithValue("@role", string.IsNullOrEmpty(user.Role) ? "User" : user.Role);
                command.Parameters.AddWithValue("@avatarPath", user.AvatarPath);
                command.Parameters.AddWithValue("@createdAt", (user.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : user.CreatedAt).ToString("o"));
                command.Parameters.AddWithValue("@updatedAt", (user.UpdatedAt == DateTime.MinValue ? DateTime.UtcNow : user.UpdatedAt).ToString("o"));

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool AddUser(string username, string password, string role)
        {
            User u = new User
            {
                Username = username,
                PasswordHash = password,
                Role = role
            };
            return AddUser(u);
        }

        public bool UpdateUser(User user)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    UPDATE users
                    SET username = @username,
                        role = @role,
                        avatarPath = @avatarPath,
                        updatedAt = @updatedAt
                    WHERE id = @id";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@avatarPath", user.AvatarPath);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
                command.Parameters.AddWithValue("@id", user.Id);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public User? GetUserByUsername(string username)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Role = reader.GetString(3),
                            AvatarPath = reader.GetString(4),
                            CreatedAt = reader.IsDBNull(5) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                            UpdatedAt = reader.IsDBNull(6) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(6), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                        };
                    }
                }
            }
            return null;
        }

        public bool CheckPassword(string username, string plainPassword)
        {
            User? u = GetUserByUsername(username);
            if (u == null) return false;
            return u.PasswordHash == HashPassword(plainPassword);
        }

        public bool DeleteUser(string username)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM users WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateRole(string username, string role)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET role = @role, updatedAt = @updatedAt WHERE username = @username";
                command.Parameters.AddWithValue("@role", role);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
                command.Parameters.AddWithValue("@username", username);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdatePassword(string username, string password)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET passwordHash = @passwordHash, updatedAt = @updatedAt WHERE username = @username";
                command.Parameters.AddWithValue("@passwordHash", HashPassword(password));
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
                command.Parameters.AddWithValue("@username", username);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Role = reader.GetString(3),
                            AvatarPath = reader.GetString(4),
                            CreatedAt = reader.IsDBNull(5) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                            UpdatedAt = reader.IsDBNull(6) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(6), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                        });
                    }
                }
            }
            return users;
        }

        public User? GetUserById(int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM users WHERE id = @id";
                command.Parameters.AddWithValue("@id", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = reader.GetString(2),
                            Role = reader.GetString(3),
                            AvatarPath = reader.GetString(4),
                            CreatedAt = reader.IsDBNull(5) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                            UpdatedAt = reader.IsDBNull(6) ? DateTime.UtcNow : DateTime.Parse(reader.GetString(6), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                        };
                    }
                }
            }
            return null;
        }

        public bool DeleteUser(int userId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM users WHERE id = @id";
                command.Parameters.AddWithValue("@id", userId);

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
