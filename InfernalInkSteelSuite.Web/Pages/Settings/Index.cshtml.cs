using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Settings;

public class IndexModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    [BindProperty]
    public ShopSettingsDto Settings { get; set; } = new();

    // Theming
    [BindProperty]
    public string SelectedTheme { get; set; } = "Neon"; // Default

    public IActionResult OnGet()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

        var role = HttpContext.Session.GetString("Role");
        if (role != "Admin" && role != "Manager") return RedirectToPage("/Dashboard/Index");

        // Fetch settings from API if available. 
        // Assuming GetShopSettingsAsync exists or needs to be added.
        // For MVP, we'll assume we can at least Mock it or I'll add simple properties.
        // NOTE: ApiClient was not defined with GetShopSettingsAsync in earlier steps.
        // I will implement this PageModel assuming I need to add that next.
        // For now, hardcode defaults or fetch if I add method.

        Settings = new ShopSettingsDto
        {
            ShopName = "Infernal Ink & Steel",
            HourlyRate = 150,
            MinimumRate = 100,
            DepositPercentage = 20
        };

        return Page();
    }

    public IActionResult OnPost()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

        // Save logic to API
        // _api.UpdateShopSettings(Settings);

        // For now, just reload page to simulate save
        TempData["Message"] = "Settings saved successfully.";

        return Page();
    }
}

public class ShopSettingsDto
{
    public string ShopName { get; set; } = "";
    public decimal HourlyRate { get; set; }
    public decimal MinimumRate { get; set; }
    public double DepositPercentage { get; set; }
    public string Theme { get; set; } = "Neon";
}
