using System;

namespace InfernalInkSteelSuite.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "";
        public string ThemeKey { get; set; } = "InfernalNeon";
        public string AvatarPath { get; set; } = "";
        public decimal HourlyRate { get; set; } = 150m;
        public double SpeedFactor { get; set; } = 1.0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // New Properties - Phase 1
        public DateTime? LastLoginAt { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; } = null;
        public string Department { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; } = 0m;
        public int FontSize { get; set; } = 14;
        public string KeyboardShortcutsJson { get; set; } = string.Empty;
        public string PermissionsJson { get; set; } = string.Empty;

        public bool IsAdmin()
        {
            return string.Equals(Role?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase);
        }

        public virtual ICollection<Document> UploadedDocuments { get; set; } = new List<Document>();
    }
}
