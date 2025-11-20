using System;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Services
{
    public class TattooPricingService : ITattooPricingService
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly IUserRepository _userRepository;

        public TattooPricingService(IShopSettingsRepository shopSettingsRepository, IUserRepository userRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            _userRepository = userRepository;
        }

        public QuoteEstimate GetEstimate(QuoteInput input)
        {
            // Example, tweak later:
            double baseArea = input.Width * input.Height; // assume in² or cm²

            double coverageFactor = 0.6 + 0.1 * input.CoverageLevel;      // 1–5 → 1.1–1.6
            double lineFactor = 0.8 + 0.1 * input.LineComplexity;
            double shadeFactor = 0.8 + 0.1 * input.ShadingComplexity;
            double colorFactor = 0.8 + 0.1 * input.ColorComplexity;
            double difficultyFactor = 0.8 + 0.1 * input.Difficulty;

            double effectiveArea = baseArea * coverageFactor;
            double complexityScore = effectiveArea *
                                     lineFactor *
                                     shadeFactor *
                                     colorFactor *
                                     difficultyFactor;

            // Example baseline per-style:
            double baseAreaPerHour = input.Style switch
            {
                "Fine line" => 20.0,
                "Traditional" => 16.0,
                "Blackwork" => 14.0,
                "Color realism" => 10.0,
                _ => 15.0
            };

            double artistSpeedFactor = GetArtistSpeedFactor(input.ArtistId); // e.g. 0.9–1.3

            double hoursEstimate = (complexityScore / baseAreaPerHour) * artistSpeedFactor;
            double lowHours = hoursEstimate * 0.85;
            double highHours = hoursEstimate * 1.15;

            decimal hourlyRate = GetArtistHourlyRate(input.ArtistId);
            decimal shopMin = GetShopMinimum();

            decimal lowPrice = (decimal)lowHours * hourlyRate;
            decimal highPrice = (decimal)highHours * hourlyRate;

            if (lowPrice < shopMin) lowPrice = shopMin;
            if (highPrice < shopMin) highPrice = shopMin;

            decimal recommendedDeposit = Math.Max(shopMin, highPrice * 0.2m);

            return new QuoteEstimate
            {
                EstimatedHoursLow = lowHours,
                EstimatedHoursHigh = highHours,
                PriceLow = lowPrice,
                PriceHigh = highPrice,
                ShopMinimum = shopMin,
                RecommendedDeposit = recommendedDeposit,
                ConfidenceScore = 0.5, // Placeholder
                SimilarJobsCount = 0 // Placeholder
            };
        }

        private decimal GetArtistHourlyRate(int artistId)
        {
            var artist = _userRepository.GetUserById(artistId);
            // Assuming User model has a property for hourly rate.
            // If not, this will need to be added.
            // For now, let's use a default value if the user is not found.
            return artist?.HourlyRate ?? 150m;
        }

        private decimal GetShopMinimum()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            return (decimal)settings.ShopMinimumRate;
        }

        private double GetArtistSpeedFactor(int artistId)
        {
            var artist = _userRepository.GetUserById(artistId);
            // Assuming User model has a property for speed factor.
            // If not, this will need to be added.
            // For now, let's use a default value if the user is not found.
            return artist?.SpeedFactor ?? 1.0;
        }
    }
}
