using System;

namespace InfernalInkSteelSuite.Domain
{
    public class Quote
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int ArtistId { get; set; }
        public string Placement { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public bool IsCoverUp { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int CoverageLevel { get; set; }
        public int LineComplexity { get; set; }
        public int ShadingComplexity { get; set; }
        public int ColorComplexity { get; set; }
        public int Difficulty { get; set; }
        public double EstimatedHoursLow { get; set; }
        public double EstimatedHoursHigh { get; set; }
        public decimal PriceLow { get; set; }
        public decimal PriceHigh { get; set; }
        public decimal ShopMinimum { get; set; }
        public decimal RecommendedDeposit { get; set; }
        public double ConfidenceScore { get; set; }
        public int SimilarJobsCount { get; set; }
        public string? Notes { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
