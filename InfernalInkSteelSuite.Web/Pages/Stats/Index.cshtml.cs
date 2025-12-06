using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace InfernalInkSteelSuite.Web.Pages.Stats;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public ApiClient.DashboardStatsDto? Stats { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Account/Login");
        }

        var role = HttpContext.Session.GetString("Role");
        var isAdminOrManager = role == "Admin" || role == "Manager";

        if (!isAdminOrManager)
        {
            return RedirectToPage("/Dashboard/Index");
        }

        Stats = await _api.GetDashboardStatsAsync();
        return Page();
    }
}
