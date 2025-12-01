using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfernalInkSteelSuite.Web.Pages.Appointments
{
    public class EditModel(IAppointmentRepository appointmentRepository, IClientRepository clientRepository) : PageModel
    {
        private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
        private readonly IClientRepository _clientRepository = clientRepository;

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;

        public SelectList ClientOptions { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = _appointmentRepository.Get(id.Value);
            if (appointment == null)
            {
                return NotFound();
            }
            Appointment = appointment;

            var clients = _clientRepository.GetAll();
            ClientOptions = new SelectList(clients, "Id", "FullName");

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

            // Ensure UserId is preserved if not bound (it should be bound if in form, but safer to re-fetch or keep hidden)
            // Actually, if we bind the whole object, UserId might be lost if not in form. 
            // Better to fetch original, update fields, and save. Or include UserId in hidden field.
            // I'll include UserId in hidden field in the View.

            _appointmentRepository.Update(Appointment);

            return RedirectToPage("./Index");
        }
    }
}
