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

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Account/Login");

        if (id.HasValue && id.Value > 0)
        {
            var existing = await _api.GetClientAsync(id.Value);
            if (existing != null)
            {
                Client = existing;
                Title = $"Edit Client: {existing.FullName}";
            }
            else
            {
                return RedirectToPage("/Clients/Index");
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var token = HttpContext.Session.GetString("ApiToken");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Determine if Create or Update
        // Note: The ApiClient currently only shows GetClientsAsync and GetClientAsync in the visible view.
        // We likely need CreateClientAsync and UpdateClientAsync methods.
        // I will assume they might exist or I need to use generic HTTP calls if they are missing from the wrapper.
        // However, the plan assumed using ApiClient.
        // If ApiClient doesn't have Create/Update for Client, I might need to add them or use _http directly if exposed.
        // Wait, ApiClient.cs view didn't show CreateClientAsync.
        // I should check if I missed them or if I need to add them to ApiClient.cs.
        // For now, I'll generate the code assuming they exist or I'll add them in the next step if I recall they were missing.
        // Actually, looking back at ApiClient.cs in Step 15 lines 129-140, strictly Getters are shown for Clients.
        // Line 129: GetClientsAsync
        // Line 136: GetClientAsync
        // No Create or Update. I must add them to ApiClient.cs as well.

        // I will write the code here assuming the methods `CreateClientAsync` and `UpdateClientAsync` exist,
        // and then I will immediately go update ApiClient.cs to add them.

        bool success;
        if (Client.Id > 0)
        {
            // Update
            success = await _api.UpdateClientAsync(Client);
        }
        else
        {
            // Create
            var created = await _api.CreateClientAsync(Client);
            success = created != null;
        }

        if (!success)
        {
            ErrorMessage = "Failed to save client. Please try again.";
            return Page();
        }

        return RedirectToPage("/Clients/Index");
    }
}
