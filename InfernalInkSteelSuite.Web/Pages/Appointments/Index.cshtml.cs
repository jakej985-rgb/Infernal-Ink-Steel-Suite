using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Appointments;

public class IndexModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    public List<AppointmentDto> Appointments { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public DateOnly? Date { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Account/Login");
        }

        var dateToUse = Date ?? DateOnly.FromDateTime(DateTime.Today);
        // Note: The ApiClient accepts DateTime?, but we want to work with DateOnly for the UI.
        // We pass the DateTime equivalent.
        Appointments = await _api.GetAppointmentsAsync(dateToUse.ToDateTime(TimeOnly.MinValue)) ?? [];
        return Page();
    }
}
