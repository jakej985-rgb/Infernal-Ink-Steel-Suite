using InfernalInkSteelSuite.Services;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ThemingTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;

        public override string Header => "Theming";

        public IEnumerable<ThemeDefinition> Themes => ThemeManager.AvailableThemes;

        private ThemeDefinition _selectedTheme;
        public ThemeDefinition SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        ThemeManager.ApplyTheme(value.Id);
                    }
                }
            }
        }

        private bool _enableAutomaticHolidayThemes;
        public bool EnableAutomaticHolidayThemes
        {
            get => _enableAutomaticHolidayThemes;
            set
            {
                if (_enableAutomaticHolidayThemes != value)
                {
                    _enableAutomaticHolidayThemes = value;
                    OnPropertyChanged();
                    SaveSettings();
                }
            }
        }

        public ThemingTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            _selectedTheme = ThemeManager.CurrentTheme;
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            EnableAutomaticHolidayThemes = settings.EnableAutomaticHolidayThemes;
        }

        private void SaveSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            settings.EnableAutomaticHolidayThemes = EnableAutomaticHolidayThemes;
            _shopSettingsRepository.SaveSettings(settings);
        }
    }
}
