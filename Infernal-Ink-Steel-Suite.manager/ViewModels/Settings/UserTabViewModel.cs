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
        private readonly User _currentUser;

        public override string Header => "User";

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _avatarPath;
        public string AvatarPath
        {
            get => _avatarPath;
            set
            {
                _avatarPath = value;
                OnPropertyChanged();
            }
        }

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
        public RelayCommand OpenChangeAvatarDialogCommand { get; }

        public UserTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository, User currentUser)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            _currentUser = currentUser;

            _username = _currentUser.Username;
            _avatarPath = _currentUser.AvatarPath;

            _selectedTheme = ThemeManager.CurrentTheme;
            LoadSettings();
            OpenChangePasswordDialogCommand = new RelayCommand(OpenChangePasswordDialog);
            OpenChangeAvatarDialogCommand = new RelayCommand(OpenChangeAvatarDialog);
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
                DataContext = new ChangePasswordDialogViewModel(_userRepository, _currentUser.Username)
            };
            dialog.ShowDialog();
        }

        private void OpenChangeAvatarDialog(object? parameter)
        {
            var dialog = new ChangeAvatarDialog
            {
                DataContext = new ChangeAvatarDialogViewModel(_userRepository, _currentUser.Username)
            };
            dialog.ShowDialog();
            // Refresh the avatar path after the dialog closes
            var user = _userRepository.GetUserById(_currentUser.Id);
            if (user != null)
            {
                AvatarPath = user.AvatarPath;
            }
        }
    }
}
