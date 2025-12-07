using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "IsArtist")]
public class ClientsController(IClientRepository clients) : ControllerBase
{
    private readonly IClientRepository _clients = clients;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        var items = await _clients.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Client>> GetById(int id)
    {
        var client = await _clients.GetByIdAsync(id);
        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create([FromBody] Client client)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _clients.AddAsync(client);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Client client)
    {
        if (id != client.Id) return BadRequest("ID mismatch.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existing = await _clients.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _clients.UpdateAsync(client);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _clients.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _clients.DeleteAsync(id);
        return NoContent();
    }
}
