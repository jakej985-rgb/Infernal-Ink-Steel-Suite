using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.DataProviders.Syncing
{
    public class SyncingShopSettingsRepository : IShopSettingsRepository
    {
        private readonly IShopSettingsRepository _inner;
        private readonly ISyncQueueRepository _syncQueue;

        public SyncingShopSettingsRepository(IShopSettingsRepository inner, ISyncQueueRepository syncQueue)
        {
            _inner = inner;
            _syncQueue = syncQueue;
        }

        public void SaveSettings(ShopSettings settings)
        {
            _inner.SaveSettings(settings);
            // ShopSettings usually singular record, ID might be 1.
            _syncQueue.Enqueue(new SyncQueueItem
            {
                EntityType = "ShopSettings",
                EntityId = settings.Id,
                Action = "Update",
                PayloadJson = JsonSerializer.Serialize(settings)
            });
        }

        public ShopSettings LoadSettings() => _inner.LoadSettings();
    }
}
