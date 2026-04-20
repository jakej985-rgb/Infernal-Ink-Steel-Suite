using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Text.Json;
using System.Windows;
using InfernalInkSteelSuite.ViewModels;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ShopHoursTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private ShopSettings _shopSettings;

        public override string Header => "Shop Hours";

        public ObservableCollection<ShopDaySettingViewModel> ShopHours { get; set; } = [];
        public ObservableCollection<SpecialDaySetting> SpecialHours { get; set; } = [];

        private DateTime _selectedSpecialDate = DateTime.Today;
        public DateTime SelectedSpecialDate
        {
            get => _selectedSpecialDate;
            set { _selectedSpecialDate = value; OnPropertyChanged(); }
        }

        private bool _isSpecialDayClosed = true;
        public bool IsSpecialDayClosed
        {
            get => _isSpecialDayClosed;
            set { _isSpecialDayClosed = value; OnPropertyChanged(); }
        }

        private string _specialDayDescription = string.Empty;
        public string SpecialDayDescription
        {
            get => _specialDayDescription;
            set { _specialDayDescription = value; OnPropertyChanged(); }
        }

        public RelayCommand AddSpecialDayCommand { get; }
        public RelayCommand RemoveSpecialDayCommand { get; }
        public RelayCommand CopyToAllDaysCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }

        public ShopHoursTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            _shopSettings = _shopSettingsRepository.LoadSettings();
            
            LoadShopHours();
            LoadSpecialHours();

            AddSpecialDayCommand = new RelayCommand(AddSpecialDay);
            RemoveSpecialDayCommand = new RelayCommand(RemoveSpecialDay);
            CopyToAllDaysCommand = new RelayCommand(CopyToAllDays);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void LoadShopHours()
        {
            List<ShopDaySetting>? settings = null;
            if (!string.IsNullOrEmpty(_shopSettings.ShopHoursJson))
            {
                try { settings = JsonSerializer.Deserialize<List<ShopDaySetting>>(_shopSettings.ShopHoursJson); } catch { }
            }

            if (settings == null || settings.Count == 0)
            {
                settings = [];
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    settings.Add(new ShopDaySetting { Day = day, IsOpen = day != DayOfWeek.Sunday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(19, 0, 0) });
                }
            }

            ShopHours.Clear();
            var orderedDays = settings.OrderBy(s => s.Day == DayOfWeek.Sunday ? 7 : (int)s.Day);
            foreach (var s in orderedDays)
            {
                var vm = new ShopDaySettingViewModel
                {
                    Day = s.Day,
                    IsOpen = s.IsOpen,
                    StartTime = DateTime.Today.Add(s.StartTime),
                    EndTime = DateTime.Today.Add(s.EndTime)
                };
                vm.SelectedStartTime = vm.StartTime.ToString("hh:mm tt");
                vm.SelectedEndTime = vm.EndTime.ToString("hh:mm tt");
                ShopHours.Add(vm);
            }
        }

        private void LoadSpecialHours()
        {
            List<SpecialDaySetting>? settings = null;
            if (!string.IsNullOrEmpty(_shopSettings.SpecialHoursJson))
            {
                try { settings = JsonSerializer.Deserialize<List<SpecialDaySetting>>(_shopSettings.SpecialHoursJson); } catch { }
            }
            SpecialHours.Clear();
            if (settings != null)
            {
                foreach (var s in settings.OrderBy(x => x.Date)) SpecialHours.Add(s);
            }
        }

        private void AddSpecialDay(object? obj)
        {
            var existing = SpecialHours.FirstOrDefault(s => s.Date.Date == SelectedSpecialDate.Date);
            if (existing != null) SpecialHours.Remove(existing);

            SpecialHours.Add(new SpecialDaySetting { Date = SelectedSpecialDate, IsClosed = IsSpecialDayClosed, Description = SpecialDayDescription });
            
            var sorted = SpecialHours.OrderBy(x => x.Date).ToList();
            SpecialHours.Clear();
            foreach (var s in sorted) SpecialHours.Add(s);
            SpecialDayDescription = string.Empty;
        }

        private void RemoveSpecialDay(object? obj)
        {
            if (obj is SpecialDaySetting setting) SpecialHours.Remove(setting);
        }

        private void CopyToAllDays(object? obj)
        {
            if (obj is ShopDaySettingViewModel sourceDay)
            {
                foreach (var day in ShopHours)
                {
                    if (day.Day != sourceDay.Day)
                    {
                        day.IsOpen = sourceDay.IsOpen;
                        day.StartTime = sourceDay.StartTime;
                        day.EndTime = sourceDay.EndTime;
                        day.SelectedStartTime = sourceDay.SelectedStartTime;
                        day.SelectedEndTime = sourceDay.SelectedEndTime;
                    }
                }
            }
        }

        private void SaveSettings(object? obj)
        {
            var latestSettings = _shopSettingsRepository.LoadSettings();
            
            var hours = ShopHours.Select(h => new ShopDaySetting 
            { 
                Day = h.Day, 
                IsOpen = h.IsOpen, 
                StartTime = h.StartTime.TimeOfDay, 
                EndTime = h.EndTime.TimeOfDay 
            }).ToList();
            latestSettings.ShopHoursJson = JsonSerializer.Serialize(hours);
            latestSettings.SpecialHoursJson = JsonSerializer.Serialize(SpecialHours.ToList());

            _shopSettingsRepository.SaveSettings(latestSettings);
            MessageBox.Show("Shop hours saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
