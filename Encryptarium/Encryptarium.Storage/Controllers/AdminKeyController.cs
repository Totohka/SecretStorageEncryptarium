using BusinessLogic.Entities;
using BusinessLogic.Services.AdminKeys.Interface;
using Encryptarium.Storage.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Storage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AdminKeyController : ControllerBase
    {
        private readonly ILogger<AdminKeyController> _logger;
        private readonly IAdminKeyService _adminKeyService;
        public AdminKeyController(IAdminKeyService adminKeyService, ILogger<AdminKeyController> logger = null)
        {
            _adminKeyService = adminKeyService;
            _logger = logger;
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(CreateAdminKeys), EntitiesEnum.None, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPost(nameof(CreateAdminKeys))]
        public async Task<IActionResult> CreateAdminKeys()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(CreateAdminKeys)}()");
            await _adminKeyService.CreateAdminKeysAsync();
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(RemoveAdminKeys), EntitiesEnum.None, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPost(nameof(RemoveAdminKeys))]
        public async Task<IActionResult> RemoveAdminKeys()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(RemoveAdminKeys)}()");

            await _adminKeyService.RemoveAdminKeyAsync();
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(SetAdminKey), EntitiesEnum.None, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy/*, Roles = Constants.Admin*/)]
        [HttpPost(nameof(SetAdminKey))]
        public async Task<IActionResult> SetAdminKey(string value)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(SetAdminKey)}()");
            await _adminKeyService.SetAdminKeyAsync(value);
            return Ok(new ServiceResponse<string>(null, true));
        }
    }
}
