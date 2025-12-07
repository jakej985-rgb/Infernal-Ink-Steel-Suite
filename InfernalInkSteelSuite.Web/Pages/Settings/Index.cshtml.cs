using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Settings;

public class IndexModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    [BindProperty]
    public ApiClient.ShopSettingsDto Settings { get; set; } = new("Infernal Ink", 150m, 100m, 20.0, "Neon", "", "", "");

    // Theming
    [BindProperty]
    public string SelectedTheme { get; set; } = "Neon";

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin" && role != "Manager") return RedirectToPage("/Dashboard/Index");

        var settings = await _api.GetShopSettingsAsync();
        if (settings != null)
        {
            Settings = settings;
            SelectedTheme = settings.Theme;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

        // Update theme in settings object
        Settings = Settings with { Theme = SelectedTheme };

        var success = await _api.UpdateShopSettingsAsync(Settings);

        if (success)
            TempData["Message"] = "Settings saved successfully.";
        else
            TempData["Message"] = "Failed to save settings.";

        return Page();
    }
}
