using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Views.Settings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

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

        private string _role;
        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
            }
        }

        private string _department;
        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
            }
        }

        private int _fontSize;
        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FontSizeLabel));

                // Update application resource for real-time preview
                if (Application.Current != null)
                {
                    Application.Current.Resources["StandardFontSize"] = (double)value;
                }
            }
        }

        public string FontSizeLabel
        {
            get
            {
                return FontSize switch
                {
                    <= 12 => "Small",
                    <= 14 => "Medium",
                    <= 16 => "Large",
                    _ => "Extra Large"
                };
            }
        }

        public ObservableCollection<int> FontSizeOptions { get; } = [10, 12, 14, 16, 18, 20];

        public static IEnumerable<ThemeDefinition> Themes => ThemeManager.AvailableThemes;

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
                    SaveShopSettings();
                }
            }
        }

        public RelayCommand OpenChangePasswordDialogCommand { get; }
        public RelayCommand OpenChangeAvatarDialogCommand { get; }
        public RelayCommand SaveFontSizeCommand { get; }

        public UserTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository, User currentUser)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            _currentUser = currentUser;

            _username = _currentUser.Username;
            _avatarPath = _currentUser.AvatarPath;
            _role = _currentUser.Role;
            _department = _currentUser.Department;
            _fontSize = _currentUser.FontSize;

            // Set initial font size
            if (Application.Current != null)
            {
                Application.Current.Resources["StandardFontSize"] = (double)_fontSize;
            }

            _selectedTheme = ThemeManager.CurrentTheme;
            LoadShopSettings();

            OpenChangePasswordDialogCommand = new RelayCommand(OpenChangePasswordDialog);
            OpenChangeAvatarDialogCommand = new RelayCommand(OpenChangeAvatarDialog);
            SaveFontSizeCommand = new RelayCommand(SaveFontSize);
        }

        private void LoadShopSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            EnableAutomaticHolidayThemes = settings.EnableAutomaticHolidayThemes;
        }

        private void SaveShopSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            settings.EnableAutomaticHolidayThemes = EnableAutomaticHolidayThemes;
            _shopSettingsRepository.SaveSettings(settings);
        }

        private void SaveFontSize(object? parameter)
        {
            _userRepository.UpdateUserFontSize(_currentUser.Username, FontSize);
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
