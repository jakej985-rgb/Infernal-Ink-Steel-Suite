using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Appointments;

public class IndexModel(ApiClient apiClient) : PageModel
{
    private readonly ApiClient _apiClient = apiClient;

    [BindProperty(SupportsGet = true)]
    public DateTime Day { get; set; } = DateTime.Today;

    public List<ApiClient.AppointmentDto> Appointments { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        // For now, we pull all appointments for the day, regardless of artist.
        Appointments = await _apiClient.GetAppointmentsAsync(Day, null);

        return Page();
    }
}
