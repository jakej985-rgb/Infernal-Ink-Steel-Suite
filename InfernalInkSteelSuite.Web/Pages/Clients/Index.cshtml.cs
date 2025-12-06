using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public List<ClientDto> Clients { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Account/Login");
        }

        Clients = await _api.GetClientsAsync() ?? new List<ClientDto>();
        return Page();
    }
}
