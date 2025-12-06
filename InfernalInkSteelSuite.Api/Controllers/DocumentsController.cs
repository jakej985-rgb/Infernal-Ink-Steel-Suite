using InfernalInkSteelSuite.Api.DTOs;
using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly DocumentService _service;

    public DocumentsController(DocumentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] int clientId, [FromForm] int uploadedByUserId, [FromForm] string? title, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
            var document = await _service.UploadDocumentAsync(clientId, uploadedByUserId, title ?? "", file);
            var dto = new DocumentDto(document.Id, document.ClientId, document.UploadedByUserId, document.Title, document.FilePath, document.CreatedAt);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public ActionResult<List<DocumentDto>> GetAll()
    {
        var docs = _service.GetAllDocuments();
        var dtos = docs.Select(d => new DocumentDto(
            d.Id,
            d.ClientId,
            d.UploadedByUserId,
            d.Title,
            d.FilePath,
            d.CreatedAt
        )).ToList();
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetMetadata(int id)
    {
        var doc = _service.GetMetadata(id);
        if (doc == null) return NotFound();
        return Ok(doc);
    }

    [HttpGet("{id}/file")]
    public IActionResult Download(int id)
    {
        var (stream, contentType, fileName) = _service.GetFile(id);
        if (stream == null) return NotFound();

        return File(stream, contentType, fileName);
    }

    [HttpGet("by-client/{clientId}")]
    public IActionResult GetByClient(int clientId)
    {
        var docs = _service.GetDocumentsForClient(clientId);
        return Ok(docs);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.DeleteDocument(id);
        return NoContent();
    }
}
