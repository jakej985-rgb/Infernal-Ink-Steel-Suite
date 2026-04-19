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
                // Update properties - this is safer than DELETE/INSERT
                _db.Entry(existing).CurrentValues.SetValues(settings);
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
