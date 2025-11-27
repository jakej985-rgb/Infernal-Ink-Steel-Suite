namespace InfernalInkSteelSuite.Domain
{
    public class QuoteEstimate
    {
        public double EstimatedHoursLow { get; set; }
        public double EstimatedHoursHigh { get; set; }
        public decimal PriceLow { get; set; }
        public decimal PriceHigh { get; set; }

        public decimal ShopMinimum { get; set; }
        public decimal RecommendedDeposit { get; set; }

        public double ConfidenceScore { get; set; } // 0–1
        public int SimilarJobsCount { get; set; }
    }
}
