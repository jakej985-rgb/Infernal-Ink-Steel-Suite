using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Domain.Sync;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace InfernalInkSteelSuite.Services
{
    public class SyncClient
    {
        private HttpClient? _httpClient;
        private string? _apiKey;

        public void Configure(string baseUrl, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _httpClient = null;
                return;
            }

            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("X-Api-Key", _apiKey);
        }

        public bool IsConfigured => _httpClient != null;

        public async Task<List<T>> GetChangesAsync<T>(string endpoint, DateTime sinceUtc)
        {
            if (_httpClient == null) return [];
            try
            {
                var response = await _httpClient.GetAsync($"{endpoint}?sinceUtc={sinceUtc:O}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<T>>() ?? [];
            }
            catch (Exception ex)
            {
                // In a production app, we'd log this properly
                System.Diagnostics.Debug.WriteLine($"Sync error (Pull from {endpoint}): {ex.Message}");
                return [];
            }
        }

        public async Task<bool> PushBatchAsync<T>(string endpoint, SyncBatchRequestDto<T> batch)
        {
            if (_httpClient == null) return false;
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
