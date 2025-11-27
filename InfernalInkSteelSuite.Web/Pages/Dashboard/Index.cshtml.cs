using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Dashboard;

public class IndexModel : PageModel
{
    public string DisplayName { get; set; } = "Unknown User";

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        DisplayName = HttpContext.Session.GetString("DisplayName") ?? "User";
        return Page();
    }
}
