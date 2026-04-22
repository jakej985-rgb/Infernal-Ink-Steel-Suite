using System;
using InfernalInkSteelSuite.Services;
using Xunit;

namespace InfernalInkSteelSuite.Data.Tests
{
    public class HolidayThemeServiceTests
    {
        [Theory]
        [InlineData(2023, 11, 23)] // Nov 1 is Wed
        [InlineData(2024, 11, 28)] // Nov 1 is Fri
        [InlineData(2025, 11, 27)] // Nov 1 is Sat
        [InlineData(2026, 11, 26)] // Nov 1 is Sun
        [InlineData(2022, 11, 24)] // Nov 1 is Tue
        public void GetThanksgivingDate_ReturnsCorrectFourthThursday(int year, int expectedMonth, int expectedDay)
        {
            // Note: GetThanksgivingDate is private in HolidayThemeService. 
            // For testing purposes, we can use reflection or temporarily make it internal/public.
            // Since I am remediating the code, I will make it public for testability.
            
            var result = (DateTime)typeof(HolidayThemeService)
                .GetMethod("GetThanksgivingDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, [year]);

            Assert.Equal(new DateTime(year, expectedMonth, expectedDay), result);
        }
    }
}
