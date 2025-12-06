using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Quotes;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public List<ApiClient.QuoteDto> Quotes { get; set; } = new();

    public async Task OnGetAsync()
    {
        Quotes = await _api.GetAllQuotesAsync() ?? new List<ApiClient.QuoteDto>();
    }
}
