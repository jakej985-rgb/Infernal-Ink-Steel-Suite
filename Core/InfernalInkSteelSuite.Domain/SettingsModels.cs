using System;

namespace InfernalInkSteelSuite.Domain
{
    public class SpecialDaySetting
    {
        public DateTime Date { get; set; }
        public bool IsClosed { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class BreakTimeSetting
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class NotificationSettings
    {
        public bool EmailEnabled { get; set; } = true;
        public bool SmsEnabled { get; set; } = false;
        public bool DesktopEnabled { get; set; } = true;
        public int ReminderHoursBefore { get; set; } = 24;
    }

    public class BackupSettings
    {
        public string BackupPath { get; set; } = string.Empty;
        public string BackupSchedule { get; set; } = "Daily"; // Daily, Weekly, Monthly
        public int RetentionDays { get; set; } = 30;
        public bool AutoBackupEnabled { get; set; } = true;
    }
}
