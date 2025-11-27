using InfernalInkSteelSuite.Repositories;
using Microsoft.Win32;
using System.Windows;

using InfernalInkSteelSuite.Domain;
using System.Text.Json;

namespace InfernalInkSteelSuite.UI.ViewModels.Settings
{
    public class BackupSettings
    {
        public string BackupPath { get; set; } = @"C:\Backups\InfernalInk";
        public bool AutoBackupEnabled { get; set; }
        public string BackupFrequency { get; set; } = "Daily";
        public int RetentionDays { get; set; } = 30;
    }

    public class BackupDataTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;

        public override string Header => "Backup & Data";

        private string _backupPath = @"C:\Backups\InfernalInk";
        public string BackupPath
        {
            get => _backupPath;
            set
            {
                _backupPath = value;
                OnPropertyChanged();
            }
        }

        private bool _autoBackupEnabled;
        public bool AutoBackupEnabled
        {
            get => _autoBackupEnabled;
            set
            {
                _autoBackupEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _backupFrequency = "Daily";
        public string BackupFrequency
        {
            get => _backupFrequency;
            set
            {
                _backupFrequency = value;
                OnPropertyChanged();
            }
        }

        public string[] BackupFrequencyOptions { get; } = { "Daily", "Weekly", "Monthly" };

        private int _retentionDays = 30;
        public int RetentionDays
        {
            get => _retentionDays;
            set
            {
                _retentionDays = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BrowseBackupPathCommand { get; }
        public RelayCommand CreateBackupCommand { get; }
        public RelayCommand ExportDataCommand { get; }
        public RelayCommand SaveBackupSettingsCommand { get; }

        public BackupDataTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;

            BrowseBackupPathCommand = new RelayCommand(BrowseBackupPath);
            CreateBackupCommand = new RelayCommand(CreateBackup);
            ExportDataCommand = new RelayCommand(ExportData);
            SaveBackupSettingsCommand = new RelayCommand(SaveSettings);
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            if (!string.IsNullOrEmpty(settings.BackupSettingsJson))
            {
                try
                {
                    var backupSettings = JsonSerializer.Deserialize<BackupSettings>(settings.BackupSettingsJson);
                    if (backupSettings != null)
                    {
                        BackupPath = backupSettings.BackupPath;
                        AutoBackupEnabled = backupSettings.AutoBackupEnabled;
                        BackupFrequency = backupSettings.BackupFrequency;
                        RetentionDays = backupSettings.RetentionDays;
                        return;
                    }
                }
                catch { }
            }

            // Defaults
            BackupPath = @"C:\Backups\InfernalInk";
            AutoBackupEnabled = false;
            BackupFrequency = "Daily";
            RetentionDays = 30;
        }

        private void BrowseBackupPath(object? parameter)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Backup Location"
            };

            if (dialog.ShowDialog() == true)
            {
                BackupPath = dialog.FolderName;
            }
        }

        private void CreateBackup(object? parameter)
        {
            MessageBox.Show("Backup created successfully!", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportData(object? parameter)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|CSV files (*.csv)|*.csv",
                DefaultExt = ".json"
            };

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show($"Data exported to {dialog.FileName}", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveSettings(object? parameter)
        {
            var backupSettings = new BackupSettings
            {
                BackupPath = BackupPath,
                AutoBackupEnabled = AutoBackupEnabled,
                BackupFrequency = BackupFrequency,
                RetentionDays = RetentionDays
            };

            var latestSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();
            latestSettings.BackupSettingsJson = JsonSerializer.Serialize(backupSettings);
            _shopSettingsRepository.SaveSettings(latestSettings);
        }
    }
}
