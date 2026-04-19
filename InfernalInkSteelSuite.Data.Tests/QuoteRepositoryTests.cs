using Xunit;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Linq;
using InfernalInkSteelSuite.Data;

namespace InfernalInkSteelSuite.Data.Tests
{
    [Collection("Database collection")]
    public class QuoteRepositoryTests(DatabaseFixture fixture)
    {
        private readonly DatabaseFixture _fixture = fixture;

        [Fact]
        public void AddQuote_ShouldUpdateId_AfterInsert()
        {
             // Arrange
            var repository = new QuoteRepository(_fixture.Context);
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
            Assert.NotEqual(0, quote.Id);

            // Verify the quote was actually saved to the database with the correct ID
            var allQuotes = repository.GetAllQuotes();
            var savedQuote = allQuotes.FirstOrDefault(q => q.Id == quote.Id);

            Assert.NotNull(savedQuote);
            Assert.Equal(quote.ArtistId, savedQuote.ArtistId);
            Assert.Equal(quote.Placement, savedQuote.Placement);
            Assert.Equal(quote.Style, savedQuote.Style);
        }
    }
}
