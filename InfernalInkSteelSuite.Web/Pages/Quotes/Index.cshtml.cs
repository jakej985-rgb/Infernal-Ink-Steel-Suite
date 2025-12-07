using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Quotes;

public class IndexModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    public List<ApiClient.QuoteDto> Quotes { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Account/Login");
        }

        Quotes = await _api.GetAllQuotesAsync() ?? [];
        return Page();
    }
}
