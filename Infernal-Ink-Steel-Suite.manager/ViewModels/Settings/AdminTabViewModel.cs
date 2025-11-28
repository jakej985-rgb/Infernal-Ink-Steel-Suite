using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using InfernalInkSteelSuite.Views.Settings;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class ShopDaySettingViewModel : BaseViewModel
    {
        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set { _isOpen = value; OnPropertyChanged(); }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set { _startTime = value; OnPropertyChanged(); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(); }
        }

        public DayOfWeek Day { get; set; }

        public string DayName => Day.ToString();

        public ObservableCollection<string> TimeSlots { get; } = [];

        private string _selectedStartTime = "10:00 AM";
        public string SelectedStartTime
        {
            get => _selectedStartTime;
            set
            {
                _selectedStartTime = value;
                if (DateTime.TryParse(value, out var time)) StartTime = time;
                OnPropertyChanged();
            }
        }

        private string _selectedEndTime = "07:00 PM";
        public string SelectedEndTime
        {
            get => _selectedEndTime;
            set
            {
                _selectedEndTime = value;
                if (DateTime.TryParse(value, out var time)) EndTime = time;
                OnPropertyChanged();
            }
        }

        public ShopDaySettingViewModel()
        {
            // Generate time slots
            var start = DateTime.Today;
            for (int i = 0; i < 48; i++)
            {
                TimeSlots.Add(start.AddMinutes(i * 30).ToString("hh:mm tt"));
            }
        }
    }

    public class AdminTabViewModel : SettingsTabViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly ShopSettings _shopSettings;

        public override string Header => "Admin";

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private bool _hasUnsavedChanges;
        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set
            {
                _hasUnsavedChanges = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _lastSavedTimestamp;
        public DateTime? LastSavedTimestamp
        {
            get => _lastSavedTimestamp;
            set
            {
                _lastSavedTimestamp = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterUsers();
            }
        }

        private ObservableCollection<User> _filteredUsers;
        public ObservableCollection<User> FilteredUsers
        {
            get => _filteredUsers;
            set
            {
                _filteredUsers = value;
                OnPropertyChanged();
            }
        }

        public string ShopName
        {
            get => _shopSettings.ShopName;
            set
            {
                if (_shopSettings.ShopName != value)
                {
                    _shopSettings.ShopName = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public bool IsSpecialMessageEnabled
        {
            get => _shopSettings.IsSpecialMessageEnabled;
            set
            {
                if (_shopSettings.IsSpecialMessageEnabled != value)
                {
                    _shopSettings.IsSpecialMessageEnabled = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public string SpecialMessageText
        {
            get => _shopSettings.SpecialMessageText;
            set
            {
                if (_shopSettings.SpecialMessageText != value)
                {
                    _shopSettings.SpecialMessageText = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public string LoginBackgroundPath
        {
            get => _shopSettings.LoginBackgroundPath;
            set
            {
                if (_shopSettings.LoginBackgroundPath != value)
                {
                    _shopSettings.LoginBackgroundPath = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public double TattooRate
        {
            get => _shopSettings.TattooPerHour;
            set
            {
                if (_shopSettings.TattooPerHour != value)
                {
                    _shopSettings.TattooPerHour = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public double PiercingSingle
        {
            get => _shopSettings.PiercingSingle;
            set
            {
                if (_shopSettings.PiercingSingle != value)
                {
                    _shopSettings.PiercingSingle = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public double ShopMinimumRate
        {
            get => _shopSettings.ShopMinimumRate;
            set
            {
                if (_shopSettings.ShopMinimumRate != value)
                {
                    _shopSettings.ShopMinimumRate = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public double TaxRate
        {
            get => _shopSettings.TaxRate;
            set
            {
                if (_shopSettings.TaxRate != value)
                {
                    _shopSettings.TaxRate = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public string DepositType
        {
            get => _shopSettings.DepositType;
            set
            {
                if (_shopSettings.DepositType != value)
                {
                    _shopSettings.DepositType = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public double DepositAmount
        {
            get => _shopSettings.DepositAmount;
            set
            {
                if (_shopSettings.DepositAmount != value)
                {
                    _shopSettings.DepositAmount = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public int BookingBufferMinutes
        {
            get => _shopSettings.BookingBufferMinutes;
            set
            {
                if (_shopSettings.BookingBufferMinutes != value)
                {
                    _shopSettings.BookingBufferMinutes = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public string CancellationPolicy
        {
            get => _shopSettings.CancellationPolicy;
            set
            {
                if (_shopSettings.CancellationPolicy != value)
                {
                    _shopSettings.CancellationPolicy = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public string SidebarArtworkPath
        {
            get => _shopSettings.SidebarArtworkPath;
            set
            {
                if (_shopSettings.SidebarArtworkPath != value)
                {
                    _shopSettings.SidebarArtworkPath = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public bool EnableHolidayThemes
        {
            get => _shopSettings.EnableAutomaticHolidayThemes;
            set
            {
                if (_shopSettings.EnableAutomaticHolidayThemes != value)
                {
                    _shopSettings.EnableAutomaticHolidayThemes = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public double AppFontSize
        {
            get => _shopSettings.AppFontSize;
            set
            {
                if (Math.Abs(_shopSettings.AppFontSize - value) > 0.01)
                {
                    _shopSettings.AppFontSize = value;
                    OnPropertyChanged();
                    OnSettingChanged();
                }
            }
        }

        public ObservableCollection<ShopDaySettingViewModel> ShopHours { get; set; } = [];

        private void LoadShopHours()
        {
            List<ShopDaySetting>? settings = null;
            if (!string.IsNullOrEmpty(_shopSettings.ShopHoursJson))
            {
                try
                {
                    settings = JsonSerializer.Deserialize<List<ShopDaySetting>>(_shopSettings.ShopHoursJson);
                }
                catch { }
            }

            if (settings == null || settings.Count == 0)
            {
                settings = [];
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    settings.Add(new ShopDaySetting
                    {
                        Day = day,
                        IsOpen = day != DayOfWeek.Sunday,
                        StartTime = DateTime.Today.AddHours(10).TimeOfDay,
                        EndTime = DateTime.Today.AddHours(19).TimeOfDay
                    });
                }
            }

            ShopHours.Clear();
            // Sort by Monday first
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

        public ObservableCollection<int> AppointmentDurationPresets { get; set; } = [];

        public ObservableCollection<SpecialDaySetting> SpecialHours { get; set; } = [];

        private DateTime _selectedSpecialDate = DateTime.Today;
        public DateTime SelectedSpecialDate
        {
            get => _selectedSpecialDate;
            set
            {
                _selectedSpecialDate = value;
                OnPropertyChanged();
            }
        }

        private bool _isSpecialDayClosed = true;
        public bool IsSpecialDayClosed
        {
            get => _isSpecialDayClosed;
            set
            {
                _isSpecialDayClosed = value;
                OnPropertyChanged();
            }
        }

        private string _specialDayDescription = string.Empty;
        public string SpecialDayDescription
        {
            get => _specialDayDescription;
            set
            {
                _specialDayDescription = value;
                OnPropertyChanged();
            }
        }

        private int _newDurationPreset;
        public int NewDurationPreset
        {
            get => _newDurationPreset;
            set
            {
                _newDurationPreset = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddDurationPresetCommand { get; }
        public RelayCommand RemoveDurationPresetCommand { get; }

        public RelayCommand AddSpecialDayCommand { get; }
        public RelayCommand RemoveSpecialDayCommand { get; }
        public RelayCommand CopyToAllDaysCommand { get; }

        public RelayCommand AddUserCommand { get; }
        public RelayCommand UpdateRoleCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        public RelayCommand DeleteUserCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand BrowseFileCommand { get; }
        public RelayCommand RestoreDefaultsCommand { get; }
        public RelayCommand ClearImageCommand { get; }

        private void LoadSpecialHours()
        {
            List<SpecialDaySetting>? settings = null;
            if (!string.IsNullOrEmpty(_shopSettings.SpecialHoursJson))
            {
                try
                {
                    settings = JsonSerializer.Deserialize<List<SpecialDaySetting>>(_shopSettings.SpecialHoursJson);
                }
                catch { }
            }

            if (settings == null) settings = [];

            SpecialHours.Clear();
            foreach (var s in settings.OrderBy(x => x.Date))
            {
                SpecialHours.Add(s);
            }
        }

        public AdminTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            _users = [];
            _filteredUsers = [];
            LoadUsers();
            _shopSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();
            LoadShopHours();
            LoadDurationPresets();
            LoadSpecialHours();

            AddUserCommand = new RelayCommand(AddUser);
            UpdateRoleCommand = new RelayCommand(UpdateRole, CanUpdateOrReset);
            ResetPasswordCommand = new RelayCommand(ResetPassword, CanUpdateOrReset);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanUpdateOrReset);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            BrowseFileCommand = new RelayCommand(BrowseFile);
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);
            ClearImageCommand = new RelayCommand(ClearImage);

            AddDurationPresetCommand = new RelayCommand(AddDurationPreset);
            RemoveDurationPresetCommand = new RelayCommand(RemoveDurationPreset);

            AddSpecialDayCommand = new RelayCommand(AddSpecialDay);
            RemoveSpecialDayCommand = new RelayCommand(RemoveSpecialDay);
            CopyToAllDaysCommand = new RelayCommand(CopyToAllDays);
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
                OnSettingChanged();
            }
        }

        private void AddSpecialDay(object? obj)
        {
            var existing = SpecialHours.FirstOrDefault(s => s.Date.Date == SelectedSpecialDate.Date);
            if (existing != null)
            {
                SpecialHours.Remove(existing);
            }

            SpecialHours.Add(new SpecialDaySetting
            {
                Date = SelectedSpecialDate,
                IsClosed = IsSpecialDayClosed,
                Description = SpecialDayDescription
            });

            // Re-sort
            var sorted = SpecialHours.OrderBy(x => x.Date).ToList();
            SpecialHours.Clear();
            foreach (var s in sorted) SpecialHours.Add(s);

            SpecialDayDescription = string.Empty;
            OnSettingChanged();
        }

        private void RemoveSpecialDay(object? obj)
        {
            if (obj is SpecialDaySetting setting)
            {
                SpecialHours.Remove(setting);
                OnSettingChanged();
            }
        }

        private void LoadDurationPresets()
        {
            List<int>? presets = null;
            if (!string.IsNullOrEmpty(_shopSettings.AppointmentDurationPresetsJson))
            {
                try
                {
                    presets = JsonSerializer.Deserialize<List<int>>(_shopSettings.AppointmentDurationPresetsJson);
                }
                catch { }
            }

            if (presets == null || presets.Count == 0)
            {
                presets = [30, 60, 90, 120, 180, 240]; // Defaults
            }

            AppointmentDurationPresets.Clear();
            foreach (var p in presets.OrderBy(x => x))
            {
                AppointmentDurationPresets.Add(p);
            }
        }

        private void AddDurationPreset(object? obj)
        {
            if (NewDurationPreset > 0 && !AppointmentDurationPresets.Contains(NewDurationPreset))
            {
                AppointmentDurationPresets.Add(NewDurationPreset);
                // Re-sort
                var sorted = AppointmentDurationPresets.OrderBy(x => x).ToList();
                AppointmentDurationPresets.Clear();
                foreach (var p in sorted) AppointmentDurationPresets.Add(p);

                NewDurationPreset = 0; // Reset input
                OnSettingChanged();
            }
        }

        private void RemoveDurationPreset(object? obj)
        {
            if (obj is int duration && AppointmentDurationPresets.Contains(duration))
            {
                AppointmentDurationPresets.Remove(duration);
                OnSettingChanged();
            }
        }

        private void OnSettingChanged()
        {
            HasUnsavedChanges = true;
        }

        private void RestoreDefaults(object? obj)
        {
            if (MessageBox.Show("Are you sure you want to restore defaults? Unsaved changes will be lost.", "Confirm Restore", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _shopSettingsRepository.LoadSettings(); // Reload to discard changes
                // Trigger property changes
                OnPropertyChanged(string.Empty);
                HasUnsavedChanges = false;
                LoadDurationPresets(); // Reload presets
            }
        }

        private void ClearImage(object? parameter)
        {
            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName)) return;

            switch (propertyName)
            {
                case "SidebarArtworkPath":
                    SidebarArtworkPath = string.Empty;
                    break;
                case "LoginBackgroundPath":
                    LoginBackgroundPath = string.Empty;
                    break;
            }
        }

        private void LoadUsers()
        {
            Users = [.. _userRepository.GetAllUsers().Where(u => !u.IsDeleted)];
            FilterUsers();
        }

        private void FilterUsers()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredUsers = new ObservableCollection<User>(Users);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                FilteredUsers = new ObservableCollection<User>(Users.Where(u =>
                    u.Username.ToLower().Contains(lowerSearch) ||
                    u.Role.ToLower().Contains(lowerSearch)));
            }
        }

        private void AddUser(object? obj)
        {
            var addUserDialog = new AddUserDialog();
            var addUserViewModel = new AddUserDialogViewModel();
            addUserDialog.DataContext = addUserViewModel;

            if (addUserDialog.ShowDialog() == true)
            {
                var newUser = new User
                {
                    Username = addUserViewModel.Username,
                    PasswordHash = _userRepository.HashPassword(addUserViewModel.Password),
                    Role = addUserViewModel.SelectedRole,
                    IsActive = true
                };
                _userRepository.AddUser(newUser);
                LoadUsers();
            }
        }

        private void UpdateRole(object? obj)
        {
            if (SelectedUser != null)
            {
                _userRepository.UpdateRole(SelectedUser.Username, SelectedUser.Role);
                // Refresh list to ensure consistency
                LoadUsers();
            }
        }

        private void ResetPassword(object? obj)
        {
            if (SelectedUser != null)
            {
                var resetPasswordDialog = new ResetPasswordDialog();
                var resetPasswordViewModel = new ResetPasswordDialogViewModel();
                resetPasswordDialog.DataContext = resetPasswordViewModel;

                if (resetPasswordDialog.ShowDialog() == true)
                {
                    _userRepository.UpdatePassword(SelectedUser.Username, resetPasswordViewModel.Password);
                    MessageBox.Show("Password updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteUser(object? obj)
        {
            if (SelectedUser != null)
            {
                if (MessageBox.Show($"Are you sure you want to delete user '{SelectedUser.Username}'? This action can be undone by an administrator.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _userRepository.SoftDeleteUser(SelectedUser.Username);
                    LoadUsers();
                }
            }
        }

        private bool CanUpdateOrReset(object? obj)
        {
            return SelectedUser != null;
        }

        private void SaveSettings(object? obj)
        {
            // Reload latest settings to ensure we don't overwrite changes from other tabs
            var latestSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();

            // Apply Admin Tab changes to the latest settings
            latestSettings.ShopName = ShopName;
            latestSettings.IsSpecialMessageEnabled = IsSpecialMessageEnabled;
            latestSettings.SpecialMessageText = SpecialMessageText;
            latestSettings.LoginBackgroundPath = LoginBackgroundPath;
            latestSettings.TattooPerHour = TattooRate;
            latestSettings.PiercingSingle = PiercingSingle;
            latestSettings.ShopMinimumRate = ShopMinimumRate;
            latestSettings.TaxRate = TaxRate;
            latestSettings.DepositType = DepositType;
            latestSettings.DepositAmount = DepositAmount;
            latestSettings.BookingBufferMinutes = BookingBufferMinutes;
            latestSettings.CancellationPolicy = CancellationPolicy;
            latestSettings.SidebarArtworkPath = SidebarArtworkPath;
            latestSettings.SidebarArtworkPath = SidebarArtworkPath;
            latestSettings.EnableAutomaticHolidayThemes = EnableHolidayThemes;
            latestSettings.AppFontSize = AppFontSize;

            // Serialize Shop Hours
            var settings = ShopHours.Select(vm => new ShopDaySetting
            {
                Day = vm.Day,
                IsOpen = vm.IsOpen,
                StartTime = vm.StartTime.TimeOfDay,
                EndTime = vm.EndTime.TimeOfDay
            }).ToList();

            latestSettings.ShopHoursJson = JsonSerializer.Serialize(settings);
            latestSettings.AppointmentDurationPresetsJson = JsonSerializer.Serialize(AppointmentDurationPresets);

            // Save the updated settings object
            _shopSettingsRepository.SaveSettings(latestSettings);

            // Update local reference (optional, but good for consistency)
            // Note: We don't replace _shopSettings entirely to avoid breaking bindings if they were bound directly,
            // but here we are binding to ViewModel properties which wrap _shopSettings, so we should update the backing fields if we want to reflect external changes?
            // Actually, for this specific bug fix, we just want to ensure OUTGOING save is correct.
            // INCOMING changes from other tabs won't be reflected in UI until reload, but that's acceptable for now.

            SettingsUpdateService.NotifySettingsChanged();
            HasUnsavedChanges = false;
            LastSavedTimestamp = DateTime.Now;
        }

        private void BrowseFile(object? parameter)
        {
            var propertyName = parameter as string;
            if (string.IsNullOrEmpty(propertyName)) return;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                switch (propertyName)
                {
                    case "SidebarArtworkPath":
                        SidebarArtworkPath = openFileDialog.FileName;
                        break;
                    case "LoginBackgroundPath":
                        LoginBackgroundPath = openFileDialog.FileName;
                        break;
                }
            }
        }
        protected override string ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(TaxRate):
                    if (TaxRate < 0 || TaxRate > 100) return "Tax rate must be between 0 and 100.";
                    break;
                case nameof(DepositAmount):
                    if (DepositAmount < 0) return "Deposit amount cannot be negative.";
                    break;
                case nameof(BookingBufferMinutes):
                    if (BookingBufferMinutes < 0) return "Buffer time cannot be negative.";
                    break;
                case nameof(ShopMinimumRate):
                    if (ShopMinimumRate < 0) return "Minimum rate cannot be negative.";
                    break;
                case nameof(TattooRate):
                    if (TattooRate < 0) return "Tattoo rate cannot be negative.";
                    break;
                case nameof(PiercingSingle):
                    if (PiercingSingle < 0) return "Piercing rate cannot be negative.";
                    break;
            }
            return base.ValidateProperty(propertyName);
        }
    }
}
