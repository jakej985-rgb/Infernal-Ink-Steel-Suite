using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace InfernalInkSteelSuite.Web.Pages.Settings;

public class IndexModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        var role = HttpContext.Session.GetString("Role");
        var isAdminOrManager = role == "Admin" || role == "Manager";

        if (!isAdminOrManager)
        {
            return RedirectToPage("/Dashboard/Index");
        }

        return Page();
    }
}
