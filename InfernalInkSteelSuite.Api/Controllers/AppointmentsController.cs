using InfernalInkSteelSuite.Api.Dtos;
using InfernalInkSteelSuite.Api.Models;
using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfernalInkSteelSuite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "IsArtist")]
    public class AppointmentsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<List<AppointmentDto>>> GetAppointments([FromQuery] DateTime? date, [FromQuery] int? artistId)
        {
            var query = _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Artist)
                .AsQueryable();

            if (date.HasValue)
            {
                var dayStart = date.Value.Date;
                var dayEnd = dayStart.AddDays(1);
                query = query.Where(a => a.DateTime >= dayStart && a.DateTime < dayEnd);
            }

            var user = HttpContext.User;
            if (user.IsInRole(UserRole.Artist.ToString()))
            {
                var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                query = query.Where(a => a.UserId == userId);
            }
            else if (artistId.HasValue)
            {
                query = query.Where(a => a.UserId == artistId.Value);
            }

            var appointments = await query.ToListAsync();

            var results = appointments.Select(a =>
            {
                Enum.TryParse<AppointmentStatus>(a.Status, true, out var statusEnum);
                return new AppointmentDto(
                    a.Id,
                    a.ClientId,
                    a.ArtistId,
                    a.StartTime,
                    a.EndTime,
                    a.ServiceType,
                    a.ServiceCategory,
                    statusEnum,
                    a.QuotedPrice,
                    a.FinalPrice,
                    a.Notes,
                    new ClientDto(a.Client.Id, a.Client.FirstName, a.Client.LastName, a.Client.Phone, a.Client.Email),
                    a.Artist.Username
                );
            }).ToList();

            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, Appointment update)
        {
            if (id != update.Id) return BadRequest();

            var existing = await _context.Appointments.FindAsync(id);
            if (existing is null) return NotFound();

            existing.StartTime = update.StartTime;
            existing.EndTime = update.EndTime;
            existing.ServiceType = update.ServiceType;
            existing.ServiceCategory = update.ServiceCategory;
            existing.Status = update.Status;
            existing.QuotedPrice = update.QuotedPrice;
            existing.FinalPrice = update.FinalPrice;
            existing.Notes = update.Notes;
            existing.ClientId = update.ClientId;
            existing.ArtistId = update.ArtistId;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var existing = await _context.Appointments.FindAsync(id);
            if (existing is null) return NotFound();

            _context.Appointments.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
