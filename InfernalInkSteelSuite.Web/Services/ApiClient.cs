using InfernalInkSteelSuite.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;

namespace InfernalInkSteelSuite.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;

        // Add the JWT token to the request headers for every request
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    // DTOs matching the API responses
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(int UserId, string Username, string DisplayName, string Role, string Token);

    public record DocumentDto(
        int Id,
        int ClientId,
        int UploadedByUserId,
        string Title,
        string FilePath,
        DateTime CreatedAt
    );

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        var resp = await _http.PostAsJsonAsync("/auth/login", new LoginRequest(username, password));
        if (!resp.IsSuccessStatusCode) return null;

        return await resp.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<List<ClientDto>> GetClientsAsync()
    {
        var result = await _http.GetFromJsonAsync<List<ClientDto>>("/clients");
        return result ?? [];
    }

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        return await _http.GetFromJsonAsync<ClientDto>($"/clients/{id}");
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync(DateTime? date = null, int? artistId = null)
    {
        var query = new List<string>();
        if (date.HasValue) query.Add($"date={date.Value:O}");
        if (artistId.HasValue) query.Add($"artistId={artistId.Value}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

        var result = await _http.GetFromJsonAsync<List<AppointmentDto>>($"/appointments{qs}");
        return result ?? [];
    }

    public async Task<List<DocumentDto>> GetDocumentsForClientAsync(int clientId)
    {
        var result = await _http.GetFromJsonAsync<List<DocumentDto>>($"/documents/by-client/{clientId}");
        return result ?? [];
    }

    public async Task<DocumentDto?> UploadDocumentAsync(
        int clientId,
        int uploadedByUserId,
        string? title,
        IFormFile file)
    {
        using var content = new MultipartFormDataContent
        {
            { new StringContent(clientId.ToString()), "clientId" },
            { new StringContent(uploadedByUserId.ToString()), "uploadedByUserId" }
        };

        if (!string.IsNullOrWhiteSpace(title))
            content.Add(new StringContent(title), "title");

        await using var stream = file.OpenReadStream();
        var streamContent = new StreamContent(stream);
        content.Add(streamContent, "file", file.FileName);

        var response = await _http.PostAsync("/documents", content);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<DocumentDto>();
    }
}
