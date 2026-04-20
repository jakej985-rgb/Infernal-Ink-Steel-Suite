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

            var hasher = new Repositories.Services.PasswordHasher();
            _repository = new UserRepository(_context, hasher);
        }

        public void Dispose()
        {
            _context.Database.CloseConnection();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void UpdateUser_ShouldCorrectlyPersistChanges()
        {
            // Arrange
            _repository.AddUser("testuser", "oldpassword", "User");
            var userToUpdate = _repository.GetUserByUsername("testuser");
            Assert.NotNull(userToUpdate);

            // Act - Manually update a field (Note: Repository.UpdateUser(user) persists modified tracked properties)
            userToUpdate.Role = "Admin";
            _repository.UpdateUser(userToUpdate);

            // Assert
            var updatedUser = _repository.GetUserByUsername("testuser");
            Assert.Equal("Admin", updatedUser?.Role);
            Assert.True(_repository.CheckPassword("testuser", "oldpassword"), "Original password should still be valid after a non-password update.");
        }
    }
}
