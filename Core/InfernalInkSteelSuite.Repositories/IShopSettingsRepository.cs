using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IShopSettingsRepository
    {
        void SaveSettings(ShopSettings settings);
        ShopSettings LoadSettings();
    }
}
