using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using InfernalInkSteelSuite.ViewModels;
using System.Text.Json;
using System;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class UserPermissions
    {
        public bool CanViewReports { get; set; }
        public bool CanManageSchedule { get; set; }
        public bool CanViewFinancials { get; set; }
        public bool CanManageInventory { get; set; }
    }

    public class ManagerTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;

        public override string Header => "Manager";

        private ObservableCollection<User> _users = [];
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
                OnPropertyChanged(nameof(HasUserSelected));
                LoadUserSettings();
            }
        }

        public bool HasUserSelected => SelectedUser != null;

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
            set
            {
                _canViewReports = value;
                OnPropertyChanged();
            }
        }

        private bool _canManageSchedule;
        public bool CanManageSchedule
        {
            get => _canManageSchedule;
            set
            {
                _canManageSchedule = value;
                OnPropertyChanged();
            }
        }

        private bool _canViewFinancials;
        public bool CanViewFinancials
        {
            get => _canViewFinancials;
            set
            {
                _canViewFinancials = value;
                OnPropertyChanged();
            }
        }

        private bool _canManageInventory;
        public bool CanManageInventory
        {
            get => _canManageInventory;
            set
            {
                _canManageInventory = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveUserSettingsCommand { get; }
        public RelayCommand ResetPermissionsCommand { get; }

        public ManagerTabViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            LoadUsers();
            LoadDepartments();

            SaveUserSettingsCommand = new RelayCommand(SaveUserSettings, CanSaveUserSettings);
            ResetPermissionsCommand = new RelayCommand(ResetPermissions, CanResetPermissions);
        }

        private void LoadUsers()
        {
            Users = [.. _userRepository.GetAllUsers().Where(u => !u.IsDeleted)];
        }

        private void LoadDepartments()
        {
            Departments = ["Tattoo", "Piercing", "Front Desk", "Management", "Other"];
        }

        private void LoadUserSettings()
        {
            if (SelectedUser == null) return;

            // Load department and commission
            SelectedDepartment = SelectedUser.Department;
            CommissionRate = SelectedUser.CommissionRate;

            // Load permissions from JSON
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
                catch
                {
                    // If JSON parsing fails, use defaults
                    ResetPermissionsToDefaults();
                }
            }
            else
            {
                ResetPermissionsToDefaults();
            }
        }

        private void SaveUserSettings(object? obj)
        {
            if (SelectedUser == null) return;

            // Save department and commission
            _userRepository.UpdateUserDepartment(SelectedUser.Username, SelectedDepartment);
            _userRepository.UpdateUserCommissionRate(SelectedUser.Username, CommissionRate);

            // Save permissions as JSON
            var permissions = new UserPermissions
            {
                CanViewReports = CanViewReports,
                CanManageSchedule = CanManageSchedule,
                CanViewFinancials = CanViewFinancials,
                CanManageInventory = CanManageInventory
            };

            var permissionsJson = JsonSerializer.Serialize(permissions);
            _userRepository.UpdateUserPermissions(SelectedUser.Username, permissionsJson);

            // Refresh user list
            LoadUsers();
        }

        private bool CanSaveUserSettings(object? obj)
        {
            return SelectedUser != null;
        }

        private void ResetPermissions(object? obj)
        {
            ResetPermissionsToDefaults();
        }

        private bool CanResetPermissions(object? obj)
        {
            return SelectedUser != null;
        }

        private void ResetPermissionsToDefaults()
        {
            // Set default permissions based on role
            if (SelectedUser == null) return;

            switch (SelectedUser.Role.ToLower())
            {
                case "admin":
                    CanViewReports = true;
                    CanManageSchedule = true;
                    CanViewFinancials = true;
                    CanManageInventory = true;
                    break;
                case "manager":
                    CanViewReports = true;
                    CanManageSchedule = true;
                    CanViewFinancials = true;
                    CanManageInventory = false;
                    break;
                default: // User
                    CanViewReports = false;
                    CanManageSchedule = false;
                    CanViewFinancials = false;
                    CanManageInventory = false;
                    break;
            }
        }
    }
}
