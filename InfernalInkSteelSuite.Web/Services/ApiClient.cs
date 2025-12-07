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

    // Stats DTOs
    public record DashboardStatsDto(int AppointmentsToday, int TotalClients, List<ClientSummaryDto> RecentClients, bool IsShopOpen, int ActiveArtistsCount, int UpcomingAppointments, int OpenQuotes);
    public record ClientSummaryDto(int Id, string Name, string Email);
    public record AppointmentStatDto(DateTime Date, int Count);

    // Quote DTOs
    public class QuoteInput
    {
        public int? ClientId { get; set; }
        public string Placement { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public bool IsCoverUp { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int CoverageLevel { get; set; }
        public int LineComplexity { get; set; }
        public int ShadingComplexity { get; set; }
        public int ColorComplexity { get; set; }
        public int Difficulty { get; set; }
        public int ArtistId { get; set; }
    }

    public class QuoteEstimate
    {
        public double EstimatedHoursLow { get; set; }
        public double EstimatedHoursHigh { get; set; }
        public decimal PriceLow { get; set; }
        public decimal PriceHigh { get; set; }
        public decimal ShopMinimum { get; set; }
        public decimal RecommendedDeposit { get; set; }
        public double ConfidenceScore { get; set; }
        public int SimilarJobsCount { get; set; }
    }

    public class QuoteDto : QuoteEstimate
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int ArtistId { get; set; }
        public string Placement { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public bool IsCoverUp { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int CoverageLevel { get; set; }
        public int LineComplexity { get; set; }
        public int ShadingComplexity { get; set; }
        public int ColorComplexity { get; set; }
        public int Difficulty { get; set; }
        public DateTime CreatedAt { get; set; }
    }


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

    public async Task<ClientDto?> CreateClientAsync(ClientDto client)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/clients", client);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ClientDto>();
    }

    public async Task<bool> UpdateClientAsync(ClientDto client)
    {
        ApplyAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/clients/{client.Id}", client);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync(DateTime? date = null, int? artistId = null)
    {
        ApplyAuthHeader();
        var query = new List<string>();
        if (date.HasValue) query.Add($"date={date.Value:O}");
        if (artistId.HasValue) query.Add($"artistId={artistId.Value}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

        var result = await _http.GetFromJsonAsync<List<AppointmentDto>>($"api/appointments{qs}");
        return result ?? [];
    }

    public async Task<AppointmentDto?> GetAppointmentAsync(int id)
    {
        ApplyAuthHeader();
        return await _http.GetFromJsonAsync<AppointmentDto>($"api/appointments/{id}");
    }

    public async Task<AppointmentDto?> CreateAppointmentAsync(AppointmentDto appt)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/appointments", appt);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AppointmentDto>();
    }

    public async Task<bool> UpdateAppointmentAsync(AppointmentDto appt)
    {
        ApplyAuthHeader();
        var response = await _http.PutAsJsonAsync($"api/appointments/{appt.Id}", appt);
        return response.IsSuccessStatusCode;
    }

    // DOCUMENTS

    public Task<List<DocumentDto>?> GetDocumentsAsync()
    {
        ApplyAuthHeader();
        return _http.GetFromJsonAsync<List<DocumentDto>>("api/Documents");
    }

    public async Task<List<DocumentDto>> GetDocumentsForClientAsync(int clientId)
    {
        ApplyAuthHeader();
        // Updated endpoint to match new controller if needed, but the new controller exposes "api/Documents/by-client/{clientId}"
        var result = await _http.GetFromJsonAsync<List<DocumentDto>>($"api/Documents/by-client/{clientId}");
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

        var response = await _http.PostAsync("api/Documents", content);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<DocumentDto>();
    }

    // QUOTES

    public async Task<QuoteEstimate?> CalculateQuoteAsync(QuoteInput input)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/Quotes/preview", input);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<QuoteEstimate>();
    }

    public async Task<QuoteDto?> CreateQuoteAsync(QuoteInput input)
    {
        ApplyAuthHeader();
        var response = await _http.PostAsJsonAsync("api/Quotes", input);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<QuoteDto>();
    }

    public async Task<List<QuoteDto>> GetAllQuotesAsync()
    {
        ApplyAuthHeader();
        var result = await _http.GetFromJsonAsync<List<QuoteDto>>("api/Quotes");
        return result ?? [];
    }

    // STATS

    public async Task<DashboardStatsDto?> GetDashboardStatsAsync()
    {
        ApplyAuthHeader();
        try
        {
            return await _http.GetFromJsonAsync<DashboardStatsDto>("api/Stats/overview");
        }
        catch (HttpRequestException)
        {
            // Fallback if API down or empty
            return new DashboardStatsDto(0, 0, [], false, 0, 0, 0);
        }
    }

    public async Task<List<AppointmentStatDto>> GetAppointmentStatsAsync(DateTime? from, DateTime? to)
    {
        ApplyAuthHeader();
        var query = new List<string>();
        if (from.HasValue) query.Add($"from={from.Value:O}");
        if (to.HasValue) query.Add($"to={to.Value:O}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

        var result = await _http.GetFromJsonAsync<List<AppointmentStatDto>>($"api/Stats/appointments-by-day{qs}");
        return result ?? [];
    }

    // SETTINGS

    public record ShopSettingsDto(
        string ShopName,
        decimal HourlyRate,
        decimal MinimumRate,
        double DepositPercentage,
        string Theme,
        string Tagline,
        string ContactEmail,
        string ContactPhone
    );

    public async Task<ShopSettingsDto?> GetShopSettingsAsync()
    {
        ApplyAuthHeader();
        try
        {
            return await _http.GetFromJsonAsync<ShopSettingsDto>("api/Settings");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<bool> UpdateShopSettingsAsync(ShopSettingsDto settings)
    {
        ApplyAuthHeader();
        var response = await _http.PutAsJsonAsync("api/Settings", settings);
        return response.IsSuccessStatusCode;
    }
}
