using Xunit;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class ClientRepositoryTests
    {
        private const string ConnectionString = "DataSource=:memory:";

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var databaseManager = new DatabaseManager(ConnectionString);
                databaseManager.InitializeDatabase();
            }
        }

        [Fact]
        public void GetClientIdByName_WithNameAndWithoutMiddleName_ReturnsCorrectId()
        {
            CreateTable();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var repository = new ClientRepository(ConnectionString);
                var client = new Client
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@example.com"
                };
                repository.Insert(client);

                var clientId = repository.GetClientIdByName("Jane Doe");

                Assert.Equal(client.Id, clientId);
            }
        }

        [Fact]
        public void GetClientIdByName_WithNameAndMiddleName_ReturnsCorrectId()
        {
            CreateTable();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var repository = new ClientRepository(ConnectionString);
                var client = new Client
                {
                    FirstName = "John",
                    MiddleName = "Michael",
                    LastName = "Smith",
                    Email = "john.smith@example.com"
                };
                repository.Insert(client);

                var clientId = repository.GetClientIdByName("John Michael Smith");

                Assert.Equal(client.Id, clientId);
            }
        }

        [Fact]
        public void GetClientIdByName_WithNameAndMiddleName_WhenSearchingWithoutMiddleName_ReturnsCorrectId()
        {
            CreateTable();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var repository = new ClientRepository(ConnectionString);
                var client = new Client
                {
                    FirstName = "John",
                    MiddleName = "Michael",
                    LastName = "Smith",
                    Email = "john.smith@example.com"
                };
                repository.Insert(client);

                var clientId = repository.GetClientIdByName("John Smith");

                Assert.Equal(client.Id, clientId);
            }
        }

        [Fact]
        public void GetClientIdByName_WhenSearchingCaseInsensitive_ReturnsCorrectId()
        {
            CreateTable();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var repository = new ClientRepository(ConnectionString);
                var client = new Client
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "jane.doe@example.com"
                };
                repository.Insert(client);

                var clientId = repository.GetClientIdByName("jane doe");

                Assert.Equal(client.Id, clientId);
            }
        }

        [Fact]
        public void GetClientIdByName_WithAmbiguousNames_ReturnsCorrectId()
        {
            CreateTable();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                var repository = new ClientRepository(ConnectionString);
                var client1 = new Client
                {
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@example.com"
                };
                repository.Insert(client1);

                var client2 = new Client
                {
                    FirstName = "John",
                    MiddleName = "Michael",
                    LastName = "Smith",
                    Email = "john.michael.smith@example.com"
                };
                repository.Insert(client2);

                var clientId = repository.GetClientIdByName("John Smith");

                Assert.Equal(client1.Id, clientId);
            }
        }
    }
}
