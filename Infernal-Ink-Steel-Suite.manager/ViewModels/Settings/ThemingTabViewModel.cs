using InfernalInkSteelSuite.Services;
using System.Collections.ObjectModel;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ThemingTabViewModel : SettingsTabViewModel
    {
        private readonly ThemeManager _themeManager;

        public override string Header => "Theming";

        private ObservableCollection<Theme> _themes;
        public ObservableCollection<Theme> Themes
        {
            get => _themes;
            set
            {
                _themes = value;
                OnPropertyChanged();
            }
        }

        private Theme _selectedTheme;
        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SetThemeCommand { get; }

        public ThemingTabViewModel()
        {
            _themeManager = ThemeManager.Instance;
            _themes = new ObservableCollection<Theme>(_themeManager.Themes);
            _selectedTheme = _themeManager.CurrentTheme ?? new Theme();
            SetThemeCommand = new RelayCommand(SetTheme);
        }

        private void SetTheme(object? obj)
        {
            if (SelectedTheme != null)
            {
                _themeManager.SetTheme(SelectedTheme);
            }
        }
    }
}
