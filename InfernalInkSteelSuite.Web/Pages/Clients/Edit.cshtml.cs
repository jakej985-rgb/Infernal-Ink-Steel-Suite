using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients
{
    public class EditModel(IClientRepository clientRepository) : PageModel
    {
        private readonly IClientRepository _clientRepository = clientRepository;

        [BindProperty]
        public Client Client { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = _clientRepository.Get(id.Value);
            if (client == null)
            {
                return NotFound();
            }
            Client = client;
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _clientRepository.Update(Client);

            return RedirectToPage("./Index");
        }
    }
}
