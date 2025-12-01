using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfernalInkSteelSuite.Web.Pages.Appointments
{
    public class CreateModel(IAppointmentRepository appointmentRepository, IClientRepository clientRepository) : PageModel
    {
        private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
        private readonly IClientRepository _clientRepository = clientRepository;

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        public SelectList ClientOptions { get; set; } = default!;

        public IActionResult OnGet()
        {
            var clients = _clientRepository.GetAll();
            ClientOptions = new SelectList(clients, "Id", "FullName");

            // Default values
            Appointment.DateTime = DateTime.Now.Date.AddHours(12);
            Appointment.DurationMinutes = 60;
            Appointment.Status = "Scheduled";

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                var clients = _clientRepository.GetAll();
                ClientOptions = new SelectList(clients, "Id", "FullName");
                return Page();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            Appointment.UserId = userId.Value;

            // Ensure SyncId is set if not already (it has default but good to be safe)
            if (Appointment.SyncId == Guid.Empty) Appointment.SyncId = Guid.NewGuid();

            _appointmentRepository.Add(Appointment);

            return RedirectToPage("./Index");
        }
    }
}
