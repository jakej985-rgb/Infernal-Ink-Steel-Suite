using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public void InitializeDatabase()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                CreateTable(connection);
                EnsureColumnsExist(connection);
                EnsureDefaultUserExists(connection);
            }
        }

        private void CreateTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS appointments (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    clientId INTEGER,
                    userId INTEGER,
                    clientName TEXT,
                    dateTime TEXT,
                    durationMinutes INTEGER,
                    serviceType TEXT,
                    serviceCategory TEXT DEFAULT '',
                    priceType TEXT DEFAULT '',
                    priceCharged REAL NOT NULL DEFAULT 0,
                    notes TEXT,
                    color TEXT,
                    status TEXT NOT NULL DEFAULT 'Scheduled'
                )";
            command.ExecuteNonQuery();

            CreateClientsTable(connection);
            CreateDocumentsTable(connection);
            CreateUsersTable(connection);
            CreateShopSettingsTable(connection);
        }

        private void CreateDocumentsTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS documents (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    userId INTEGER,
                    clientId INTEGER,
                    title TEXT,
                    filePath TEXT,
                    createdAt TEXT
                )";
            command.ExecuteNonQuery();
        }

        private void CreateShopSettingsTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS shopsettings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShopName TEXT,
                    LogoPath TEXT,
                    AccentColor TEXT,
                    SidebarArtworkPath TEXT,
                    LoginHeadline TEXT,
                    LoginTagline TEXT,
                    LoginBackgroundPath TEXT,
                    LoginHeadlineFontFamily TEXT,
                    LoginTaglineFontFamily TEXT,
                    LoginTextColor TEXT,
                    TattooPerHour REAL NOT NULL DEFAULT 0,
                    PiercingSingle REAL NOT NULL DEFAULT 0,
                    PiercingMulti REAL NOT NULL DEFAULT 0,
                    CreatedAt TEXT,
                    UpdatedAt TEXT
                )";
            command.ExecuteNonQuery();
        }

        private void CreateUsersTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS users (
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

        private void CreateClientsTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS clients (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    firstName TEXT,
                    middleName TEXT,
                    lastName TEXT,
                    phone TEXT,
                    email TEXT,
                    notes TEXT,
                    visits INTEGER
                )";
            command.ExecuteNonQuery();
        }

        private void EnsureColumnsExist(SqliteConnection connection)
        {
            EnsureColumnExists(connection, "appointments", "serviceCategory", "TEXT DEFAULT ''");
            EnsureColumnExists(connection, "appointments", "priceType", "TEXT DEFAULT ''");
            EnsureColumnExists(connection, "appointments", "priceCharged", "REAL NOT NULL DEFAULT 0");
            EnsureColumnExists(connection, "users", "avatarPath", "TEXT DEFAULT ''");
            EnsureColumnExists(connection, "users", "createdAt", "TEXT");
            EnsureColumnExists(connection, "users", "updatedAt", "TEXT");
        }

        private bool TableHasColumn(SqliteConnection connection, string tableName, string columnName)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(1) == columnName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void EnsureColumnExists(SqliteConnection connection, string tableName, string columnName, string columnDefinition)
        {
            if (!TableHasColumn(connection, tableName, columnName))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition}";
                command.ExecuteNonQuery();
            }
        }
        private void EnsureDefaultUserExists(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM users";
            var userCount = (long)command.ExecuteScalar();

            if (userCount == 0)
            {
                command.CommandText =
                    @"INSERT INTO users (username, passwordHash, role, avatarPath, createdAt, updatedAt)
                        VALUES (@username, @passwordHash, @role, @avatarPath, @createdAt, @updatedAt)";

                var passwordHash = GetSha256Hash("password");

                command.Parameters.AddWithValue("@username", "admin");
                command.Parameters.AddWithValue("@passwordHash", passwordHash);
                command.Parameters.AddWithValue("@role", "Admin");
                command.Parameters.AddWithValue("@avatarPath", "");
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

                command.ExecuteNonQuery();
            }
        }

        private static string GetSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
