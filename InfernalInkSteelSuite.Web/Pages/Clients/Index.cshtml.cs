using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients;

public class IndexModel(ApiClient apiClient) : PageModel
{
    private readonly ApiClient _apiClient = apiClient;

    public List<ApiClient.ClientDto> Clients { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        Clients = await _apiClient.GetClientsAsync();
        return Page();
    }
}
