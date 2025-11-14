using System;

namespace InfernalInkSteelSuite.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsAdmin()
        {
            return Role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
