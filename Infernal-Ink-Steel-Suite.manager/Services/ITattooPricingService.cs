using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Services
{
    public interface ITattooPricingService
    {
        QuoteEstimate GetEstimate(QuoteInput input);
    }
}
