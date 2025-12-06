using InfernalInkSteelSuite.Api.Dtos;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InfernalInkSteelSuite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "IsArtist")]
    public class AppointmentsController(IAppointmentRepository appointments) : ControllerBase
    {
        private readonly IAppointmentRepository _appointments = appointments;

        [HttpGet]
        public ActionResult<List<AppointmentDto>> GetAppointments([FromQuery] DateTime? date, [FromQuery] int? artistId)
        {
            List<Appointment> appointments;

            if (date.HasValue)
            {
                appointments = _appointments.GetAppointmentsByDate(date.Value);
            }
            else if (artistId.HasValue)
            {
                appointments = _appointments.GetAppointmentsByUserId(artistId.Value);
            }
            else
            {
                appointments = _appointments.GetAll();
            }

            var user = HttpContext.User;
            if (user.IsInRole(UserRole.Artist.ToString()) && !user.IsInRole(UserRole.Admin.ToString()))
            {
                var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (artistId.HasValue && artistId.Value != userId)
                {
                    return Forbid();
                }

                if (!artistId.HasValue)
                {
                    appointments = appointments.Where(a => a.UserId == userId).ToList();
                }
            }

            var results = appointments.Select(a =>
            {
                Enum.TryParse<AppointmentStatus>(a.Status, true, out var statusEnum);

                ClientDto? clientDto = null;
                if (a.Client != null)
                {
                    clientDto = new ClientDto(a.Client.Id, a.Client.FirstName, a.Client.LastName, a.Client.Phone, a.Client.Email);
                }
                else
                {
                    // Fallback or empty if needed
                }

                return new AppointmentDto(
                    a.Id,
                    a.ClientId,
                    a.UserId,
                    a.StartTime,
                    a.EndTime,
                    a.ServiceType,
                    a.ServiceCategory,
                    statusEnum,
                    a.QuotedPrice,
                    a.FinalPrice,
                    a.Notes,
                    clientDto,
                    a.Artist?.Username ?? ""
                );
            }).ToList();

            return Ok(results);
        }

        [HttpGet("{id:int}")]
        public ActionResult<Appointment> GetById(int id)
        {
            var item = _appointments.Get(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public ActionResult<Appointment> Create([FromBody] Appointment appointment)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _appointments.Add(appointment);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.Id) return BadRequest("ID mismatch.");

            var existing = _appointments.Get(id);
            if (existing == null) return NotFound();

            _appointments.Update(appointment);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "IsAdmin")]
        public IActionResult Delete(int id)
        {
            var existing = _appointments.Get(id);
            if (existing == null) return NotFound();

            _appointments.Delete(id);
            return NoContent();
        }
    }
}
