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

        public ObservableCollection<string> TimeSlots { get; } = new ObservableCollection<string>();

        private string _selectedStartTime;
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

        private string _selectedEndTime;
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

        public string ShopName
        {
            get => _shopSettings.ShopName;
            set
            {
                if (_shopSettings.ShopName != value)
                {
                    _shopSettings.ShopName = value;
                    OnPropertyChanged();
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
                }
            }
        }

        public ObservableCollection<ShopDaySettingViewModel> ShopHours { get; set; } = new();

        private void LoadShopHours()
        {
            List<ShopDaySetting> settings = null;
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
                settings = new List<ShopDaySetting>();
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

        public RelayCommand AddUserCommand { get; }
        public RelayCommand UpdateRoleCommand { get; }
        public RelayCommand ResetPasswordCommand { get; }
        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand BrowseFileCommand { get; }

        public AdminTabViewModel(IUserRepository userRepository, IShopSettingsRepository shopSettingsRepository)
        {
            _userRepository = userRepository;
            _shopSettingsRepository = shopSettingsRepository;
            _users = [];
            LoadUsers();
            _shopSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();
            LoadShopHours();

            AddUserCommand = new RelayCommand(AddUser);
            UpdateRoleCommand = new RelayCommand(UpdateRole, CanUpdateOrReset);
            ResetPasswordCommand = new RelayCommand(ResetPassword, CanUpdateOrReset);
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            BrowseFileCommand = new RelayCommand(BrowseFile);
        }

        private void LoadUsers()
        {
            Users = [.. _userRepository.GetAllUsers()];
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
                    Role = addUserViewModel.SelectedRole
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
                }
            }
        }

        private bool CanUpdateOrReset(object? obj)
        {
            return SelectedUser != null;
        }

        private void SaveSettings(object? obj)
        {
            // Serialize Shop Hours
            var settings = ShopHours.Select(vm => new ShopDaySetting
            {
                Day = vm.Day,
                IsOpen = vm.IsOpen,
                StartTime = vm.StartTime.TimeOfDay,
                EndTime = vm.EndTime.TimeOfDay
            }).ToList();

            _shopSettings.ShopHoursJson = JsonSerializer.Serialize(settings);

            _shopSettingsRepository.SaveSettings(_shopSettings);
            SettingsUpdateService.NotifySettingsChanged();
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
    }
}
