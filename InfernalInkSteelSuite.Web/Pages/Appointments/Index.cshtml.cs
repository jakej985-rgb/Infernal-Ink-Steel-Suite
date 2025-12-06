using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Appointments;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public List<AppointmentDto> Appointments { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateOnly? Date { get; set; }

    public async Task OnGetAsync()
    {
        var dateToUse = Date ?? DateOnly.FromDateTime(DateTime.Today);
        // Note: The ApiClient accepts DateTime?, but we want to work with DateOnly for the UI.
        // We pass the DateTime equivalent.
        Appointments = await _api.GetAppointmentsAsync(dateToUse.ToDateTime(TimeOnly.MinValue)) ?? new List<AppointmentDto>();
    }
}
