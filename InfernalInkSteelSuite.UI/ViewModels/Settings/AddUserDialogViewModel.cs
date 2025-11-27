using System.Collections.ObjectModel;

namespace InfernalInkSteelSuite.UI.ViewModels.Settings
{
    public class AddUserDialogViewModel : BaseViewModel
    {
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

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
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

        public AddUserDialogViewModel()
        {
            _username = string.Empty;
            _password = string.Empty;
            _roles = ["Admin", "Manager", "User"];
            _selectedRole = _roles[0];
        }
    }
}
