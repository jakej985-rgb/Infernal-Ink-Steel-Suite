using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Domain.Sync;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InfernalInkSteelSuite.Services
{
    public class SyncClient
    {
        private HttpClient? _httpClient;
        private string? _jwtToken;

        public void Configure(string baseUrl, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _httpClient = null;
                return;
            }

            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

            // Authenticate via JWT Bearer (C2 fix — was using X-Api-Key which the API never validates)
            _ = Task.Run(async () =>
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("auth/login", new { Username = username, Password = password });
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        if (result.TryGetProperty("token", out var tokenEl))
                        {
                            _jwtToken = tokenEl.GetString();
                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", _jwtToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SyncClient auth failed: {ex.Message}");
                }
            }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Legacy overload for backward compatibility. Uses username/password from linked settings.
        /// </summary>
        public void Configure(string baseUrl, string apiKeyOrPassword)
        {
            // Treat the second parameter as a password with a default sync username
            Configure(baseUrl, "admin", apiKeyOrPassword);
        }

        public bool IsConfigured => _httpClient != null && _jwtToken != null;

        public async Task<List<T>> GetChangesAsync<T>(string endpoint, DateTime sinceUtc)
        {
            if (_httpClient == null || _jwtToken == null) return [];
            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}?sinceUtc={sinceUtc:O}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<T>>() ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sync error (Pull from {endpoint}): {ex.Message}");
                return [];
            }
        }

        public async Task<bool> PushBatchAsync<T>(string endpoint, SyncBatchRequestDto<T> batch)
        {
            if (_httpClient == null || _jwtToken == null) return false;
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, batch);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Sync error (Push to {endpoint}): {ex.Message}");
                return false;
            }
        }
    }
}
