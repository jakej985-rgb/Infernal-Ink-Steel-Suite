using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ShopSettingsTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private ShopSettings _shopSettings;

        public override string Header => "Shop Settings";

        public string ShopName
        {
            get => _shopSettings.ShopName;
            set
            {
                if (_shopSettings.ShopName != value)
                {
                    _shopSettings.ShopName = value;
                    OnPropertyChanged();
                }
            }
        }

        public double TattooRate
        {
            get => _shopSettings.TattooPerHour;
            set
            {
                if (_shopSettings.TattooPerHour != value)
                {
                    _shopSettings.TattooPerHour = value;
                    OnPropertyChanged();
                }
            }
        }

        public double PiercingSingle
        {
            get => _shopSettings.PiercingSingle;
            set
            {
                if (_shopSettings.PiercingSingle != value)
                {
                    _shopSettings.PiercingSingle = value;
                    OnPropertyChanged();
                }
            }
        }

        public double PiercingMulti
        {
            get => _shopSettings.PiercingMulti;
            set
            {
                if (_shopSettings.PiercingMulti != value)
                {
                    _shopSettings.PiercingMulti = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand SaveCommand { get; }

        public ShopSettingsTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            _shopSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();
            SaveCommand = new RelayCommand(SaveSettings);
        }

        private void SaveSettings(object? obj)
        {
            _shopSettingsRepository.SaveSettings(_shopSettings);
        }
    }
}
