using Xunit;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class ClientRepositoryTests : IDisposable
    {
        private readonly ClientRepository _repository;
        private readonly DbConnection _connection;

        public ClientRepositoryTests()
        {
            var connectionString = "Data Source=ClientTestDb;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(connectionString);
            _connection.Open(); // Keep the connection open to keep the DB alive

            var databaseManager = new DatabaseManager(connectionString);
            databaseManager.InitializeDatabase();
            _repository = new ClientRepository(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public void GetClientIdByName_WithNameAndWithoutMiddleName_ReturnsCorrectId()
        {
            var client = new Client { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
            _repository.Insert(client);
            var clientId = _repository.GetClientIdByName("Jane Doe");
            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WithNameAndMiddleName_ReturnsCorrectId()
        {
            var client = new Client { FirstName = "John", MiddleName = "Michael", LastName = "Smith", Email = "john.smith@example.com" };
            _repository.Insert(client);
            var clientId = _repository.GetClientIdByName("John Michael Smith");
            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WithNameAndMiddleName_WhenSearchingWithoutMiddleName_ReturnsCorrectId()
        {
            var clientWithMiddle = new Client { FirstName = "John", MiddleName = "Michael", LastName = "Smith", Email = "john.michael.smith@example.com" };
            _repository.Insert(clientWithMiddle);
            var clientWithoutMiddle = new Client { FirstName = "John", LastName = "Smith", Email = "john.smith@example.com" };
            _repository.Insert(clientWithoutMiddle);
            var clientId = _repository.GetClientIdByName("John Smith");
            Assert.Equal(clientWithoutMiddle.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WhenSearchingCaseInsensitive_ReturnsCorrectId()
        {
            var client = new Client { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
            _repository.Insert(client);
            var clientId = _repository.GetClientIdByName("jane doe");
            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WithAmbiguousNames_ReturnsCorrectId()
        {
            var client1 = new Client { FirstName = "John", LastName = "Smith", Email = "john.smith@example.com" };
            _repository.Insert(client1);
            var client2 = new Client { FirstName = "John", MiddleName = "Michael", LastName = "Smith", Email = "john.michael.smith@example.com" };
            _repository.Insert(client2);
            var clientId = _repository.GetClientIdByName("John Smith");
            Assert.Equal(client1.Id, clientId);
        }
    }
}
