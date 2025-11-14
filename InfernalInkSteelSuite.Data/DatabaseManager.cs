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
