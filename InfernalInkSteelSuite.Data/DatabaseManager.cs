using Microsoft.Data.Sqlite;
using System;
using System.Data;

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
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Salt TEXT NOT NULL,
                    Role TEXT NOT NULL
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
    }
}
