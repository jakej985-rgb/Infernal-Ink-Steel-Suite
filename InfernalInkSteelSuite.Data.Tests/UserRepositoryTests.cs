using Xunit;
using InfernalInkSteelSuite.Repositories;
using Microsoft.EntityFrameworkCore;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly UserRepository _repository;
        private readonly AppDbContext _context;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;
            _context = new AppDbContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _repository = new UserRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.CloseConnection();
            _context.Dispose();
            GC.SuppressFinalize(this);
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
