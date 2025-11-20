using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Repositories
{
    public class UserRepository(string connectionString) : IUserRepository
    {
        private const string UserColumns = "id, username, passwordHash, role, ThemeKey, avatarPath, createdAt, updatedAt, HourlyRate, SpeedFactor";
        private readonly string _connectionString = connectionString;

        private User MapReaderToUser(SqliteDataReader reader)
        {
            var createdAtString = reader.IsDBNull(6) ? null : reader.GetString(6);
            var updatedAtString = reader.IsDBNull(7) ? null : reader.GetString(7);

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3),
                ThemeKey = reader.GetString(4),
                AvatarPath = reader.GetString(5),
                CreatedAt = string.IsNullOrEmpty(createdAtString) ? DateTime.MinValue : DateTime.Parse(createdAtString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                UpdatedAt = string.IsNullOrEmpty(updatedAtString) ? DateTime.MinValue : DateTime.Parse(updatedAtString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                HourlyRate = reader.GetDecimal(8),
                SpeedFactor = reader.GetDouble(9)
            };
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
                        ThemeKey TEXT NOT NULL DEFAULT 'InfernalNeon',
                        avatarPath TEXT DEFAULT '',
                        createdAt TEXT DEFAULT (strftime('%Y-%m-%dT%H:%M:%SZ', 'now')),
                        updatedAt TEXT DEFAULT (strftime('%Y-%m-%dT%H:%M:%SZ', 'now'))
                    )";
                command.ExecuteNonQuery();
            }
            return true;
        }

        public static string HashPassword(string plain)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
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
                    INSERT INTO users (username, passwordHash, role, avatarPath, ThemeKey)
                    VALUES (@username, @passwordHash, @role, @avatarPath, @themeKey)";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@passwordHash", HashPassword(string.IsNullOrEmpty(user.PasswordHash) ? "password" : user.PasswordHash));
                command.Parameters.AddWithValue("@role", string.IsNullOrEmpty(user.Role) ? "User" : user.Role);
                command.Parameters.AddWithValue("@avatarPath", user.AvatarPath);
                command.Parameters.AddWithValue("@themeKey", user.ThemeKey);

                return command.ExecuteNonQuery() > 0;
            }
        }

        public bool AddUser(string username, string password, string role)
        {
            User u = new User
            {
                Username = username,
                PasswordHash = password,
                Role = role,
                ThemeKey = "InfernalNeon"
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
                        ThemeKey = @themeKey,
                        updatedAt = @updatedAt
                    WHERE id = @id";
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@avatarPath", user.AvatarPath);
                command.Parameters.AddWithValue("@themeKey", user.ThemeKey);
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
                command.CommandText = $"SELECT {UserColumns} FROM users WHERE username = @username";
                command.Parameters.AddWithValue("@username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToUser(reader);
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

        public bool UpdateAvatarPath(string username, string avatarPath)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE users SET avatarPath = @avatarPath, updatedAt = @updatedAt WHERE username = @username";
                command.Parameters.AddWithValue("@avatarPath", avatarPath);
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
                command.CommandText = $"SELECT {UserColumns} FROM users";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(MapReaderToUser(reader));
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
                command.CommandText = $"SELECT {UserColumns} FROM users WHERE id = @id";
                command.Parameters.AddWithValue("@id", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToUser(reader);
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

        private DateTime ParseDateTime(object? readerValue)
        {
            if (readerValue == null || readerValue == DBNull.Value)
            {
                return DateTime.MinValue;
            }

            var dateStr = readerValue.ToString();
            if (string.IsNullOrWhiteSpace(dateStr) || !DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsedDate))
            {
                return DateTime.MinValue;
            }
            return parsedDate;
        }
    }
}
