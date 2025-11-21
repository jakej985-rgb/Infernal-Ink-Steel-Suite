using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using InfernalInkSteelSuite.Views.Settings;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class AdminTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly ShopSettings _shopSettings;

        public override string Header => "Admin";

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

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

        public bool IsSpecialMessageEnabled
        {
            get => _shopSettings.IsSpecialMessageEnabled;
            set
            {
                if (_shopSettings.IsSpecialMessageEnabled != value)
                {
                    _shopSettings.IsSpecialMessageEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SpecialMessageText
        {
            get => _shopSettings.SpecialMessageText;
            set
            {
                if (_shopSettings.SpecialMessageText != value)
                {
                    _shopSettings.SpecialMessageText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoginBackgroundPath
        {
            get => _shopSettings.LoginBackgroundPath;
            set
            {
                if (_shopSettings.LoginBackgroundPath != value)
                {
                    _shopSettings.LoginBackgroundPath = value;
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

        public double ShopMinimumRate
        {
            get => _shopSettings.ShopMinimumRate;
            set
            {
                if (_shopSettings.ShopMinimumRate != value)
                {
                    _shopSettings.ShopMinimumRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SidebarArtworkPath
        {
            get => _shopSettings.SidebarArtworkPath;
            set
            {
                if (_shopSettings.SidebarArtworkPath != value)
                {
                    _shopSettings.SidebarArtworkPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool EnableHolidayThemes
        {
            get => _shopSettings.EnableAutomaticHolidayThemes;
            set
            {
                if (_shopSettings.EnableAutomaticHolidayThemes != value)
                {
                    _shopSettings.EnableAutomaticHolidayThemes = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand AddUserCommand { get; }
        public RelayCommand UpdateRoleCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand BrowseFileCommand { get; }

        public AdminTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            _users = [];
            LoadUsers();
            _shopSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();

            AddUserCommand = new RelayCommand(AddUser);
            UpdateRoleCommand = new RelayCommand(UpdateRole, CanUpdateOrReset);
            ResetPasswordCommand = new RelayCommand(ResetPassword, CanUpdateOrReset);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            BrowseFileCommand = new RelayCommand(BrowseFile);
        }

        private void LoadUsers()
        {
            Users = [.. _userRepository.GetAllUsers()];
        }

        private void AddUser(object? obj)
        {
            var addUserDialog = new AddUserDialog();
            var addUserViewModel = new AddUserDialogViewModel();
            addUserDialog.DataContext = addUserViewModel;

            if (addUserDialog.ShowDialog() == true)
            {
                var newUser = new User
                {
                    Username = addUserViewModel.Username,
                    PasswordHash = _userRepository.HashPassword(addUserViewModel.Password),
                    Role = addUserViewModel.SelectedRole
                };
                _userRepository.AddUser(newUser);
                LoadUsers();
            }
        }

        private void UpdateRole(object? obj)
        {
            if (SelectedUser != null)
            {
                _userRepository.UpdateRole(SelectedUser.Username, SelectedUser.Role);
            }
        }

        private void ResetPassword(object? obj)
        {
            if (SelectedUser != null)
            {
                var resetPasswordDialog = new ResetPasswordDialog();
                var resetPasswordViewModel = new ResetPasswordDialogViewModel();
                resetPasswordDialog.DataContext = resetPasswordViewModel;

                if (resetPasswordDialog.ShowDialog() == true)
                {
                    _userRepository.UpdatePassword(SelectedUser.Username, resetPasswordViewModel.Password);
                }
            }
        }

        private bool CanUpdateOrReset(object? obj)
        {
            return SelectedUser != null;
        }

        private void SaveSettings(object? obj)
        {
            _shopSettingsRepository.SaveSettings(_shopSettings);
            SettingsUpdateService.NotifySettingsChanged();
        }

        private void BrowseFile(object? parameter)
        {
            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName)) return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                switch (propertyName)
                {
                    case "SidebarArtworkPath":
                        SidebarArtworkPath = openFileDialog.FileName;
                        break;
                    case "LoginBackgroundPath":
                        LoginBackgroundPath = openFileDialog.FileName;
                        break;
                }
            }
        }
    }
}
