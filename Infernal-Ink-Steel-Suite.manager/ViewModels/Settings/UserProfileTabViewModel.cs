using InfernalInkSteelSuite.Repositories;

namespace Infernal_Ink_Steel_Suite.manager.ViewModels.Settings
{
    public class UserProfileTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly string _username;

        public override string Header => "User Profile";

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
            }
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ChangePasswordCommand { get; }

        public UserProfileTabViewModel(IUserRepository userRepository, string username)
        {
            _userRepository = userRepository;
            _username = username;
            ChangePasswordCommand = new RelayCommand(ChangePassword);
        }

        private void ChangePassword(object parameter)
        {
            if (NewPassword == ConfirmPassword)
            {
                _userRepository.UpdatePassword(_username, NewPassword);
            }
        }
    }
}
