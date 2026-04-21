using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace InfernalInkSteelSuite.Web.Services;

public class AuthHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var token = httpContext.Session.GetString("ApiToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
