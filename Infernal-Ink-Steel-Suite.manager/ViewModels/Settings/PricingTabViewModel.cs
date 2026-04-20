using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Text.Json;
using System.Windows;
using System.Collections.Generic;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class PricingTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private ShopSettings _shopSettings;

        public override string Header => "Pricing";

        public double TattooRate
        {
            get => _shopSettings.TattooPerHour;
            set { if (_shopSettings.TattooPerHour != value) { _shopSettings.TattooPerHour = value; OnPropertyChanged(); } }
        }

        public double PiercingSingle
        {
            get => _shopSettings.PiercingSingle;
            set { if (_shopSettings.PiercingSingle != value) { _shopSettings.PiercingSingle = value; OnPropertyChanged(); } }
        }

        public double ShopMinimumRate
        {
            get => _shopSettings.ShopMinimumRate;
            set { if (_shopSettings.ShopMinimumRate != value) { _shopSettings.ShopMinimumRate = value; OnPropertyChanged(); } }
        }

        public double TaxRate
        {
            get => _shopSettings.TaxRate;
            set { if (_shopSettings.TaxRate != value) { _shopSettings.TaxRate = value; OnPropertyChanged(); } }
        }

        public string DepositType
        {
            get => _shopSettings.DepositType;
            set { if (_shopSettings.DepositType != value) { _shopSettings.DepositType = value; OnPropertyChanged(); } }
        }

        public double DepositAmount
        {
            get => _shopSettings.DepositAmount;
            set { if (_shopSettings.DepositAmount != value) { _shopSettings.DepositAmount = value; OnPropertyChanged(); } }
        }

        public int BookingBufferMinutes
        {
            get => _shopSettings.BookingBufferMinutes;
            set { if (_shopSettings.BookingBufferMinutes != value) { _shopSettings.BookingBufferMinutes = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<int> AppointmentDurationPresets { get; set; } = [];

        private int _newDurationPreset;
        public int NewDurationPreset
        {
            get => _newDurationPreset;
            set { _newDurationPreset = value; OnPropertyChanged(); }
        }

        public RelayCommand AddDurationPresetCommand { get; }
        public RelayCommand RemoveDurationPresetCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }

        public PricingTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            _shopSettings = _shopSettingsRepository.LoadSettings();
            
            LoadDurationPresets();

            AddDurationPresetCommand = new RelayCommand(AddDurationPreset);
            RemoveDurationPresetCommand = new RelayCommand(RemoveDurationPreset);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void LoadDurationPresets()
        {
            List<int>? presets = null;
            if (!string.IsNullOrEmpty(_shopSettings.AppointmentDurationPresetsJson))
            {
                try { presets = JsonSerializer.Deserialize<List<int>>(_shopSettings.AppointmentDurationPresetsJson); } catch { }
            }
            presets ??= [30, 60, 90, 120, 180, 240];
            AppointmentDurationPresets.Clear();
            foreach (var p in presets.OrderBy(x => x)) AppointmentDurationPresets.Add(p);
        }

        private void AddDurationPreset(object? obj)
        {
            if (NewDurationPreset > 0 && !AppointmentDurationPresets.Contains(NewDurationPreset))
            {
                AppointmentDurationPresets.Add(NewDurationPreset);
                var sorted = AppointmentDurationPresets.OrderBy(x => x).ToList();
                AppointmentDurationPresets.Clear();
                foreach (var p in sorted) AppointmentDurationPresets.Add(p);
                NewDurationPreset = 0;
            }
        }

        private void RemoveDurationPreset(object? obj)
        {
            if (obj is int duration && AppointmentDurationPresets.Contains(duration))
            {
                AppointmentDurationPresets.Remove(duration);
            }
        }

        private void SaveSettings(object? obj)
        {
            var latestSettings = _shopSettingsRepository.LoadSettings();
            
            latestSettings.TattooPerHour = TattooRate;
            latestSettings.PiercingSingle = PiercingSingle;
            latestSettings.ShopMinimumRate = ShopMinimumRate;
            latestSettings.TaxRate = TaxRate;
            latestSettings.DepositType = DepositType;
            latestSettings.DepositAmount = DepositAmount;
            latestSettings.BookingBufferMinutes = BookingBufferMinutes;
            latestSettings.AppointmentDurationPresetsJson = JsonSerializer.Serialize(AppointmentDurationPresets.ToList());

            _shopSettingsRepository.SaveSettings(latestSettings);
            MessageBox.Show("Pricing settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
