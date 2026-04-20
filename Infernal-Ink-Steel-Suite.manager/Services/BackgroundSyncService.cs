using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Domain.Sync;
using InfernalInkSteelSuite.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace InfernalInkSteelSuite.Services
{
    public class BackgroundSyncService
    {
        private readonly AppDbContext _localDb;
        private readonly SyncClient _syncClient;
        private readonly IShopSettingsRepository _settingsRepo;
        private CancellationTokenSource? _cts;

        public event Action<string>? OnSyncStatusChanged;

        public BackgroundSyncService(AppDbContext localDb, SyncClient syncClient, IShopSettingsRepository settingsRepo)
        {
            _localDb = localDb;
            _syncClient = syncClient;
            _settingsRepo = settingsRepo;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _ = Task.Run(() => SyncLoop(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private async Task SyncLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await PerformSyncAsync();
                }
                catch (Exception ex)
                {
                    OnSyncStatusChanged?.Invoke($"Sync failed: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(2), ct);
            }
        }

        public async Task PerformSyncAsync()
        {
            var settings = _settingsRepo.LoadSettings();
            if (string.IsNullOrEmpty(settings.LinkedAccountsJson)) return;

            // Simplified deserialization since we just need the URL and Key
            var linked = JsonSerializer.Deserialize<JsonElement>(settings.LinkedAccountsJson);
            string? webAppUrl = linked.TryGetProperty("WebAppUrl", out var urlEl) ? urlEl.GetString() : null;
            string? apiKey = linked.TryGetProperty("ApiKey", out var keyEl) ? keyEl.GetString() : null;

            if (string.IsNullOrEmpty(webAppUrl)) return;

            _syncClient.Configure(webAppUrl, apiKey ?? "");
            if (!_syncClient.IsConfigured) return;

            OnSyncStatusChanged?.Invoke("Syncing...");

            var lastSyncUtc = settings.LastSyncUtc;
            var nextSyncUtc = DateTime.UtcNow;

            // 1. Pull Changes from Server
            await PullChangesAsync(lastSyncUtc);

            // 2. Push Local Changes to Server
            await PushChangesAsync(lastSyncUtc);

            // 3. Persist successful sync timestamp
            settings.LastSyncUtc = nextSyncUtc;
            _settingsRepo.SaveSettings(settings);

            OnSyncStatusChanged?.Invoke("Synced");
        }

        private async Task PullChangesAsync(DateTime lastSyncUtc)
        {
            // Pull Clients
            var remoteClients = await _syncClient.GetChangesAsync<Client>("api/sync/clients", lastSyncUtc);
            foreach (var remote in remoteClients)
            {
                var local = await _localDb.Clients.FirstOrDefaultAsync(c => c.SyncId == remote.SyncId);
                if (local == null)
                {
                    remote.Id = 0; // Ensure local ID is generated
                    _localDb.Clients.Add(remote);
                }
                else if (remote.LastModifiedUtc > local.LastModifiedUtc)
                {
                    _localDb.Entry(local).CurrentValues.SetValues(remote);
                }
            }

            // Pull Appointments (Conflict resolution logic)
            var remoteAppts = await _syncClient.GetChangesAsync<Appointment>("api/sync/appointments", lastSyncUtc);
            foreach (var remote in remoteAppts)
            {
                var local = await _localDb.Appointments.FirstOrDefaultAsync(a => a.SyncId == remote.SyncId);
                if (local == null)
                {
                    // Double-booking check
                    var overlap = await _localDb.Appointments.AnyAsync(a => 
                        a.UserId == remote.UserId && 
                        a.StartTime < remote.EndTime && 
                        a.EndTime > remote.StartTime);
                    
                    if (overlap)
                    {
                        remote.HasSyncConflict = true;
                        remote.SyncConflictNotes = "Conflict: Overlap detected during server sync.";
                    }

                    remote.Id = 0;
                    _localDb.Appointments.Add(remote);
                }
                else if (remote.LastModifiedUtc > local.LastModifiedUtc)
                {
                    _localDb.Entry(local).CurrentValues.SetValues(remote);
                }
            }

            await _localDb.SaveChangesAsync();
        }

        private async Task PushChangesAsync(DateTime lastSyncUtc)
        {
            // Push Local Clients
            var localClientChanges = await _localDb.Clients
                .Where(c => c.LastModifiedUtc > lastSyncUtc)
                .ToListAsync();

            if (localClientChanges.Any())
            {
                var batch = new SyncBatchRequestDto<Client>
                {
                    Changes = localClientChanges.Select(c => new SyncChangeDto<Client>
                    {
                        EntityId = c.SyncId,
                        Operation = "Update",
                        Payload = c,
                        ClientTimestampUtc = c.LastModifiedUtc
                    }).ToList()
                };
                await _syncClient.PushBatchAsync("api/sync/clients", batch);
            }

            // Push Local Appointments
            var localApptChanges = await _localDb.Appointments
                .Where(a => a.LastModifiedUtc > lastSyncUtc)
                .ToListAsync();

            if (localApptChanges.Any())
            {
                var batch = new SyncBatchRequestDto<Appointment>
                {
                    Changes = localApptChanges.Select(a => new SyncChangeDto<Appointment>
                    {
                        EntityId = a.SyncId,
                        Operation = "Update",
                        Payload = a,
                        ClientTimestampUtc = a.LastModifiedUtc
                    }).ToList()
                };
                await _syncClient.PushBatchAsync("api/sync/appointments", batch);
            }
        }
    }
}
