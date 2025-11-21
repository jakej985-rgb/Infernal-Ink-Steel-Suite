using InfernalInkSteelSuite.Domain;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace InfernalInkSteelSuite.Repositories
{
    public class QuoteRepository(string connectionString) : IQuoteRepository
    {
        private readonly string _connectionString = connectionString;

        public bool AddQuote(Quote quote)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                    INSERT INTO quotes (
                        clientId, artistId, placement, style, isCoverUp, width, height,
                        coverageLevel, lineComplexity, shadingComplexity, colorComplexity,
                        difficulty, estimatedHoursLow, estimatedHoursHigh, priceLow,
                        priceHigh, shopMinimum, recommendedDeposit, confidenceScore,
                        similarJobsCount, createdAt)
                    VALUES (
                        @clientId, @artistId, @placement, @style, @isCoverUp, @width, @height,
                        @coverageLevel, @lineComplexity, @shadingComplexity, @colorComplexity,
                        @difficulty, @estimatedHoursLow, @estimatedHoursHigh, @priceLow,
                        @priceHigh, @shopMinimum, @recommendedDeposit, @confidenceScore,
                        @similarJobsCount, @createdAt)";

            command.Parameters.AddWithValue("@clientId", quote.ClientId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@artistId", quote.ArtistId);
            command.Parameters.AddWithValue("@placement", quote.Placement);
            command.Parameters.AddWithValue("@style", quote.Style);
            command.Parameters.AddWithValue("@isCoverUp", quote.IsCoverUp);
            command.Parameters.AddWithValue("@width", quote.Width);
            command.Parameters.AddWithValue("@height", quote.Height);
            command.Parameters.AddWithValue("@coverageLevel", quote.CoverageLevel);
            command.Parameters.AddWithValue("@lineComplexity", quote.LineComplexity);
            command.Parameters.AddWithValue("@shadingComplexity", quote.ShadingComplexity);
            command.Parameters.AddWithValue("@colorComplexity", quote.ColorComplexity);
            command.Parameters.AddWithValue("@difficulty", quote.Difficulty);
            command.Parameters.AddWithValue("@estimatedHoursLow", quote.EstimatedHoursLow);
            command.Parameters.AddWithValue("@estimatedHoursHigh", quote.EstimatedHoursHigh);
            command.Parameters.AddWithValue("@priceLow", quote.PriceLow);
            command.Parameters.AddWithValue("@priceHigh", quote.PriceHigh);
            command.Parameters.AddWithValue("@shopMinimum", quote.ShopMinimum);
            command.Parameters.AddWithValue("@recommendedDeposit", quote.RecommendedDeposit);
            command.Parameters.AddWithValue("@confidenceScore", quote.ConfidenceScore);
            command.Parameters.AddWithValue("@similarJobsCount", quote.SimilarJobsCount);
            command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));

            return command.ExecuteNonQuery() > 0;
        }

        public List<Quote> GetAllQuotes()
        {
            var quotes = new List<Quote>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM quotes";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                quotes.Add(MapReaderToQuote(reader));
            }
            return quotes;
        }

        private static Quote MapReaderToQuote(SqliteDataReader reader)
        {
            return new Quote
            {
                Id = reader.GetInt32(0),
                ClientId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                ArtistId = reader.GetInt32(2),
                Placement = reader.GetString(3),
                Style = reader.GetString(4),
                IsCoverUp = reader.GetBoolean(5),
                Width = reader.GetDouble(6),
                Height = reader.GetDouble(7),
                CoverageLevel = reader.GetInt32(8),
                LineComplexity = reader.GetInt32(9),
                ShadingComplexity = reader.GetInt32(10),
                ColorComplexity = reader.GetInt32(11),
                Difficulty = reader.GetInt32(12),
                EstimatedHoursLow = reader.GetDouble(13),
                EstimatedHoursHigh = reader.GetDouble(14),
                PriceLow = reader.GetDecimal(15),
                PriceHigh = reader.GetDecimal(16),
                ShopMinimum = reader.GetDecimal(17),
                RecommendedDeposit = reader.GetDecimal(18),
                ConfidenceScore = reader.GetDouble(19),
                SimilarJobsCount = reader.GetInt32(20),
                CreatedAt = DateTime.Parse(reader.GetString(21), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            };
        }
    }
}
