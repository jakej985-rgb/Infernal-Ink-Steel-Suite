using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApiClient _api;

        public ClientsController(ApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _api.GetClientsAsync()
                          ?? new List<ClientDto>();
            return View(clients);
        }
    }
}
