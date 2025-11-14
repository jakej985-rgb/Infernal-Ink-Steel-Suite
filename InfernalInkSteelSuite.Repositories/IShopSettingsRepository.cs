using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.Repositories
{
    public interface IShopSettingsRepository
    {
        void CreateTable();
        void SaveSettings(ShopSettings settings);
        ShopSettings LoadSettings();
    }
}
