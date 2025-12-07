using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients;

public class IndexModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public List<ClientDto> Clients { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Account/Login");
        }

        var all = await _api.GetClientsAsync() ?? [];

        Clients = string.IsNullOrWhiteSpace(Search)
            ? all
            : all.Where(c => c.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();

        return Page();
    }
}
