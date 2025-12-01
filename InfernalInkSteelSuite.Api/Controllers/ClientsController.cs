using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfernalInkSteelSuite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "IsArtist")]
    public class ClientsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<List<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return client;
        }

        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> UpdateClient(int id, Client update)
        {
            if (id != update.Id) return BadRequest();

            var existing = await _context.Clients.FindAsync(id);
            if (existing is null) return NotFound();

            existing.FirstName = update.FirstName;
            existing.LastName = update.LastName;
            existing.Phone = update.Phone;
            existing.Email = update.Email;
            existing.Notes = update.Notes;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsAdmin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var existing = await _context.Clients.FindAsync(id);
            if (existing is null) return NotFound();

            _context.Clients.Remove(existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
