using System.ComponentModel.DataAnnotations;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients;

public class DocumentsModel(ApiClient apiClient) : PageModel
{
    private readonly ApiClient _apiClient = apiClient;

    [BindProperty(SupportsGet = true)]
    public int ClientId { get; set; }

    public ApiClient.ClientDto? Client { get; set; }

    public List<ApiClient.DocumentDto> Documents { get; set; } = [];

    [BindProperty]
    [Display(Name = "File")]
    public IFormFile? UploadFile { get; set; }

    [BindProperty]
    [Display(Name = "Title")]
    public string? Title { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        if (ClientId <= 0)
            return BadRequest();

        await LoadClientAndDocs();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null)
            return RedirectToPage("/Account/Login");

        if (ClientId <= 0)
            return BadRequest();

        if (UploadFile is null || UploadFile.Length == 0)
        {
            ErrorMessage = "Please select a file to upload.";
            await LoadClientAndDocs();
            return Page();
        }

        var doc = await _apiClient.UploadDocumentAsync(
            ClientId,
            userId.Value,
            string.IsNullOrWhiteSpace(Title) ? UploadFile.FileName : Title,
            UploadFile);

        if (doc is null)
        {
            ErrorMessage = "Upload failed.";
            await LoadClientAndDocs();
            return Page();
        }

        return RedirectToPage(new { ClientId });
    }

    private async Task LoadClientAndDocs()
    {
        Client = await _apiClient.GetClientAsync(ClientId);
        Documents = await _apiClient.GetDocumentsForClientAsync(ClientId);
    }
}
