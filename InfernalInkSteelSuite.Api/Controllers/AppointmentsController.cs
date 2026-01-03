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
        public ActionResult<List<AppointmentDto>> GetAppointments([FromQuery] DateTime? date, [FromQuery] int? artistId, [FromQuery] int? clientId)
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
                    appointments = [.. appointments.Where(a => a.UserId == userId)];
                }
            }

            // Filter by Status if provided explicitly (e.g. for Purgatory/Waitlist)
            string? status = HttpContext.Request.Query["status"];
            if (!string.IsNullOrEmpty(status))
            {
                appointments = [.. appointments.Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase))];
            }

            var results = appointments.Select(a =>
            {
                ClientDto? clientDto = null;
                if (a.Client != null)
                {
                    clientDto = new ClientDto(a.Client.Id, a.Client.FirstName, a.Client.LastName, a.Client.Phone, a.Client.Email);
                }

                return new AppointmentDto(
                    a.Id,
                    a.ClientId,
                    a.UserId,
                    a.StartTime,
                    a.EndTime,
                    a.ServiceType,
                    a.ServiceCategory,
                    a.Status, // Pass string directly
                    a.QuotedPrice,
                    a.FinalPrice,
                    a.Notes,
                    clientDto,
                    a.Artist?.Username ?? "",
                    a.PriceType,
                    a.PriceCharged,
                    a.Color,
                    a.IsBlockOff
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
        public ActionResult<Appointment> Create([FromBody] AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appointment = new Appointment();
            MapToDomain(appointmentDto, appointment);

            _appointments.Add(appointment);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] AppointmentDto appointmentDto)
        {
            if (id != appointmentDto.Id) return BadRequest("ID mismatch.");

            var existing = _appointments.Get(id);
            if (existing == null) return NotFound();

            MapToDomain(appointmentDto, existing);
            // Ensure Id is preserved (though MapToDomain shouldn't touch it, safety first)
            existing.Id = id;

            _appointments.Update(existing);
            return NoContent();
        }

        private static void MapToDomain(AppointmentDto dto, Appointment entity)
        {
            entity.ClientId = dto.ClientId;
            // Explicitly map ArtistId to UserId
            entity.UserId = dto.ArtistId;
            entity.DateTime = dto.StartTime;

            // Calculate duration
            if (dto.EndTime > dto.StartTime)
            {
                entity.DurationMinutes = (int)(dto.EndTime - dto.StartTime).TotalMinutes;
            }
            else
            {
                // Default if invalid
                entity.DurationMinutes = 60;
            }

            entity.ServiceType = dto.ServiceType ?? "Tattoo";
            entity.ServiceCategory = dto.ServiceCategory ?? "General";
            entity.Notes = dto.Notes ?? string.Empty;

            // Map Status (use string directly, default if empty)
            entity.Status = string.IsNullOrWhiteSpace(dto.Status) ? "Scheduled" : dto.Status;

            // Price fields
            entity.QuotedPrice = dto.QuotedPrice;
            entity.FinalPrice = dto.FinalPrice;

            // Mapped fields
            entity.PriceType = string.IsNullOrWhiteSpace(dto.PriceType) ? "Hourly" : dto.PriceType;
            entity.PriceCharged = dto.PriceCharged;
            entity.Color = dto.Color ?? string.Empty;
            entity.IsBlockOff = dto.IsBlockOff;

            // If new (Id=0), set some defaults if needed
            if (entity.Id == 0)
            {
                if (string.IsNullOrEmpty(entity.PriceType)) entity.PriceType = "Hourly";
            }
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
