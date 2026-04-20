using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using InfernalInkSteelSuite.ViewModels;
using System.Text.Json;
using System;
using System.Windows;
using InfernalInkSteelSuite.Views.Settings;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class UserManagementTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;

        public override string Header => "Users";

        private ObservableCollection<User> _users = [];
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredUsers));
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
                OnPropertyChanged(nameof(HasUserSelected));
                LoadUserSettings();
            }
        }

        public bool HasUserSelected => SelectedUser != null;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredUsers));
            }
        }

        public ObservableCollection<User> FilteredUsers
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return Users;
                
                var lowerSearch = SearchText.ToLower();
                return new ObservableCollection<User>(Users.Where(u => 
                    u.Username.Contains(lowerSearch, StringComparison.OrdinalIgnoreCase) ||
                    (u.Role ?? "").Contains(lowerSearch, StringComparison.OrdinalIgnoreCase)));
            }
        }

        private ObservableCollection<string> _departments = [];
        public ObservableCollection<string> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged();
            }
        }

        private string _selectedDepartment = string.Empty;
        public string SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged();
            }
        }

        private decimal _commissionRate;
        public decimal CommissionRate
        {
            get => _commissionRate;
            set
            {
                _commissionRate = value;
                OnPropertyChanged();
            }
        }

        // Permission Properties
        private bool _canViewReports;
        public bool CanViewReports
        {
            get => _canViewReports;
            set { _canViewReports = value; OnPropertyChanged(); }
        }

        private bool _canManageSchedule;
        public bool CanManageSchedule
        {
            get => _canManageSchedule;
            set { _canManageSchedule = value; OnPropertyChanged(); }
        }

        private bool _canViewFinancials;
        public bool CanViewFinancials
        {
            get => _canViewFinancials;
            set { _canViewFinancials = value; OnPropertyChanged(); }
        }

        private bool _canManageInventory;
        public bool CanManageInventory
        {
            get => _canManageInventory;
            set { _canManageInventory = value; OnPropertyChanged(); }
        }

        // Commands
        public RelayCommand AddUserCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        public RelayCommand DeleteUserCommand { get; }
        public RelayCommand SaveUserSettingsCommand { get; }
        public RelayCommand ResetPermissionsCommand { get; }

        public UserManagementTabViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            LoadUsers();
            LoadDepartments();

            AddUserCommand = new RelayCommand(AddUser);
            ResetPasswordCommand = new RelayCommand(ResetPassword, CanUpdateOrReset);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanUpdateOrReset);
            SaveUserSettingsCommand = new RelayCommand(SaveUserSettings, CanSaveUserSettings);
            ResetPermissionsCommand = new RelayCommand(ResetPermissions, CanResetPermissions);
        }

        private void LoadUsers()
        {
            Users = [.. _userRepository.GetAllUsers().Where(u => !((ISyncEntity)u).IsDeleted)];
        }

        private void LoadDepartments()
        {
            Departments = ["Piercer", "Tattoo", "Both"];
        }

        private void LoadUserSettings()
        {
            if (SelectedUser == null) return;

            SelectedDepartment = SelectedUser.Department;
            CommissionRate = SelectedUser.CommissionRate;

            if (!string.IsNullOrEmpty(SelectedUser.PermissionsJson))
            {
                try
                {
                    var permissions = JsonSerializer.Deserialize<UserPermissions>(SelectedUser.PermissionsJson);
                    if (permissions != null)
                    {
                        CanViewReports = permissions.CanViewReports;
                        CanManageSchedule = permissions.CanManageSchedule;
                        CanViewFinancials = permissions.CanViewFinancials;
                        CanManageInventory = permissions.CanManageInventory;
                    }
                }
                catch { ResetPermissionsToDefaults(); }
            }
            else { ResetPermissionsToDefaults(); }
        }

        private void SaveUserSettings(object? obj)
        {
            if (SelectedUser == null) return;

            SelectedUser.Department = SelectedDepartment;
            SelectedUser.CommissionRate = CommissionRate;

            var permissions = new UserPermissions
            {
                CanViewReports = CanViewReports,
                CanManageSchedule = CanManageSchedule,
                CanViewFinancials = CanViewFinancials,
                CanManageInventory = CanManageInventory
            };

            SelectedUser.PermissionsJson = JsonSerializer.Serialize(permissions);
            _userRepository.UpdateUser(SelectedUser);
            LoadUsers();
            MessageBox.Show("User settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    Role = addUserViewModel.SelectedRole,
                    IsActive = true
                };
                _userRepository.AddUser(newUser);
                LoadUsers();
            }
        }

        private void ResetPassword(object? obj)
        {
            if (SelectedUser == null) return;

            var resetPasswordDialog = new ResetPasswordDialog();
            var resetPasswordViewModel = new ResetPasswordDialogViewModel();
            resetPasswordDialog.DataContext = resetPasswordViewModel;

            if (resetPasswordDialog.ShowDialog() == true)
            {
                _userRepository.UpdatePassword(SelectedUser.Username, resetPasswordViewModel.Password);
                MessageBox.Show("Password updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteUser(object? obj)
        {
            if (SelectedUser == null) return;

            if (MessageBox.Show($"Are you sure you want to delete user '{SelectedUser.Username}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _userRepository.SoftDeleteUser(SelectedUser.Username);
                LoadUsers();
                SelectedUser = null;
            }
        }

        private bool CanUpdateOrReset(object? obj) => SelectedUser != null;
        private bool CanSaveUserSettings(object? obj) => SelectedUser != null;
        private bool CanResetPermissions(object? obj) => SelectedUser != null;

        private void ResetPermissions(object? obj) => ResetPermissionsToDefaults();

        private void ResetPermissionsToDefaults()
        {
            if (SelectedUser == null) return;
            switch ((SelectedUser.Role ?? string.Empty).ToLower())
            {
                case "admin":
                    CanViewReports = CanManageSchedule = CanViewFinancials = CanManageInventory = true;
                    break;
                case "manager":
                    CanViewReports = CanManageSchedule = CanViewFinancials = true;
                    CanManageInventory = false;
                    break;
                default:
                    CanViewReports = CanManageSchedule = CanViewFinancials = CanManageInventory = false;
                    break;
            }
        }
    }
}
