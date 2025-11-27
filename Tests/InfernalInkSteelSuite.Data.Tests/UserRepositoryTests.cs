using Xunit;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly UserRepository _repository;
        private readonly DbConnection _connection;

        public UserRepositoryTests()
        {
            var connectionString = "Data Source=UserTestDb;Mode=Memory;Cache=Shared";
            _connection = new SqliteConnection(connectionString);
            _connection.Open();

            var databaseManager = new DatabaseManager(connectionString);
            databaseManager.InitializeDatabase();
            _repository = new UserRepository(connectionString);
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public void UpdateUser_PasswordShouldNotBeUpdatable()
        {
            // Arrange
            _repository.AddUser("testuser", "oldpassword", "User");
            var userToUpdate = _repository.GetUserByUsername("testuser");
            Assert.NotNull(userToUpdate);

            // Act
            userToUpdate.PasswordHash = "newpassword";
            _repository.UpdateUser(userToUpdate);

            // Assert
            var isNewPasswordCorrect = _repository.CheckPassword("testuser", "newpassword");
            Assert.False(isNewPasswordCorrect, "Password was updated, but it should not have been.");
            var isOldPasswordCorrect = _repository.CheckPassword("testuser", "oldpassword");
            Assert.True(isOldPasswordCorrect, "Old password should still be valid.");
        }
    }
}
