using InfernalInkSteelSuite.Data;
using System.Security.Claims;

namespace InfernalInkSteelSuite.Api.Services;

public class WebUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentUsername()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
               ?? _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }
}
