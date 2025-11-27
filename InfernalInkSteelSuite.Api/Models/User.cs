namespace InfernalInkSteelSuite.Api.Models;

public enum UserRole
{
    Admin,
    Manager,
    Artist,
    Piercer
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}
