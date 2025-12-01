using InfernalInkSteelSuite.Repositories;
using InfernalInkSteelSuite.Domain;

using System.Text.Json;

namespace InfernalInkSteelSuite.ViewModels.Settings
{
    public class NotificationSettings
    {
        public bool EmailAppointmentReminders { get; set; }
        public bool SmsAppointmentReminders { get; set; }
        public bool DesktopNotifications { get; set; }
        public string ReminderTiming { get; set; } = "1 day before";
    }

    public class NotificationSettingsTabViewModel : SettingsTabViewModel
    {
        private readonly IShopSettingsRepository _shopSettingsRepository;

        public override string Header => "Notifications";

        private bool _emailAppointmentReminders;
        public bool EmailAppointmentReminders
        {
            get => _emailAppointmentReminders;
            set
            {
                _emailAppointmentReminders = value;
                OnPropertyChanged();
            }
        }

        private bool _smsAppointmentReminders;
        public bool SmsAppointmentReminders
        {
            get => _smsAppointmentReminders;
            set
            {
                _smsAppointmentReminders = value;
                OnPropertyChanged();
            }
        }

        private bool _desktopNotifications;
        public bool DesktopNotifications
        {
            get => _desktopNotifications;
            set
            {
                _desktopNotifications = value;
                OnPropertyChanged();
            }
        }

        private string _reminderTiming = "1 day before";
        public string ReminderTiming
        {
            get => _reminderTiming;
            set
            {
                _reminderTiming = value;
                OnPropertyChanged();
            }
        }

        public string[] ReminderTimingOptions { get; } =
        [
            "1 hour before",
            "2 hours before",
            "4 hours before",
            "1 day before",
            "2 days before"
        ];

        public RelayCommand SaveNotificationSettingsCommand { get; }

        public NotificationSettingsTabViewModel(IShopSettingsRepository shopSettingsRepository)
        {
            _shopSettingsRepository = shopSettingsRepository;
            LoadSettings();
            SaveNotificationSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void LoadSettings()
        {
            var settings = _shopSettingsRepository.LoadSettings();
            if (!string.IsNullOrEmpty(settings.NotificationSettingsJson))
            {
                try
                {
                    var notificationSettings = JsonSerializer.Deserialize<NotificationSettings>(settings.NotificationSettingsJson);
                    if (notificationSettings != null)
                    {
                        EmailAppointmentReminders = notificationSettings.EmailAppointmentReminders;
                        SmsAppointmentReminders = notificationSettings.SmsAppointmentReminders;
                        DesktopNotifications = notificationSettings.DesktopNotifications;
                        ReminderTiming = notificationSettings.ReminderTiming;
                        return;
                    }
                }
                catch { }
            }

            // Defaults if load fails or empty
            EmailAppointmentReminders = true;
            SmsAppointmentReminders = false;
            DesktopNotifications = true;
            ReminderTiming = "1 day before";
        }

        private void SaveSettings(object? parameter)
        {
            var notificationSettings = new NotificationSettings
            {
                EmailAppointmentReminders = EmailAppointmentReminders,
                SmsAppointmentReminders = SmsAppointmentReminders,
                DesktopNotifications = DesktopNotifications,
                ReminderTiming = ReminderTiming
            };

            var latestSettings = _shopSettingsRepository.LoadSettings() ?? new ShopSettings();
            latestSettings.NotificationSettingsJson = JsonSerializer.Serialize(notificationSettings);
            _shopSettingsRepository.SaveSettings(latestSettings);
        }
    }
}
