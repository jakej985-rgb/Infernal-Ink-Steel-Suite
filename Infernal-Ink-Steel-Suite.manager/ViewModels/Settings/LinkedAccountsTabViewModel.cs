using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Text.Json;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class LinkedAccountsSettings
    {
        public string InstagramUrl { get; set; } = string.Empty;
        public string FacebookUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public string WebAppUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

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

        private string _webAppUrl = string.Empty;
        public string WebAppUrl
        {
            get => _webAppUrl;
            set
            {
                _webAppUrl = value;
                OnPropertyChanged();
            }
        }

        private string _apiKey = string.Empty;
        public string ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
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
            var settings = _shopSettingsRepository.LoadSettings();
            if (!string.IsNullOrEmpty(settings.LinkedAccountsJson))
            {
                try
                {
                    var linkedAccounts = JsonSerializer.Deserialize<LinkedAccountsSettings>(settings.LinkedAccountsJson);
                    if (linkedAccounts != null)
                    {
                        InstagramUrl = linkedAccounts.InstagramUrl;
                        FacebookUrl = linkedAccounts.FacebookUrl;
                        TwitterUrl = linkedAccounts.TwitterUrl;
                        WebsiteUrl = linkedAccounts.WebsiteUrl;
                        WebAppUrl = linkedAccounts.WebAppUrl;
                        ApiKey = linkedAccounts.ApiKey;
                    }
                }
                catch { }
            }
        }

        private void SaveSettings(object? parameter)
        {
            var linkedAccounts = new LinkedAccountsSettings
            {
                InstagramUrl = InstagramUrl,
                FacebookUrl = FacebookUrl,
                TwitterUrl = TwitterUrl,
                WebsiteUrl = WebsiteUrl,
                WebAppUrl = WebAppUrl,
                ApiKey = ApiKey
            };

            var latestSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();
            latestSettings.LinkedAccountsJson = JsonSerializer.Serialize(linkedAccounts);
            _shopSettingsRepository.SaveSettings(latestSettings);
        }
    }
}
