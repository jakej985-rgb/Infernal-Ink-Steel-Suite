using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace InfernalInkSteelSuite.Domain
{
    public class ShopSettings : ISyncEntity
    {
        public int Id { get; set; } = -1;
        public Guid SyncId { get; set; } = Guid.NewGuid();
        public DateTime LastModifiedUtc { get; set; } = DateTime.UtcNow;
        public string LastModifiedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public byte[]? RowVersion { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string LogoPath { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string SidebarArtworkPath { get; set; } = string.Empty;
        [Obsolete("This property is no longer used and will be removed in a future version.")]
        [NotMapped]
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
        public double AppFontSize { get; set; } = 14.0;
        public DateTime LastSyncUtc { get; set; } = DateTime.MinValue;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Force refresh for compiler metadata
    }

    public class ShopDaySetting
    {
        public DayOfWeek Day { get; set; }
        public bool IsOpen { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
