using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients
{
    public class CreateModel(IClientRepository clientRepository) : PageModel
    {
        private readonly IClientRepository _clientRepository = clientRepository;

        [BindProperty]
        public Client Client { get; set; } = new Client();

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _clientRepository.Insert(Client);

            return RedirectToPage("./Index");
        }
    }
}
