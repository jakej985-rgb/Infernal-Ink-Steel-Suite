using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Windows;
using InfernalInkSteelSuite.Views.Settings;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class AdminTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private ShopSettings _shopSettings;

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

        public string LoginHeadline
        {
            get => _shopSettings.LoginHeadline;
            set
            {
                if (_shopSettings.LoginHeadline != value)
                {
                    _shopSettings.LoginHeadline = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoginTagline
        {
            get => _shopSettings.LoginTagline;
            set
            {
                if (_shopSettings.LoginTagline != value)
                {
                    _shopSettings.LoginTagline = value;
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

        public RelayCommand AddUserCommand { get; }
        public RelayCommand UpdateRoleCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        public RelayCommand SaveBrandingCommand { get; }

        public AdminTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            LoadUsers();
            _shopSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();

            AddUserCommand = new RelayCommand(AddUser);
            UpdateRoleCommand = new RelayCommand(UpdateRole, CanUpdateOrReset);
            ResetPasswordCommand = new RelayCommand(ResetPassword, CanUpdateOrReset);
            SaveBrandingCommand = new RelayCommand(SaveBranding);
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(_userRepository.GetAllUsers());
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

        private void SaveBranding(object? obj)
        {
            _shopSettingsRepository.SaveSettings(_shopSettings);
        }
    }
}
