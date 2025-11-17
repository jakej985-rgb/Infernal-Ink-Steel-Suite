using Xunit;
using Microsoft.Data.Sqlite;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Data;
using System;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private const string ConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;

        public UserRepositoryTests()
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();
            var databaseManager = new DatabaseManager(ConnectionString);
            databaseManager.InitializeDatabase();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        [Fact]
        public void UpdateUser_PasswordShouldNotBeUpdatable()
        {
            // Arrange
            var repository = new UserRepository(ConnectionString);
            repository.AddUser("testuser", "oldpassword", "User");

            var userToUpdate = repository.GetUserByUsername("testuser");
            Assert.NotNull(userToUpdate);

            // Act
            userToUpdate.PasswordHash = "newpassword";
            repository.UpdateUser(userToUpdate);

            // Assert
            var isNewPasswordCorrect = repository.CheckPassword("testuser", "newpassword");
            Assert.False(isNewPasswordCorrect, "Password was updated, but it should not have been.");
            var isOldPasswordCorrect = repository.CheckPassword("testuser", "oldpassword");
            Assert.True(isOldPasswordCorrect, "Old password should still be valid.");
        }
    }
}
