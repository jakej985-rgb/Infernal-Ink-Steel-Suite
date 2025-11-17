using Xunit;
using Microsoft.Data.Sqlite;
using System;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class DatabaseManagerTests
    {
        [Fact]
        public void InitializeDatabase_WithInvalidDateFormat_DoesNotThrowException()
        {
            var connectionString = "DataSource=:memory:";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText =
                    @"CREATE TABLE users (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        username TEXT UNIQUE NOT NULL,
                        passwordHash TEXT NOT NULL,
                        role TEXT NOT NULL,
                        ThemeKey TEXT NOT NULL DEFAULT 'InfernalNeon',
                        avatarPath TEXT,
                        createdAt TEXT,
                        updatedAt TEXT
                    )";
                createTableCommand.ExecuteNonQuery();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText =
                    @"INSERT INTO users (username, passwordHash, role, createdAt)
                      VALUES ('testuser', 'hash', 'User', 'invalid-date')";
                insertCommand.ExecuteNonQuery();

                var databaseManager = new DatabaseManager(connectionString);

                var exception = Record.Exception(() => databaseManager.InitializeDatabase());
                Assert.Null(exception);
            }
        }
    }
}
