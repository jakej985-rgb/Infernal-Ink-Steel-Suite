using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace InfernalInkSteelSuite.Web.Pages.Settings;

public class IndexModel : PageModel
{
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

        return Page();
    }
}
