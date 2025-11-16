using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Linq;

namespace Infernal_Ink_Steel_Suite.manager.ViewModels.Settings
{
    public class ManagerTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;

        public override string Header => "Manager";

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

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _roles;
        public ObservableCollection<string> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                OnPropertyChanged();
            }
        }

        private string _selectedRole;
        public string SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand UpdateRoleCommand { get; }

        public ManagerTabViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            LoadUsers();
            LoadRoles();

            UpdateRoleCommand = new RelayCommand(UpdateRole, CanUpdateRole);
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(_userRepository.GetAllUsers());
        }

        private void LoadRoles()
        {
            Roles = new ObservableCollection<string> { "Admin", "Manager", "User" };
        }

        private void UpdateRole(object obj)
        {
            if (SelectedUser != null && !string.IsNullOrEmpty(SelectedRole))
            {
                _userRepository.UpdateRole(SelectedUser.Username, SelectedRole);
            }
        }

        private bool CanUpdateRole(object obj)
        {
            return SelectedUser != null && !string.IsNullOrEmpty(SelectedRole);
        }
    }
}
