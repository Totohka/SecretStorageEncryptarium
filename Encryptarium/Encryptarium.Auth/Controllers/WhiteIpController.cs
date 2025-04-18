using BusinessLogic.Entities;
using BusinessLogic.Services.Ips.Interface;
using BusinessLogic.Services.Tokens.Interface;
using Encryptarium.Auth.Attributes;
using Encryptarium.Auth.Controllers.Base;
using Encryptarium.Auth.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Auth.Controllers
{
    /// <summary>
    /// Контроллер для управления IP адресами, только для админов
    /// </summary>
    [Authorize(Roles = Constants.Admin, Policy = Constants.TokenPolicy)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WhiteIpController : BaseController
    {
        private readonly IIpService _ipService;
        public WhiteIpController(IAccessTokenService accessTokenService,
                                 IIpService ipService,
                                 ILogger<WhiteIpController> logger) : base(accessTokenService, logger)
        {
            _ipService = ipService;
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(GetIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.None)]
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetIp(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(GetIp)}()");

            return Ok(await _ipService.GetIpListAsync(uid));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(GetAllIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.None)]
        [HttpGet]
        public async Task<IActionResult> GetAllIp()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(GetAllIp)}()");

            return Ok(await _ipService.GetAllIpListAsync());
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(GetIpByApiKey), EntitiesEnum.WhiteListIp, PartHttpContextEnum.None)]
        [HttpGet(nameof(GetIpByApiKey))]
        public async Task<IActionResult> GetIpByApiKey(Guid apiKeyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(GetIpByApiKey)}()");

            return Ok(await _ipService.GetIpByApiKeyUidAsync(apiKeyUid));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(CreateIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.None)]
        [HttpPost]
        public async Task<IActionResult> CreateIp(WhiteListIpDTO createWhiteListIpDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(CreateIp)}()");

            if (!GetBool(await _ipService.CreateIpAsync(createWhiteListIpDTO.Ip, createWhiteListIpDTO.ApiKeyUid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(UpdateWhiteIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpPut("edit/{uid}")]
        public async Task<IActionResult> UpdateWhiteIp(Guid uid, WhiteListIpDTO whiteListIp)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(UpdateWhiteIp)}()");

            if (!GetBool(await _ipService.UpdateIpAsync(uid, whiteListIp.Ip)))
                return BadRequest(new ServiceResponse<string>(null, false));

            if (!GetBool(await _ipService.UpdateApiKeyUidAsync(uid, whiteListIp.ApiKeyUid)))
                return BadRequest(new ServiceResponse<string>(null, false));

            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(UpdateIpApiKeyUid), EntitiesEnum.WhiteListIp, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpPut("edit/key/{uid}")]
        public async Task<IActionResult> UpdateIpApiKeyUid(Guid uid, Guid apiKeyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(UpdateIpApiKeyUid)}()");

            if (!GetBool(await _ipService.UpdateApiKeyUidAsync(uid, apiKeyUid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(UpdateIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpPut("edit/ip/{uid}")]
        public async Task<IActionResult> UpdateIp(Guid uid, string ip)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(UpdateIp)}()");

            if (!GetBool(await _ipService.UpdateIpAsync(uid, ip)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(DeactivedIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpPut("deactivate/{uid}")]
        public async Task<IActionResult> DeactivedIp(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(DeactivedIp)}()");

            if (!GetBool(await _ipService.DeactivateIpByUidAsync(uid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }


        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(DeleteIpByUid), EntitiesEnum.WhiteListIp, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpDelete("delete/uid/{uid}")]
        public async Task<IActionResult> DeleteIpByUid(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(DeleteIpByUid)}()");

            if (!GetBool(await _ipService.DeleteIpByUidAsync(uid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.WhiteIpController, nameof(DeleteIpByIp), EntitiesEnum.WhiteListIp, PartHttpContextEnum.None)]
        [HttpDelete("delete/ip/{ip}")]
        public async Task<IActionResult> DeleteIpByIp(string ip)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(WhiteIpController)}.{nameof(DeleteIpByIp)}()");

            if (!GetBool(await _ipService.DeleteIpAsync(ip)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }
    }
}
