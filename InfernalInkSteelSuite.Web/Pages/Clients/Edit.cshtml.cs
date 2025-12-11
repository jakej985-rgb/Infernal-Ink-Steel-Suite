using InfernalInkSteelSuite.Web.Models;
using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Clients;

public class EditModel(ApiClient api) : PageModel
{
    private readonly ApiClient _api = api;

    [BindProperty]
    public ClientDto Client { get; set; } = new();

    public string Title { get; set; } = "New Client";
    public string? ErrorMessage { get; set; }

    public List<ApiClient.DocumentDto> Documents { get; set; } = [];

    [BindProperty]
    public string? UploadTitle { get; set; }

    [BindProperty]
    public string? AvatarHtml { get; set; } // Legacy or unused?

    [BindProperty]
    public string? AvatarBase64 { get; set; }

    [BindProperty]
    public IFormFile? Upload { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Account/Login");


        if (id.HasValue && id.Value > 0)
        {
            // Update mode
            var existing = await _api.GetClientAsync(id.Value);
            if (existing == null) return RedirectToPage("/Clients/Index");
            Client = existing;
            Title = "Edit Client";

            // Load documents
            Documents = await _api.GetDocumentsForClientAsync(id.Value);
        }
        else
        {
            // Create mode
            // No documents for new client
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid) return Page();

        try
        {
            ClientDto savedClient;
            if (Client.Id > 0)
            {
                await _api.UpdateClientAsync(Client);
                savedClient = Client;
            }
            else
            {
                savedClient = await _api.CreateClientAsync(Client);
            }

            // Handle avatar upload if present
            if (!string.IsNullOrEmpty(AvatarBase64))
            {
                // Format: "data:image/jpeg;base64,....."
                var parts = AvatarBase64.Split(',');
                var base64 = parts.Length > 1 ? parts[1] : parts[0];
                var bytes = Convert.FromBase64String(base64);

                using var stream = new MemoryStream(bytes);
                await _api.UploadAvatarAsync(savedClient.Id, stream, "avatar.jpg");
            }

            if (Client.Id == 0) return RedirectToPage("/Clients/Edit", new { id = savedClient.Id });
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Content; // Or parse it if JSON
            ModelState.AddModelError("", $"API Error: {ex.Content}");
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error occurred.";
            ModelState.AddModelError("", $"Error: {ex.Message}");
            return Page();
        }

        return RedirectToPage("/Clients/Index");
    }

    public async Task<IActionResult> OnPostUploadAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token)) return RedirectToPage("/Account/Login");

        if (Upload == null || Client.Id == 0)
        {
            ErrorMessage = "Please select a file and ensure client is saved.";
            // Reload client and docs
            Client = await _api.GetClientAsync(Client.Id) ?? new();
            Documents = await _api.GetDocumentsForClientAsync(Client.Id);
            return Page();
        }

        var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        await _api.UploadDocumentAsync(Client.Id, userId, UploadTitle, Upload);

        return RedirectToPage("/Clients/Edit", new { id = Client.Id });
    }
}
