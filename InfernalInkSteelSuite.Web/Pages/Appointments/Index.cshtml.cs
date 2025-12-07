using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace InfernalInkSteelSuite.Web.Pages.Appointments;

public class IndexModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    [BindProperty(SupportsGet = true)]
    public string? Date { get; set; }

    public DateOnly SelectedDate { get; set; }
    public List<AppointmentDto> Appointments { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Account/Login");

        if (string.IsNullOrEmpty(Date) || !DateOnly.TryParse(Date, out var parsedDate))
        {
            SelectedDate = DateOnly.FromDateTime(DateTime.Today);
            // Optionally redirect to include nice URL, but keeping simple for now
        }
        else
        {
            SelectedDate = parsedDate;
        }

        // Pass DateTime to API (start of day)
        Appointments = await _api.GetAppointmentsAsync(date: SelectedDate.ToDateTime(TimeOnly.MinValue));

        // Sort by time
        Appointments = [.. Appointments.OrderBy(a => a.StartTime)];

        return Page();
    }
}
