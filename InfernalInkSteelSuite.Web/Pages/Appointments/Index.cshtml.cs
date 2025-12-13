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
    public List<AppointmentDto> WaitlistAppointments { get; set; } = [];
    public Dictionary<DateOnly, int> AppointmentCounts { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Account/Login");

        if (string.IsNullOrEmpty(Date) || !DateOnly.TryParse(Date, out var parsedDate))
        {
            SelectedDate = DateOnly.FromDateTime(DateTime.Today);
        }
        else
        {
            SelectedDate = parsedDate;
        }

        // Fetch ALL appointments to populate the Calendar Heatmap & Filter locally
        // This avoids multiple API calls and enables the heatmap counts.
        var allAppointments = await _api.GetAppointmentsAsync();

        // 1. Populate Calendar Counts
        AppointmentCounts = allAppointments
            .GroupBy(a => DateOnly.FromDateTime(a.StartTime))
            .ToDictionary(g => g.Key, g => g.Count());

        // 2. Filter for Main Stage (Selected Date)
        Appointments = allAppointments
            .Where(a => DateOnly.FromDateTime(a.StartTime) == SelectedDate)
            .OrderBy(a => a.StartTime)
            .ToList();

        // 3. Filter Waitlist (Pending status)
        WaitlistAppointments = allAppointments
            .Where(a => a.Status == "Pending")
            .OrderBy(a => a.StartTime)
            .ToList();

        return Page();
    }
}
