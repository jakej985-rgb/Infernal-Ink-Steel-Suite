using System.ComponentModel.DataAnnotations;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Account;

public class LoginModel(ApiClient apiClient) : PageModel
{
    private readonly ApiClient _apiClient = apiClient;

    [BindProperty]
    [Required]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _apiClient.LoginAsync(Username, Password);
        if (result is null)
        {
            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        // store basic info in session
        HttpContext.Session.SetInt32("UserId", result.UserId);
        HttpContext.Session.SetString("Username", result.Username);
        HttpContext.Session.SetString("DisplayName", result.DisplayName);
        HttpContext.Session.SetString("Role", result.Role);
        HttpContext.Session.SetString("Token", result.Token);

        return RedirectToPage("/Dashboard/Index");
    }
}
