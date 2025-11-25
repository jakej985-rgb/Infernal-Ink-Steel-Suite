using InfernalInkSteelSuite.Repositories;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class LinkedAccountsTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;

        public override string Header => "Integrations";

        private string _instagramUrl = string.Empty;
        public string InstagramUrl
        {
            get => _instagramUrl;
            set
            {
                _instagramUrl = value;
                OnPropertyChanged();
            }
        }

        private string _facebookUrl = string.Empty;
        public string FacebookUrl
        {
            get => _facebookUrl;
            set
            {
                _facebookUrl = value;
                OnPropertyChanged();
            }
        }

        private string _twitterUrl = string.Empty;
        public string TwitterUrl
        {
            get => _twitterUrl;
            set
            {
                _twitterUrl = value;
                OnPropertyChanged();
            }
        }

        private string _websiteUrl = string.Empty;
        public string WebsiteUrl
        {
            get => _websiteUrl;
            set
            {
                _websiteUrl = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveIntegrationsCommand { get; }

        public LinkedAccountsTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            LoadSettings();
            SaveIntegrationsCommand = new RelayCommand(SaveSettings);
        }

        private void LoadSettings()
        {
            // Load from shop settings
        }

        private void SaveSettings(object? parameter)
        {
            // Save to shop settings
        }
    }
}
