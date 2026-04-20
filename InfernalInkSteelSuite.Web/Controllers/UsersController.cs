using InfernalInkSteelSuite.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(ApiClient apiClient) : ControllerBase
{
    private readonly ApiClient _apiClient = apiClient;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _apiClient.GetUsersAsync();
        return Ok(users);
    }
}
