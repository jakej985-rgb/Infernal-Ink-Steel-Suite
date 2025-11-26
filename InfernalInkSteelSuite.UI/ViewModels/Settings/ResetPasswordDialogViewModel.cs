using InfernalInkSteelSuite.UI.ViewModels;

namespace InfernalInkSteelSuite.UI.ViewModels.Settings
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

        public ResetPasswordDialogViewModel()
        {
            _password = string.Empty;
        }
    }
}
