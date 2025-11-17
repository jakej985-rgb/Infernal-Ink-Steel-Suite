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
    }
}
