using InfernalInkSteelSuite.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InfernalInkSteelSuite.Web.Services;

public class ApiOptions
{
    public string ApiBaseUrl { get; set; } = "";
}

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApiOptions _options;

    public ApiClient(HttpClient http, IOptions<ApiOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _httpContextAccessor = httpContextAccessor;
        _options = options.Value;

        if (!string.IsNullOrWhiteSpace(_options.ApiBaseUrl))
        {
            _http.BaseAddress = new Uri(_options.ApiBaseUrl);
        }
    }

    private void ApplyAuthHeader()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _http.DefaultRequestHeaders.Authorization = null;
            return;
        }

        var token = httpContext.Session.GetString("ApiToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _http.DefaultRequestHeaders.Authorization = null;
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
        ApplyAuthHeader();
        var result = await _http.GetFromJsonAsync<List<ClientDto>>("api/clients");
        return result ?? [];
    }

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<ClientDto>($"api/clients/{id}");
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync(DateTime? date = null, int? artistId = null)
    {
        ApplyAuthHeader();
        var query = new List<string>();
        if (date.HasValue) query.Add($"date={date.Value:O}");
        if (artistId.HasValue) query.Add($"artistId={artistId.Value}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

        var result = await _http.GetFromJsonAsync<List<AppointmentDto>>($"/appointments{qs}");
        return result ?? [];
    }

    public async Task<List<DocumentDto>> GetDocumentsForClientAsync(int clientId)
    {
        ApplyAuthHeader();
        var result = await _http.GetFromJsonAsync<List<DocumentDto>>($"/documents/by-client/{clientId}");
        return result ?? [];
    }

    public async Task<DocumentDto?> UploadDocumentAsync(
        int clientId,
        int uploadedByUserId,
        string? title,
        IFormFile file)
    {
        ApplyAuthHeader();
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
