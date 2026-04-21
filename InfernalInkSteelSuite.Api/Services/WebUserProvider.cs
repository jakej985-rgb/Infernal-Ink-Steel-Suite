using InfernalInkSteelSuite.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InfernalInkSteelSuite.Api.Services;

public class WebUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentUsername()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        // TokenService sets JwtRegisteredClaimNames.Sub with the username,
        // which the JWT middleware maps to ClaimTypes.NameIdentifier
        return user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? user?.Identity?.Name;
    }
}
