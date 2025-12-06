using Xunit;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Data.Tests
{
    [Collection("Database collection")]
    public class QuoteRepositoryTests
    {
        private readonly DatabaseFixture _fixture;

        public QuoteRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void AddQuote_ShouldUpdateId_AfterInsert()
        {
             // Arrange
            var repository = new QuoteRepository(_fixture.ConnectionString);
            var quote = new Quote
            {
                ArtistId = 1,
                Placement = "Leg",
                Style = "Traditional",
                IsCoverUp = false,
                Width = 15,
                Height = 15,
                CoverageLevel = 2,
                LineComplexity = 2,
                ShadingComplexity = 2,
                ColorComplexity = 2,
                Difficulty = 2,
                EstimatedHoursLow = 3,
                EstimatedHoursHigh = 5,
                PriceLow = 300,
                PriceHigh = 500,
                ShopMinimum = 100,
                RecommendedDeposit = 60,
                ConfidenceScore = 0.8,
                SimilarJobsCount = 0,
                CreatedAt = System.DateTime.UtcNow
            };

            // Act
            var success = repository.AddQuote(quote);

            // Assert
            Assert.True(success);

            // This assertion is what we WANT to pass after the fix.
            // For now, it will fail if the ID is 0.
            // Since this is "demonstrating the bug", I will assert NotEqual(0) and expect it to fail currently if I ran it.
            // But wait, the task is to verify the fix.
            // So I should write a test that fails NOW and passes LATER.

            // If I assert Assert.NotEqual(0, quote.Id), it will fail now.
            Assert.NotEqual(0, quote.Id);
        }
    }
}
