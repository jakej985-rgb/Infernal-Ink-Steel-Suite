using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApiClient _api;

        public AppointmentsController(ApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _api.GetAppointmentsAsync()
                                ?? new List<AppointmentDto>();
            return View(appointments);
        }
    }
}
