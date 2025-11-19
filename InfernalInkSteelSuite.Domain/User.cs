using System;

namespace InfernalInkSteelSuite.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "";
        public string ThemeKey { get; set; } = "InfernalNeon";
        public string AvatarPath { get; set; } = "";
        public decimal HourlyRate { get; set; } = 150m;
        public double SpeedFactor { get; set; } = 1.0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAdmin()
        {
            return string.Equals(Role?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
