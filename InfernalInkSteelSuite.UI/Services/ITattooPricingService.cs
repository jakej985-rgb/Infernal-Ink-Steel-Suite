using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.UI.Services
{
    public interface ITattooPricingService
    {
        QuoteEstimate GetEstimate(QuoteInput input);
    }
}
