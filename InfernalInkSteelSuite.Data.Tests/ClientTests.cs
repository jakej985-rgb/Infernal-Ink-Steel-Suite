using InfernalInkSteelSuite.Domain;
using Xunit;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class ClientTests
    {
        [Theory]
        [InlineData("John", "Doe", "", "John Doe")]
        [InlineData("John", "Doe", " ", "John Doe")]
        [InlineData("John", "Doe", null, "John Doe")]
        [InlineData("John", "", "Smith", "John Smith")]
        [InlineData("John", " ", "Smith", "John Smith")]
        [InlineData(null, " ", "Smith", "Smith")]
        [InlineData(null, null, "Smith", "Smith")]
        [InlineData("John", "  Michael  ", "Doe", "John Michael Doe")]
        public void FullName_ShouldFormatCorrectly(string firstName, string middleName, string lastName, string expected)
        {
            var client = new Client { FirstName = firstName, MiddleName = middleName, LastName = lastName };
            Assert.Equal(expected, client.FullName);
        }

        [Fact]
        public void FullName_ShouldTrimWhitespaceFromAllNameParts()
        {
            var client = new Client { FirstName = "  John  ", MiddleName = "  Michael  ", LastName = "  Doe  " };
            Assert.Equal("John Michael Doe", client.FullName);
        }
    }
}
