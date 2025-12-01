using System.ComponentModel.DataAnnotations;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients;

public class DocumentsModel(IDocumentRepository documentRepository, IClientRepository clientRepository, IWebHostEnvironment environment) : PageModel
{
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly IWebHostEnvironment _environment = environment;

    [BindProperty(SupportsGet = true)]
    public int ClientId { get; set; }

    public Client? Client { get; set; }

    public List<Document> Documents { get; set; } = [];

    [BindProperty]
    [Display(Name = "File")]
    public IFormFile? UploadFile { get; set; }

    [BindProperty]
    [Display(Name = "Title")]
    public string? Title { get; set; }

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        if (ClientId <= 0)
            return BadRequest();

        LoadClientAndDocs();
        if (Client == null) return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        if (ClientId <= 0)
            return BadRequest();

        LoadClientAndDocs();
        if (Client == null) return NotFound();

        if (UploadFile is null || UploadFile.Length == 0)
        {
            ErrorMessage = "Please select a file to upload.";
            return Page();
        }

        // Save file
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", ClientId.ToString());
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + UploadFile.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await UploadFile.CopyToAsync(fileStream);
        }

        // Create Document Record
        var doc = new Document
        {
            ClientId = ClientId,
            UploadedByUserId = userId.Value,
            Title = string.IsNullOrWhiteSpace(Title) ? UploadFile.FileName : Title,
            FilePath = Path.Combine("uploads", ClientId.ToString(), uniqueFileName),
            CreatedAt = DateTime.UtcNow
        };

        _documentRepository.Insert(doc);

        return RedirectToPage(new { ClientId });
    }

    public IActionResult OnGetDownload(int id)
    {
        var doc = _documentRepository.Get(id);
        if (doc == null) return NotFound();

        var filePath = Path.Combine(_environment.WebRootPath, doc.FilePath);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        return PhysicalFile(filePath, "application/octet-stream", doc.Title + Path.GetExtension(doc.FilePath));
    }

    private void LoadClientAndDocs()
    {
        Client = _clientRepository.Get(ClientId);
        // Assuming GetAll() returns all, we filter here or add GetByClientId to Repo. 
        // IDocumentRepository doesn't have GetByClientId in the interface I saw earlier?
        // Let's check. It had GetDocuments(userId, role...). 
        // I might need to filter GetAll() or add a method. 
        // For now, I'll filter GetAll().
        if (Client != null)
        {
            Documents = [.. _documentRepository.GetAll().Where(d => d.ClientId == ClientId)];
        }
    }
}
