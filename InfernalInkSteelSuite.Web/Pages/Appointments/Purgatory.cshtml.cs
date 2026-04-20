using InfernalInkSteelSuite.Web.Services;
using InfernalInkSteelSuite.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Appointments
{
    public class PurgatoryModel(ApiClient apiClient) : PageModel
    {
        private readonly ApiClient _apiClient = apiClient;

        public List<AppointmentDto> PendingAppts { get; set; } = [];
        public List<AppointmentDto> ConfirmedAppts { get; set; } = [];
        public List<AppointmentDto> CompletedAppts { get; set; } = [];

        public async Task OnGetAsync()
        {
            var all = await _apiClient.GetAppointmentsAsync();
            
            PendingAppts = [.. all.Where(a => a.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))];
            ConfirmedAppts = [.. all.Where(a => a.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))];
            CompletedAppts = [.. all.Where(a => a.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))];
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var appt = await _apiClient.GetAppointmentAsync(id);
            if (appt == null) return NotFound();

            appt = appt with { Status = status };
            await _apiClient.UpdateAppointmentAsync(appt);

            return new JsonResult(new { success = true });
        }
    }
}
