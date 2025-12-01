using InfernalInkSteelSuite.Domain;
using InfernalInkSteelSuite.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SettingsController(IShopSettingsRepository settingsRepository) : ControllerBase
    {
        private readonly IShopSettingsRepository _settingsRepository = settingsRepository;

        [HttpGet]
        public ActionResult<ShopSettings> GetSettings()
        {
            var settings = _settingsRepository.LoadSettings();
            return Ok(settings);
        }

        [HttpPut]
        [Authorize(Policy = "IsAdmin")]
        public ActionResult UpdateSettings(ShopSettings settings)
        {
            _settingsRepository.SaveSettings(settings);
            return Ok();
        }
    }
}
