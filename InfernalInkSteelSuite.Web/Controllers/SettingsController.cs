using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Web.Controllers;

public class SettingsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
