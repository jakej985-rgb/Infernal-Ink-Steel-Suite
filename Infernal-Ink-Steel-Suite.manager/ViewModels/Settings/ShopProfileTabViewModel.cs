using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using Microsoft.Win32;
using System.Windows;
using System;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ShopProfileTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private ShopSettings _shopSettings;

        public override string Header => "Shop Profile";

        public string ShopName
        {
            get => _shopSettings.ShopName;
            set { if (_shopSettings.ShopName != value) { _shopSettings.ShopName = value; OnPropertyChanged(); } }
        }

        public bool IsSpecialMessageEnabled
        {
            get => _shopSettings.IsSpecialMessageEnabled;
            set { if (_shopSettings.IsSpecialMessageEnabled != value) { _shopSettings.IsSpecialMessageEnabled = value; OnPropertyChanged(); } }
        }

        public string SpecialMessageText
        {
            get => _shopSettings.SpecialMessageText;
            set { if (_shopSettings.SpecialMessageText != value) { _shopSettings.SpecialMessageText = value; OnPropertyChanged(); } }
        }

        public string LoginBackgroundPath
        {
            get => _shopSettings.LoginBackgroundPath;
            set { if (_shopSettings.LoginBackgroundPath != value) { _shopSettings.LoginBackgroundPath = value; OnPropertyChanged(); } }
        }

        public string SidebarArtworkPath
        {
            get => _shopSettings.SidebarArtworkPath;
            set { if (_shopSettings.SidebarArtworkPath != value) { _shopSettings.SidebarArtworkPath = value; OnPropertyChanged(); } }
        }

        public bool EnableHolidayThemes
        {
            get => _shopSettings.EnableAutomaticHolidayThemes;
            set { if (_shopSettings.EnableAutomaticHolidayThemes != value) { _shopSettings.EnableAutomaticHolidayThemes = value; OnPropertyChanged(); } }
        }

        public double AppFontSize
        {
            get => _shopSettings.AppFontSize;
            set { if (Math.Abs(_shopSettings.AppFontSize - value) > 0.01) { _shopSettings.AppFontSize = value; OnPropertyChanged(); } }
        }

        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand BrowseFileCommand { get; }
        public RelayCommand ClearImageCommand { get; }

        public ShopProfileTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            _shopSettings = _shopSettingsRepository.LoadSettings();

            SaveSettingsCommand = new RelayCommand(SaveSettings);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            ClearImageCommand = new RelayCommand(ClearImage);
        }

        private void BrowseFile(object? parameter)
        {
            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName)) return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                switch (propertyName)
                {
                    case "SidebarArtworkPath": SidebarArtworkPath = openFileDialog.FileName; break;
                    case "LoginBackgroundPath": LoginBackgroundPath = openFileDialog.FileName; break;
                }
            }
        }

        private void ClearImage(object? parameter)
        {
            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName)) return;

            switch (propertyName)
            {
                case "SidebarArtworkPath": SidebarArtworkPath = string.Empty; break;
                case "LoginBackgroundPath": LoginBackgroundPath = string.Empty; break;
            }
        }

        private void SaveSettings(object? obj)
        {
            var latestSettings = _shopSettingsRepository.LoadSettings();
            
            latestSettings.ShopName = ShopName;
            latestSettings.IsSpecialMessageEnabled = IsSpecialMessageEnabled;
            latestSettings.SpecialMessageText = SpecialMessageText;
            latestSettings.LoginBackgroundPath = LoginBackgroundPath;
            latestSettings.SidebarArtworkPath = SidebarArtworkPath;
            latestSettings.EnableAutomaticHolidayThemes = EnableHolidayThemes;
            latestSettings.AppFontSize = AppFontSize;

            _shopSettingsRepository.SaveSettings(latestSettings);
            MessageBox.Show("Shop profile settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
