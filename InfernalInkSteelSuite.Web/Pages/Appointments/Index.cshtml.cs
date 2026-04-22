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

        // 1. Fetch Heatmap Counts (H1 Fix: Use optimized endpoint)
        var heatmap = await _api.GetAppointmentHeatmapAsync();
        AppointmentCounts = heatmap.ToDictionary(
            kvp => DateOnly.ParseExact(kvp.Key, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            kvp => kvp.Value
        );

        // 2. Fetch Appointments for the Selected Date (Filtered API Call)
        Appointments = await _api.GetAppointmentsAsync(date: SelectedDate.ToDateTime(TimeOnly.MinValue));

        // 3. Fetch Waitlist (Pending status)
        WaitlistAppointments = await _api.GetAppointmentsAsync(status: "Pending");

        return Page();
    }
    public async Task<IActionResult> OnGetDetailsPartialAsync(int id)
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return Unauthorized();

        var appt = await _api.GetAppointmentAsync(id);
        if (appt == null) return NotFound();

        List<AppointmentDto> history = [];
        if (appt.ClientId > 0)
        {
            // Pass clientId to GetAppointmentsAsync to filter
            history = await _api.GetAppointmentsAsync(clientId: appt.ClientId);
        }

        var viewModel = new AppointmentDetailsViewModel
        {
            CurrentAppointment = appt,
            PastSessions = history,
            AvailableQuotes = []
        };

        if (appt.ClientId > 0)
        {
            var quotes = await _api.GetAllQuotesAsync();
            viewModel.AvailableQuotes = quotes.Where(q => q.ClientId == appt.ClientId).OrderByDescending(q => q.CreatedAt).ToList();
        }

        return Partial("_AppointmentDetailsModal", viewModel);
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return Unauthorized();

        var appt = await _api.GetAppointmentAsync(id);
        if (appt == null) return NotFound();

        var updateDto = appt with { Status = "Completed" };

        try
        {
            await _api.UpdateAppointmentAsync(updateDto);
            return new OkResult();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    public async Task<IActionResult> OnPostUpdateFinancialsAsync(int id, decimal quotedPrice)
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return Unauthorized();

        var appt = await _api.GetAppointmentAsync(id);
        if (appt == null) return NotFound();

        var updateDto = appt with { QuotedPrice = quotedPrice };

        try
        {
            await _api.UpdateAppointmentAsync(updateDto);
            return new OkResult();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}
