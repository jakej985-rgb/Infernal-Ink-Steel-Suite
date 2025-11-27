namespace InfernalInkSteelSuite.Api.Dtos;

public class UserUpdateDto
{
    public string Username { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public Models.UserRole Role { get; set; }
    public bool IsActive { get; set; }
}
