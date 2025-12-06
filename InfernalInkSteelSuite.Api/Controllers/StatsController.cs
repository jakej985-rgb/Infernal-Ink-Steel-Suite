using InfernalInkSteelSuite.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly StatsService _service;

    public StatsController(StatsService service)
    {
        _service = service;
    }

    [HttpGet("overview")]
    public IActionResult GetOverview()
    {
        return Ok(_service.GetOverview());
    }

    [HttpGet("appointments-by-day")]
    public IActionResult GetAppointmentsByDay([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var start = from ?? DateTime.UtcNow.AddDays(-7);
        var end = to ?? DateTime.UtcNow.AddDays(7);
        return Ok(_service.GetAppointmentsByDay(start, end));
    }
}
