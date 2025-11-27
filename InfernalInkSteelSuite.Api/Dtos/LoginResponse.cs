using InfernalInkSteelSuite.Api.Models;

namespace InfernalInkSteelSuite.Api.Dtos;

public class LoginResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public UserRole Role { get; set; }
    public string Token { get; set; } = null!; // placeholder for future JWT
}
