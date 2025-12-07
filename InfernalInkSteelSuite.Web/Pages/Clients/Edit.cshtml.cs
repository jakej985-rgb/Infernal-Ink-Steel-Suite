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
    public string UploadTitle { get; set; } = "";

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

        bool success;
        if (Client.Id > 0)
        {
            success = await _api.UpdateClientAsync(Client);
        }
        else
        {
            var created = await _api.CreateClientAsync(Client);
            if (created != null)
            {
                // Redirect to edit page so they can upload docs if they want, or index
                return RedirectToPage("/Clients/Edit", new { id = created.Id });
            }
            success = false;
        }

        if (!success)
        {
            ErrorMessage = "Failed to save client.";
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
