using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Web.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ApiClient apiClient) : ControllerBase
{
    private readonly ApiClient _apiClient = apiClient;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Invalid credentials.");
        }

        var response = await _apiClient.LoginAsync(request.Username, request.Password);
        if (response != null)
        {
            // Set session token for subsequent ApiClient calls
            HttpContext.Session.SetString("ApiToken", response.Token);
            return Ok(response);
        }

        return Unauthorized("Invalid username or password.");
    }
}

public record LoginRequest(string Username, string Password);
