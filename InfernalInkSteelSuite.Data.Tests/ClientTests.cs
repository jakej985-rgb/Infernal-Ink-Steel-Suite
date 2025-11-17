using InfernalInkSteelSuite.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfernalInkSteelSuite.Data.Tests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        [DataRow("John", "Doe", "", "John Doe")]
        [DataRow("John", "Doe", " ", "John Doe")]
        [DataRow("John", "Doe", null, "John Doe")]
        [DataRow("John", "", "Smith", "John Smith")]
        [DataRow("John", " ", "Smith", "John Smith")]
        [DataRow(null, " ", "Smith", "Smith")]
        [DataRow(null, null, "Smith", "Smith")]
        [DataRow("John", "  Michael  ", "Doe", "John Michael Doe")]
        public void FullName_ShouldFormatCorrectly(string firstName, string middleName, string lastName, string expected)
        {
            var client = new Client { FirstName = firstName, MiddleName = middleName, LastName = lastName };
            Assert.AreEqual(expected, client.FullName);
        }
    }
}
