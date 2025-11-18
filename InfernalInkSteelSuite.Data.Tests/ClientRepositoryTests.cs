using Xunit;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Data.Tests
{
    [Collection("Database collection")]
    public class ClientRepositoryTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly ClientRepository _repository;

        public ClientRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new ClientRepository(_fixture.ConnectionString);
        }

        [Fact]
        public void GetClientIdByName_WithNameAndWithoutMiddleName_ReturnsCorrectId()
        {
            var client = new Client
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            };
            _repository.Insert(client);

            var clientId = _repository.GetClientIdByName("Jane Doe");

            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WithNameAndMiddleName_ReturnsCorrectId()
        {
            var client = new Client
            {
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Smith",
                Email = "john.smith@example.com"
            };
            _repository.Insert(client);

            var clientId = _repository.GetClientIdByName("John Michael Smith");

            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WithNameAndMiddleName_WhenSearchingWithoutMiddleName_ReturnsCorrectId()
        {
            var client = new Client
            {
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Smith",
                Email = "john.smith@example.com"
            };
            _repository.Insert(client);

            var clientId = _repository.GetClientIdByName("John Smith");

            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WhenSearchingCaseInsensitive_ReturnsCorrectId()
        {
            var client = new Client
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            };
            _repository.Insert(client);

            var clientId = _repository.GetClientIdByName("jane doe");

            Assert.Equal(client.Id, clientId);
        }

        [Fact]
        public void GetClientIdByName_WithAmbiguousNames_ReturnsCorrectId()
        {
            var client1 = new Client
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@example.com"
            };
            _repository.Insert(client1);

            var client2 = new Client
            {
                FirstName = "John",
                MiddleName = "Michael",
                LastName = "Smith",
                Email = "john.michael.smith@example.com"
            };
            _repository.Insert(client2);

            var clientId = _repository.GetClientIdByName("John Smith");

            Assert.Equal(client1.Id, clientId);
        }
    }
}
