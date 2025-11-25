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
        private const string UserColumns = "id, username, passwordHash, role, ThemeKey, avatarPath, createdAt, updatedAt, HourlyRate, SpeedFactor, LastLoginAt, IsActive, IsDeleted, DeletedAt, Department, CommissionRate, FontSize, KeyboardShortcutsJson, PermissionsJson";
        private readonly string _connectionString = connectionString;

        private static User MapReaderToUser(SqliteDataReader reader)
        {
            var createdAtString = reader.IsDBNull(6) ? null : reader.GetString(6);
            var updatedAtString = reader.IsDBNull(7) ? null : reader.GetString(7);
            var lastLoginAtString = reader.IsDBNull(10) ? null : reader.GetString(10);
            var deletedAtString = reader.IsDBNull(13) ? null : reader.GetString(13);

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
                SpeedFactor = reader.GetDouble(9),
                LastLoginAt = string.IsNullOrEmpty(lastLoginAtString) ? null : DateTime.Parse(lastLoginAtString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                IsActive = !reader.IsDBNull(11) && reader.GetInt32(11) == 1,
                IsDeleted = !reader.IsDBNull(12) && reader.GetInt32(12) == 1,
                DeletedAt = string.IsNullOrEmpty(deletedAtString) ? null : DateTime.Parse(deletedAtString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                Department = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                CommissionRate = reader.IsDBNull(15) ? 0m : reader.GetDecimal(15),
                FontSize = reader.IsDBNull(16) ? 14 : reader.GetInt32(16),
                KeyboardShortcutsJson = reader.IsDBNull(17) ? string.Empty : reader.GetString(17),
                PermissionsJson = reader.IsDBNull(18) ? string.Empty : reader.GetString(18)
            };
        }

        public bool CreateTable()
        {
            using var connection = new SqliteConnection(_connectionString);
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
            return true;
        }

        public string HashPassword(string plain) => ComputeHash(plain);

        private static string ComputeHash(string plain)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plain));
            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public string? GetUsernameById(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT username FROM users WHERE id = @id";
            command.Parameters.AddWithValue("@id", userId);

            var result = command.ExecuteScalar();
            return result?.ToString();
        }

        public bool AddUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
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

        public bool AddUser(string username, string password, string role)
        {
            User u = new()
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
            using var connection = new SqliteConnection(_connectionString);
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

        public User? GetUserByUsername(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT {UserColumns} FROM users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToUser(reader);
            }
            return null;
        }

        public bool CheckPassword(string username, string plainPassword)
        {
            User? u = GetUserByUsername(username);
            if (u == null) return false;
            return u.PasswordHash == ComputeHash(plainPassword);
        }

        public bool DeleteUser(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateRole(string username, string role)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET role = @role, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@role", role);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdatePassword(string username, string password)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET passwordHash = @passwordHash, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@passwordHash", HashPassword(password));
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateAvatarPath(string username, string avatarPath)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET avatarPath = @avatarPath, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@avatarPath", avatarPath);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Ensure new columns exist
            EnsureColumnExists(connection, "LastLoginAt", "TEXT");
            EnsureColumnExists(connection, "IsActive", "INTEGER", "1");
            EnsureColumnExists(connection, "IsDeleted", "INTEGER", "0");
            EnsureColumnExists(connection, "DeletedAt", "TEXT");
            EnsureColumnExists(connection, "Department", "TEXT", "''");
            EnsureColumnExists(connection, "CommissionRate", "REAL", "0");
            EnsureColumnExists(connection, "FontSize", "INTEGER", "14");
            EnsureColumnExists(connection, "KeyboardShortcutsJson", "TEXT", "''");

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT {UserColumns} FROM users";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(MapReaderToUser(reader));
            }
            return users;
        }

        public User? GetUserById(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT {UserColumns} FROM users WHERE id = @id";
            command.Parameters.AddWithValue("@id", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToUser(reader);
            }
            return null;
        }

        public bool DeleteUser(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM users WHERE id = @id";
            command.Parameters.AddWithValue("@id", userId);

            return command.ExecuteNonQuery() > 0;
        }

        private static DateTime ParseDateTime(object? readerValue)
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

        // New methods for Phase 1
        public List<User> GetActiveUsers()
        {
            var users = new List<User>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            EnsureColumnExists(connection, "IsActive", "INTEGER", "1");
            EnsureColumnExists(connection, "IsDeleted", "INTEGER", "0");

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT {UserColumns} FROM users WHERE IsActive = 1 AND IsDeleted = 0";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(MapReaderToUser(reader));
            }
            return users;
        }

        public bool UpdateLastLogin(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "LastLogin At", "TEXT");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET LastLoginAt = @lastLoginAt, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@lastLoginAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool SetUserActiveStatus(string username, bool isActive)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "IsActive", "INTEGER", "1");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET IsActive = @isActive, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool SoftDeleteUser(string username)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "IsDeleted", "INTEGER", "0");
            EnsureColumnExists(connection, "DeletedAt", "TEXT");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET IsDeleted = 1, DeletedAt = @deletedAt, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@deletedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateUserPermissions(string username, string permissionsJson)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "PermissionsJson", "TEXT", "''");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET PermissionsJson = @permissionsJson, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@permissionsJson", permissionsJson);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateUserDepartment(string username, string department)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "Department", "TEXT", "''");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET Department = @department, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@department", department);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateUserCommissionRate(string username, decimal rate)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "CommissionRate", "REAL", "0");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET CommissionRate = @rate, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@rate", rate);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateUserFontSize(string username, int fontSize)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            EnsureColumnExists(connection, "FontSize", "INTEGER", "14");

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE users SET FontSize = @fontSize, updatedAt = @updatedAt WHERE username = @username";
            command.Parameters.AddWithValue("@fontSize", fontSize);
            command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));
            command.Parameters.AddWithValue("@username", username);

            return command.ExecuteNonQuery() > 0;
        }

        private static void EnsureColumnExists(SqliteConnection connection, string columnName, string columnType, string defaultValue = "''")
        {
            var command = connection.CreateCommand();
            command.CommandText = "PRAGMA table_info(users)";
            bool exists = false;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"]?.ToString()?.Equals(columnName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
            {
                var alter = connection.CreateCommand();
                alter.CommandText = $"ALTER TABLE users ADD COLUMN {columnName} {columnType} DEFAULT {defaultValue}";
                alter.ExecuteNonQuery();
            }
        }
    }
}
