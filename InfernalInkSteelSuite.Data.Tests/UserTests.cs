using InfernalInkSteelSuite.Domain;
using Xunit;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class UserTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("User", false)]
        [InlineData("Admin", true)]
        [InlineData("admin", true)]
        [InlineData("ADMIN", true)]
        public void IsAdmin_ShouldReturnCorrectValueForRole(string role, bool expected)
        {
            // Arrange
            var user = new User { Role = role };

            // Act
            var result = user.IsAdmin();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
