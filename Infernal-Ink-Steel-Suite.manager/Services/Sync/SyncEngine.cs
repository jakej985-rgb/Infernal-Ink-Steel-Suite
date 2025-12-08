using System;
using System.Timers;
using System.Threading.Tasks;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Net.NetworkInformation;

namespace InfernalInkSteelSuite.Services.Sync
{
    public class SyncEngine : ISyncEngine
    {
        private readonly ISyncQueueRepository _syncQueue;
        private readonly IApiClient _apiClient;
        private readonly IDataProvider _localProvider; // Must be the LOCAL one (not Syncing wrapper)
        private readonly System.Timers.Timer _syncTimer;
        private bool _isSyncing;

        public event EventHandler<SyncStatusEventArgs>? SyncStatusChanged;

        public bool IsSyncing => _isSyncing;

        public SyncEngine(ISyncQueueRepository syncQueue, IApiClient apiClient, IDataProvider localProvider)
        {
            _syncQueue = syncQueue;
            _apiClient = apiClient;
            _localProvider = localProvider;

            _syncTimer = new System.Timers.Timer(30000); // 30 seconds for testing, maybe 5 mins prod
            _syncTimer.Elapsed += async (s, e) => await RunSyncCycleAsync();
        }

        public void Start()
        {
            _syncTimer.Start();
            // Trigger initial sync
            Task.Run(RunSyncCycleAsync);
        }

        public void Stop()
        {
            _syncTimer.Stop();
        }

        public async Task ForceSyncAsync()
        {
            await RunSyncCycleAsync();
        }

        private async Task RunSyncCycleAsync()
        {
            if (_isSyncing) return;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                NotifyStatus(false, "Offline", DateTime.Now, false);
                return;
            }

            _isSyncing = true;
            NotifyStatus(true, "Syncing...", DateTime.Now, true);

            try
            {
                // 1. Push Changes
                await PushChangesAsync();

                // 2. Pull Changes
                await PullChangesAsync();

                NotifyStatus(false, "Synced", DateTime.Now, true);
            }
            catch (Exception ex)
            {
                NotifyStatus(false, $"Sync Failed: {ex.Message}", DateTime.Now, true);
            }
            finally
            {
                _isSyncing = false;
            }
        }

        private async Task PushChangesAsync()
        {
            var pending = _syncQueue.GetPendingItems();
            foreach (var item in pending)
            {
                try
                {
                    await ProcessSyncItem(item);
                    _syncQueue.MarkAsSynced(item.Id);
                }
                catch (Exception ex)
                {
                    _syncQueue.UpdateStatus(item.Id, "Failed", ex.Message);
                }
            }
        }

        private async Task ProcessSyncItem(SyncQueueItem item)
        {
            // Implementation depends on EntityType and Action
            // Use _apiClient to push
            // Example:
            /*
            if (item.EntityType == "Client")
            {
                var client = System.Text.Json.JsonSerializer.Deserialize<Client>(item.PayloadJson!);
                if (item.Action == "Create") await _apiClient.CreateClientAsync(client!);
                // ...
            }
            */
            await Task.Delay(100); // Stub
        }

        private async Task PullChangesAsync()
        {
            // Implementation to fetch from API and update _localProvider
            await Task.Delay(100); // Stub
        }

        private void NotifyStatus(bool isSyncing, string msg, DateTime time, bool online)
        {
            SyncStatusChanged?.Invoke(this, new SyncStatusEventArgs
            {
                IsSyncing = isSyncing,
                StatusMessage = msg,
                LastSyncTime = time,
                IsOnline = online
            });
        }
    }
}
