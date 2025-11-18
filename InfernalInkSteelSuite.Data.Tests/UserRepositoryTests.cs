using Xunit;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Data.Tests
{
    [Collection("Database collection")]
    public class UserRepositoryTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly UserRepository _repository;

        public UserRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new UserRepository(_fixture.ConnectionString);
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
