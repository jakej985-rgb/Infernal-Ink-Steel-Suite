using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace InfernalInkSteelSuite.Data
{
    public class DatabaseManager(string connectionString)
    {
        private readonly string _connectionString = connectionString;

        private SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public void InitializeDatabase()
        {
            using var connection = GetConnection();
            connection.Open();
            CreateTable(connection);
            EnsureColumnsExist(connection);
            MigrateShopSettings(connection);
            EnsureDefaultUserExists(connection);
            MigrateUserDateFormats(connection);
        }

        private static void CreateTable(SqliteConnection connection)
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
            CreateQuotesTable(connection);
        }

        private static void CreateQuotesTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS quotes (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    clientId INTEGER,
                    artistId INTEGER,
                    placement TEXT,
                    style TEXT,
                    isCoverUp INTEGER,
                    width REAL,
                    height REAL,
                    coverageLevel INTEGER,
                    lineComplexity INTEGER,
                    shadingComplexity INTEGER,
                    colorComplexity INTEGER,
                    difficulty INTEGER,
                    estimatedHoursLow REAL,
                    estimatedHoursHigh REAL,
                    priceLow REAL,
                    priceHigh REAL,
                    shopMinimum REAL,
                    recommendedDeposit REAL,
                    confidenceScore REAL,
                    similarJobsCount INTEGER,
                    createdAt TEXT
                )";
            command.ExecuteNonQuery();
        }

        private static void CreateDocumentsTable(SqliteConnection connection)
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

        private static void CreateShopSettingsTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS shopsettings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ShopName TEXT,
                    LogoPath TEXT,
                    AccentColor TEXT,
                    SidebarArtworkPath TEXT,
                    LoginBackgroundPath TEXT,
                    LoginHeadlineFontFamily TEXT,
                    LoginTaglineFontFamily TEXT,
                    LoginTextColor TEXT,
                    TattooPerHour REAL NOT NULL DEFAULT 0,
                    PiercingSingle REAL NOT NULL DEFAULT 0,
                    PiercingMulti REAL NOT NULL DEFAULT 0,
                    SpecialMessageText TEXT,
                    IsSpecialMessageEnabled INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT,
                    UpdatedAt TEXT
                )";
            command.ExecuteNonQuery();
        }

        private static void CreateUsersTable(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText =
                @"CREATE TABLE IF NOT EXISTS users (
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

        private static void CreateClientsTable(SqliteConnection connection)
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

        private static void EnsureColumnsExist(SqliteConnection connection)
        {
            EnsureColumnExists(connection, "appointments", "serviceCategory", "TEXT DEFAULT ''");
            EnsureColumnExists(connection, "appointments", "priceType", "TEXT DEFAULT ''");
            EnsureColumnExists(connection, "appointments", "priceCharged", "REAL NOT NULL DEFAULT 0");
            EnsureColumnExists(connection, "shopsettings", "EnableAutomaticHolidayThemes", "INTEGER NOT NULL DEFAULT 0");
            EnsureColumnExists(connection, "shopsettings", "ShopMinimumRate", "REAL NOT NULL DEFAULT 0");
            EnsureColumnExists(connection, "users", "HourlyRate", "DECIMAL NOT NULL DEFAULT 150");
            EnsureColumnExists(connection, "users", "SpeedFactor", "DOUBLE NOT NULL DEFAULT 1.0");
            EnsureColumnExists(connection, "users", "LastLoginAt", "TEXT");
            EnsureColumnExists(connection, "users", "IsActive", "INTEGER", "1");
            EnsureColumnExists(connection, "users", "IsDeleted", "INTEGER", "0");
            EnsureColumnExists(connection, "users", "DeletedAt", "TEXT");
            EnsureColumnExists(connection, "users", "Department", "TEXT", "''");
            EnsureColumnExists(connection, "users", "CommissionRate", "REAL", "0");
            EnsureColumnExists(connection, "users", "FontSize", "INTEGER", "14");
            EnsureColumnExists(connection, "users", "KeyboardShortcutsJson", "TEXT", "''");
            EnsureColumnExists(connection, "users", "PermissionsJson", "TEXT", "''");
            EnsureColumnExists(connection, "clients", "photoPath", "TEXT DEFAULT ''");
        }

        private static void MigrateShopSettings(SqliteConnection connection)
        {
            // First, ensure the new columns exist.
            EnsureColumnExists(connection, "shopsettings", "SpecialMessageText", "TEXT");
            EnsureColumnExists(connection, "shopsettings", "IsSpecialMessageEnabled", "INTEGER NOT NULL DEFAULT 1");

            // Then, if the old column exists, copy its data to the new one.
            if (TableHasColumn(connection, "shopsettings", "LoginTagline"))
            {
                var command = connection.CreateCommand();
                command.CommandText = "UPDATE shopsettings SET SpecialMessageText = LoginTagline WHERE SpecialMessageText IS NULL";
                command.ExecuteNonQuery();
            }
        }

        private static bool TableHasColumn(SqliteConnection connection, string tableName, string columnName)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var existingName = reader.GetString(1);
                if (string.Equals(existingName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static void EnsureColumnExists(SqliteConnection connection, string tableName, string columnName, string columnDefinition = "TEXT", string defaultValue = "")
        {
            if (TableHasColumn(connection, tableName, columnName))
            {
                return;
            }

            try
            {
                var command = connection.CreateCommand();
                var defaultClause = string.IsNullOrEmpty(defaultValue) ? "" : $" DEFAULT {defaultValue}";
                command.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition}{defaultClause}";
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex) when (ex.Message.Contains("duplicate column name", StringComparison.OrdinalIgnoreCase))
            {
                // Ignore duplicate column error
            }
        }
        private static void EnsureDefaultUserExists(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM users";
            var userCount = (long?)command.ExecuteScalar() ?? 0L;

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
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow.ToString("o"));

                command.ExecuteNonQuery();
            }
        }

        private static string GetSha256Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        private static void MigrateUserDateFormats(SqliteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT id, createdAt, updatedAt FROM users";

            var usersToUpdate = new System.Collections.Generic.List<(int id, string? createdAt, string? updatedAt)>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var createdAt = reader.IsDBNull(1) ? null : reader.GetString(1);
                    var updatedAt = reader.IsDBNull(2) ? null : reader.GetString(2);

                    string? newCreatedAt = null;
                    string? newUpdatedAt = null;

                    if (createdAt != null && !DateTime.TryParse(createdAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _))
                    {
                        if (DateTime.TryParse(createdAt, out DateTime parsedDate))
                        {
                            newCreatedAt = parsedDate.ToString("o");
                        }
                    }

                    if (updatedAt != null && !DateTime.TryParse(updatedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _))
                    {
                        if (DateTime.TryParse(updatedAt, out DateTime parsedDate))
                        {
                            newUpdatedAt = parsedDate.ToString("o");
                        }
                    }

                    if (newCreatedAt != null || newUpdatedAt != null)
                    {
                        usersToUpdate.Add((id, newCreatedAt, newUpdatedAt));
                    }
                }
            }

            foreach (var (id, createdAt, updatedAt) in usersToUpdate)
            {
                var updateCommand = connection.CreateCommand();
                var setClauses = new System.Collections.Generic.List<string>();
                if (createdAt != null)
                {
                    setClauses.Add("createdAt = @createdAt");
                    updateCommand.Parameters.AddWithValue("@createdAt", createdAt);
                }
                if (updatedAt != null)
                {
                    setClauses.Add("updatedAt = @updatedAt");
                    updateCommand.Parameters.AddWithValue("@updatedAt", updatedAt);
                }

                if (setClauses.Count > 0)
                {
                    updateCommand.CommandText = $"UPDATE users SET {string.Join(", ", setClauses)} WHERE id = @id";
                    updateCommand.Parameters.AddWithValue("@id", id);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
