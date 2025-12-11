using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "IsArtist")]
[Authorize(Policy = "IsArtist")]
public class ClientsController(IClientRepository clients, IWebHostEnvironment env) : ControllerBase
{
    private readonly IClientRepository _clients = clients;
    private readonly IWebHostEnvironment _env = env;

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

    [HttpPost("{id:int}/avatar")]
    public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var client = await _clients.GetByIdAsync(id);
        if (client == null) return NotFound();

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update client photo path (relative URL)
        var photoUrl = $"/uploads/avatars/{uniqueFileName}";
        client.PhotoPath = photoUrl;
        await _clients.UpdateAsync(client);

        return Ok(new { PhotoPath = photoUrl });
    }
}
