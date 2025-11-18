using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Views.Settings;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class UserTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly string _username;

        public override string Header => "User";

        public IEnumerable<ThemeDefinition> Themes => ThemeManager.AvailableThemes;

        private ThemeDefinition _selectedTheme;
        public ThemeDefinition SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        ThemeManager.ApplyTheme(value.Id);
                    }
                }
            }
        }

        private bool _enableAutomaticHolidayThemes;
        public bool EnableAutomaticHolidayThemes
        {
            get => _enableAutomaticHolidayThemes;
            set
            {
                if (_enableAutomaticHolidayThemes != value)
                {
                    _enableAutomaticHolidayThemes = value;
                    OnPropertyChanged();
                    SaveSettings();
                }
            }
        }

        public RelayCommand OpenChangePasswordDialogCommand { get; }

        public UserTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository, string username)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            _username = username;
            _selectedTheme = ThemeManager.CurrentTheme;
            LoadSettings();
            OpenChangePasswordDialogCommand = new RelayCommand(OpenChangePasswordDialog);
        }

        private void LoadSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            EnableAutomaticHolidayThemes = settings.EnableAutomaticHolidayThemes;
        }

        private void SaveSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            settings.EnableAutomaticHolidayThemes = EnableAutomaticHolidayThemes;
            _shopSettingsRepository.SaveSettings(settings);
        }

        private void OpenChangePasswordDialog(object? parameter)
        {
            var dialog = new ChangePasswordDialog
            {
                DataContext = new ChangePasswordDialogViewModel(_userRepository, _username)
            };
            dialog.ShowDialog();
        }
    }
}
