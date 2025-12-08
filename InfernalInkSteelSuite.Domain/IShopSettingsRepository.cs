using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Domain
{
    public interface IShopSettingsRepository
    {
        void SaveSettings(ShopSettings settings);
        ShopSettings LoadSettings();
    }
}
