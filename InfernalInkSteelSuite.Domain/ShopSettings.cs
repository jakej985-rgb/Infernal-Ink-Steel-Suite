using System;

namespace InfernalInkSteelSuite.Domain
{
    public class ShopSettings
    {
        public int Id { get; set; } = -1;
        public string ShopName { get; set; } = string.Empty;
        public string LogoPath { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string SidebarArtworkPath { get; set; } = string.Empty;
        [Obsolete("This property is no longer used and will be removed in a future version.")]
        public string LoginHeadline { get; set; } = string.Empty;
        public string SpecialMessageText { get; set; } = string.Empty;
        public string LoginBackgroundPath { get; set; } = string.Empty;
        public string LoginHeadlineFontFamily { get; set; } = string.Empty;
        public string LoginTaglineFontFamily { get; set; } = string.Empty;
        public string LoginTextColor { get; set; } = string.Empty;
        public double TattooPerHour { get; set; } = 0.0;
        public double PiercingSingle { get; set; } = 0.0;
        public double PiercingMulti { get; set; } = 0.0;
        public double ShopMinimumRate { get; set; } = 0.0;
        public bool EnableAutomaticHolidayThemes { get; set; } = false;
        public bool IsSpecialMessageEnabled { get; set; } = true;
        public string ShopHoursJson { get; set; } = string.Empty;

        // New Settings - Phase 1
        public double TaxRate { get; set; } = 0.0;
        public string DepositType { get; set; } = "Percentage"; // "Percentage" or "Fixed"
        public double DepositAmount { get; set; } = 0.0;
        public int BookingBufferMinutes { get; set; } = 0;
        public string CancellationPolicy { get; set; } = string.Empty;
        public string AppointmentDurationPresetsJson { get; set; } = string.Empty;
        public string SpecialHoursJson { get; set; } = string.Empty;
        public string NotificationSettingsJson { get; set; } = string.Empty;
        public string BackupSettingsJson { get; set; } = string.Empty;
        public string LinkedAccountsJson { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class ShopDaySetting
    {
        public DayOfWeek Day { get; set; }
        public bool IsOpen { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
