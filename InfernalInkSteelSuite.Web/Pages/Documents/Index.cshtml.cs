using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InfernalInkSteelSuite.Web.Pages.Documents;

public class IndexModel : PageModel
{
    private readonly ApiClient _api;

    public IndexModel(ApiClient api)
    {
        _api = api;
    }

    public List<ApiClient.DocumentDto> Documents { get; set; } = new();

    public async Task OnGetAsync()
    {
        Documents = await _api.GetDocumentsAsync() ?? new List<ApiClient.DocumentDto>();
    }
}
