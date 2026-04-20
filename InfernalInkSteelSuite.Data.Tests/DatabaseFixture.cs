using Microsoft.EntityFrameworkCore;
using InfernalInkSteelSuite.Data;
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private readonly SqliteConnection _connection;
        public AppDbContext Context { get; }

        public DatabaseFixture()
        {
            // Use in-memory SQLite for parallel-safe, fast tests
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(_connection);
            
            Context = new AppDbContext(optionsBuilder.Options);
            Context.Database.EnsureCreated();

            // Note: DatabaseManager is retired. All schema is handled via EF Core.
        }

        public void Dispose()
        {
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    [Xunit.CollectionDefinition("Database collection")]
    public class DatabaseCollection : Xunit.ICollectionFixture<DatabaseFixture>
    {
    }
}
