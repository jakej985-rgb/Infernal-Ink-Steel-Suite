using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ResetPasswordDialogViewModel : BaseViewModel
    {
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
    }
}
