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
    public class BackgroundSyncService(string connectionString, SyncClient syncClient)
    {
        private readonly string _connectionString = connectionString;
        private readonly SyncClient _syncClient = syncClient;
        private CancellationTokenSource? _cts;

        private AppDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(_connectionString);
            return new AppDbContext(optionsBuilder.Options);
        }

        public event Action<string>? OnSyncStatusChanged;

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
            int failureCount = 0;
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await PerformSyncAsync();
                    failureCount = 0; // Reset on success
                    await Task.Delay(TimeSpan.FromMinutes(2), ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    var backoffMinutes = Math.Min(Math.Pow(2, failureCount), 30); // Max 30 min backoff
                    
                    OnSyncStatusChanged?.Invoke($"Sync failed: {ex.Message}. Retrying in {backoffMinutes} min.");
                    
                    try 
                    {
                        await Task.Delay(TimeSpan.FromMinutes(backoffMinutes), ct);
                    }
                    catch (OperationCanceledException) { break; }
                }
            }
        }

        public async Task PerformSyncAsync()
        {
            using var db = CreateContext();
            var settingsRepo = new ShopSettingsRepository(db);

            InfernalInkSteelSuite.Domain.ShopSettings settings = settingsRepo.LoadSettings();
            if (string.IsNullOrEmpty(settings.LinkedAccountsJson)) 
            {
                OnSyncStatusChanged?.Invoke("Local (Not Linked)");
                return;
            }

            // Simplified deserialization since we just need the URL and Key
            var linked = JsonSerializer.Deserialize<JsonElement>(settings.LinkedAccountsJson);
            string? webAppUrl = linked.TryGetProperty("WebAppUrl", out var urlEl) ? urlEl.GetString() : null;
            string? apiKey = linked.TryGetProperty("ApiKey", out var keyEl) ? keyEl.GetString() : null;

            if (string.IsNullOrEmpty(webAppUrl)) 
            {
                OnSyncStatusChanged?.Invoke("Sync Disabled (No URL)");
                return;
            }

            _syncClient.Configure(webAppUrl, apiKey ?? "");
            if (!_syncClient.IsConfigured) 
            {
                OnSyncStatusChanged?.Invoke("Sync Error (Client Config)");
                return;
            }

            OnSyncStatusChanged?.Invoke("Syncing...");

            var lastSyncUtc = settings.LastSyncUtc;
            var nextSyncUtc = DateTime.UtcNow;

            // 1. Pull Changes from Server
            await PullChangesAsync(db, lastSyncUtc);

            // 2. Push Local Changes to Server
            await PushChangesAsync(db, lastSyncUtc);

            // 3. Persist successful sync timestamp
            settings.LastSyncUtc = nextSyncUtc;
            settingsRepo.SaveSettings(settings);

            OnSyncStatusChanged?.Invoke("Synced");
        }

        private async Task PullChangesAsync(AppDbContext db, DateTime lastSyncUtc)
        {
            // Pull Clients
            var remoteClients = await _syncClient.GetChangesAsync<Client>("api/sync/clients", lastSyncUtc);
            foreach (var remote in remoteClients)
            {
                var local = await db.Clients.FirstOrDefaultAsync(c => c.SyncId == remote.SyncId);
                if (local == null)
                {
                    remote.Id = 0; // Ensure local ID is generated
                    db.Clients.Add(remote);
                }
                else if (remote.LastModifiedUtc > local.LastModifiedUtc)
                {
                    db.Entry(local).CurrentValues.SetValues(remote);
                }
            }

            // Pull Appointments (Conflict resolution logic)
            var remoteAppts = await _syncClient.GetChangesAsync<Appointment>("api/sync/appointments", lastSyncUtc);
            foreach (var remote in remoteAppts)
            {
                var local = await db.Appointments.FirstOrDefaultAsync(a => a.SyncId == remote.SyncId);
                if (local == null)
                {
                    // Double-booking check
                    var overlap = await db.Appointments.AnyAsync(a => 
                        a.UserId == remote.UserId && 
                        a.StartTime < remote.EndTime && 
                        a.EndTime > remote.StartTime);
                    
                    if (overlap)
                    {
                        remote.HasSyncConflict = true;
                        remote.SyncConflictNotes = "Conflict: Overlap detected during server sync.";
                    }

                    remote.Id = 0;
                    db.Appointments.Add(remote);
                }
                else if (remote.LastModifiedUtc > local.LastModifiedUtc)
                {
                    db.Entry(local).CurrentValues.SetValues(remote);
                }
            }

            await db.SaveChangesAsync();
        }

        private async Task PushChangesAsync(AppDbContext db, DateTime lastSyncUtc)
        {
            // Push Local Clients
            var localClientChanges = await db.Clients
                .Where(c => c.LastModifiedUtc > lastSyncUtc)
                .ToListAsync();

            if (localClientChanges.Count > 0)
            {
                var batch = new SyncBatchRequestDto<Client>
                {
                    Changes = [.. localClientChanges.Select(c => new SyncChangeDto<Client>
                    {
                        EntityId = c.SyncId,
                        Operation = "Update",
                        Payload = c,
                        ClientTimestampUtc = c.LastModifiedUtc
                    })]
                };
                await _syncClient.PushBatchAsync("api/sync/clients", batch);
            }

            // Push Local Appointments
            var localApptChanges = await db.Appointments
                .Where(a => a.LastModifiedUtc > lastSyncUtc)
                .ToListAsync();

            if (localApptChanges.Count > 0)
            {
                var batch = new SyncBatchRequestDto<Appointment>
                {
                    Changes = [.. localApptChanges.Select(a => new SyncChangeDto<Appointment>
                    {
                        EntityId = a.SyncId,
                        Operation = "Update",
                        Payload = a,
                        ClientTimestampUtc = a.LastModifiedUtc
                    })]
                };
                await _syncClient.PushBatchAsync("api/sync/appointments", batch);
            }
        }
    }
}
