using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class AccessibilityTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly User _currentUser;

        public override string Header => "Accessibility";

        private int _fontSize;
        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                OnPropertyChanged();
            }
        }

        private bool _highContrastMode;
        public bool HighContrastMode
        {
            get => _highContrastMode;
            set
            {
                _highContrastMode = value;
                OnPropertyChanged();
            }
        }

        private bool _screenReaderMode;
        public bool ScreenReaderMode
        {
            get => _screenReaderMode;
            set
            {
                _screenReaderMode = value;
                OnPropertyChanged();
            }
        }

        private bool _reducedMotion;
        public bool ReducedMotion
        {
            get => _reducedMotion;
            set
            {
                _reducedMotion = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveAccessibilitySettingsCommand { get; }

        public AccessibilityTabViewModel(IUserRepository userRepository, User currentUser)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;

            _fontSize = _currentUser.FontSize;

            SaveAccessibilitySettingsCommand = new RelayCommand(SaveSettings);
        }

        private void SaveSettings(object? parameter)
        {
            _currentUser.FontSize = FontSize;
            _userRepository.UpdateUser(_currentUser);
        }
    }
}
