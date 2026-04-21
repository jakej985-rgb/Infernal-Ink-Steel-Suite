namespace InfernalInkSteelSuite.Data;

public interface ICurrentUserProvider
{
    string? GetCurrentUsername();
}

public class SystemUserProvider : ICurrentUserProvider
{
    public string? GetCurrentUsername() => "System";
}
