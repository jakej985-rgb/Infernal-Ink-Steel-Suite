using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Dashboard
{
    public class IndexModel(IAppointmentRepository appointmentRepository, IClientRepository clientRepository) : PageModel
    {
        public IEnumerable<Appointment> TodayAppointments { get; set; } = [];
        public IEnumerable<Client> RecentClients { get; set; } = [];
        public bool IsShopOpen { get; set; }
        public int ActiveArtistsCount { get; set; } = 3; // Placeholder logic for now.

        public void OnGet()
        {
            var today = DateTime.Today;
            // Assuming GetAppointmentsByDateRange exists or similar
            TodayAppointments = appointmentRepository.GetAppointmentsByDateRange(today, today.AddDays(1).AddTicks(-1));

            // Assuming GetAll exists, we'll take last 5. Real app should have GetRecent or similar.
            RecentClients = clientRepository.GetAll().OrderByDescending(c => c.Id).Take(5);

            // Simple logic for shop status
            IsShopOpen = true;
        }
    }
}
