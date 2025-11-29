using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;

        public IEnumerable<Appointment> TodayAppointments { get; set; } = new List<Appointment>();
        public IEnumerable<Client> RecentClients { get; set; } = new List<Client>();
        public bool IsShopOpen { get; set; }
        public int ActiveArtistsCount { get; set; } = 3; // Placeholder logic for now

        public IndexModel(IAppointmentRepository appointmentRepository, IClientRepository clientRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _appointmentRepository = appointmentRepository;
            _clientRepository = clientRepository;
            _shopSettingsRepository = shopSettingsRepository;
        }

        public void OnGet()
        {
            var today = DateTime.Today;
            // Assuming GetAppointmentsByDateRange exists or similar
            TodayAppointments = _appointmentRepository.GetAppointmentsByDateRange(today, today.AddDays(1).AddTicks(-1));

            // Assuming GetAll exists, we'll take last 5. Real app should have GetRecent or similar.
            RecentClients = _clientRepository.GetAll().OrderByDescending(c => c.Id).Take(5);

            // Simple logic for shop status
            IsShopOpen = true;
        }
    }
}
