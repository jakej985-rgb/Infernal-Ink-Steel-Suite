using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public string ConnectionString { get; }

        public DatabaseFixture()
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "test_shop_manager.db");
            ConnectionString = $"Data Source={dbPath}";

            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            var databaseManager = new DatabaseManager(ConnectionString);
            databaseManager.InitializeDatabase();
        }

        public void Dispose()
        {
            var dbPath = new SqliteConnectionStringBuilder(ConnectionString).DataSource;
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }

    [Xunit.CollectionDefinition("Database collection")]
    public class DatabaseCollection : Xunit.ICollectionFixture<DatabaseFixture>
    {
    }
}
