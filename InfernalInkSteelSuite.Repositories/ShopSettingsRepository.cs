using InfernalInkSteelSuite.Data;
using InfernalInkSteelSuite.Domain;
using System;
using System.Linq;

namespace InfernalInkSteelSuite.Repositories
{
    public class ShopSettingsRepository(AppDbContext dbContext) : IShopSettingsRepository
    {
        private readonly AppDbContext _db = dbContext;

        public void SaveSettings(ShopSettings settings)
        {
            var existing = _db.ShopSettings.FirstOrDefault();
            if (existing == null)
            {
                _db.ShopSettings.Add(settings);
            }
            else
            {
                // Preserve sync metadata and identity from the existing record
                var preservedId = existing.Id;
                var preservedSyncId = existing.SyncId;
                var preservedRowVersion = existing.RowVersion;
                var preservedCreatedAt = existing.CreatedAt;

                _db.Entry(existing).CurrentValues.SetValues(settings);

                // Restore protected fields
                existing.Id = preservedId;
                existing.SyncId = preservedSyncId;
                existing.RowVersion = preservedRowVersion;
                existing.CreatedAt = preservedCreatedAt;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            _db.SaveChanges();
        }

        public ShopSettings LoadSettings()
        {
            return _db.ShopSettings.FirstOrDefault() ?? new ShopSettings();
        }
    }
}
