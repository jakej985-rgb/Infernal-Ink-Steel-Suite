namespace InfernalInkSteelSuite.Api.Dtos;

public class UserCreateDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public Models.UserRole Role { get; set; }
}
