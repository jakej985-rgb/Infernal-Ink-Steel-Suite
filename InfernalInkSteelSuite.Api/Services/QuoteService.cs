using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Api.Services;

public class QuoteService
{
    private readonly IQuoteRepository _repository;
    private readonly IShopSettingsRepository _shopSettingsRepository;

    public QuoteService(IQuoteRepository repository, IShopSettingsRepository shopSettingsRepository)
    {
        _repository = repository;
        _shopSettingsRepository = shopSettingsRepository;
    }

    public QuoteEstimate CalculateQuote(QuoteInput input)
    {
        // Simple logic ported from common tattoo shop practices
        // Price = (EstimatedHours * HourlyRate) + BasePrice

        // 1. Estimate Base Hours from Size and Coverage
        // Area in sq inches/cm. Let's assume input is in CM for calculation but the logic is abstract.
        double area = input.Width * input.Height;

        // Base time: 1 hour for setup/stencil + 0.1 hours per sq cm (very rough heuristic)
        // Adjust for coverage level (1-5)
        double baseHours = 0.5 + (area * 0.05 * (input.CoverageLevel / 3.0));

        // 2. Adjust for Complexity
        // Average complexity (1-5)
        double avgComplexity = (input.LineComplexity + input.ShadingComplexity + input.ColorComplexity + input.Difficulty) / 4.0;
        double complexityMultiplier = 0.8 + (avgComplexity * 0.4); // Ranges from 1.2 to 2.8

        double estimatedHours = baseHours * complexityMultiplier;

        if (input.IsCoverUp)
        {
            estimatedHours *= 1.5; // Coverups take longer
        }

        // 3. Get Shop Settings for Rate
        var settings = _shopSettingsRepository.LoadSettings();
        decimal hourlyRate = (decimal)(settings?.TattooPerHour ?? 150.0);
        decimal shopMinimum = (decimal)(settings?.ShopMinimumRate ?? 100.0);

        // 4. Calculate Price Range
        double variance = 0.2; // +/- 20%
        double hoursLow = Math.Round(estimatedHours * (1 - variance), 1);
        double hoursHigh = Math.Round(estimatedHours * (1 + variance), 1);

        decimal priceLow = (decimal)hoursLow * hourlyRate;
        decimal priceHigh = (decimal)hoursHigh * hourlyRate;

        // Enforce Minimum
        if (priceLow < shopMinimum) priceLow = shopMinimum;
        if (priceHigh < shopMinimum) priceHigh = shopMinimum;

        decimal deposit = priceLow * 0.2m; // 20% deposit

        return new QuoteEstimate
        {
            EstimatedHoursLow = hoursLow,
            EstimatedHoursHigh = hoursHigh,
            PriceLow = priceLow,
            PriceHigh = priceHigh,
            ShopMinimum = shopMinimum,
            RecommendedDeposit = Math.Round(deposit, 2),
            ConfidenceScore = 0.85, // Placeholder
            SimilarJobsCount = 0 // Placeholder
        };
    }

    public Quote CreateQuote(QuoteInput input, QuoteEstimate estimate)
    {
        var quote = new Quote
        {
            ClientId = input.ClientId,
            ArtistId = input.ArtistId,
            Placement = input.Placement,
            Style = input.Style,
            IsCoverUp = input.IsCoverUp,
            Width = input.Width,
            Height = input.Height,
            CoverageLevel = input.CoverageLevel,
            LineComplexity = input.LineComplexity,
            ShadingComplexity = input.ShadingComplexity,
            ColorComplexity = input.ColorComplexity,
            Difficulty = input.Difficulty,
            EstimatedHoursLow = estimate.EstimatedHoursLow,
            EstimatedHoursHigh = estimate.EstimatedHoursHigh,
            PriceLow = estimate.PriceLow,
            PriceHigh = estimate.PriceHigh,
            ShopMinimum = estimate.ShopMinimum,
            RecommendedDeposit = estimate.RecommendedDeposit,
            ConfidenceScore = estimate.ConfidenceScore,
            SimilarJobsCount = estimate.SimilarJobsCount,
            CreatedAt = DateTime.UtcNow
        };

        _repository.AddQuote(quote);
        return quote;
    }

    public List<Quote> GetAllQuotes()
    {
        return _repository.GetAllQuotes();
    }

    public Quote? GetQuote(int id)
    {
        // Repo doesn't have Get(id) yet?
        // Let's rely on GetAll and filter for now if strictly necessary, or add to repo.
        // Given I can edit repo, I should add it.
        // But to save time and stick to plan, I'll filter GetAll() for now as it returns a List in memory.
        return _repository.GetAllQuotes().FirstOrDefault(q => q.Id == id);
    }
}
