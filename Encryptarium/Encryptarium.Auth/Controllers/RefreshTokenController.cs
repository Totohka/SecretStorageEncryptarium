using Encryptarium.Auth.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Services.Tokens.Interface;
using Model.Enums;
using Model;
using Encryptarium.Auth.Attributes;
using BusinessLogic.Entities;

namespace Encryptarium.Auth.Controllers
{
    /// <summary>
    /// Контроллер CRUD для управления refresh токенами, только для админов
    /// </summary>
    [Authorize(Roles = Constants.Admin, Policy = Constants.TokenPolicy)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RefreshTokenController : BaseController
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshTokenController(ILogger<RefreshTokenController> logger,
                                      IRefreshTokenService refreshTokenService,
                                      IAccessTokenService accessTokenService) : base(accessTokenService, logger)
        {
            _refreshTokenService = refreshTokenService;
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.RefreshTokenController, nameof(GetRefreshToken), EntitiesEnum.RefreshToken, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetRefreshToken(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RefreshTokenController)}.{nameof(GetRefreshToken)}()");

            return Ok(await _refreshTokenService.GetAsync(uid));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.RefreshTokenController, nameof(GetAllRefreshToken), EntitiesEnum.RefreshToken, PartHttpContextEnum.None)]
        [HttpGet]
        public async Task<IActionResult> GetAllRefreshToken()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RefreshTokenController)}.{nameof(GetAllRefreshToken)}()");

            return Ok(await _refreshTokenService.GetAllAsync());
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.RefreshTokenController, nameof(CreateRefreshToken), EntitiesEnum.RefreshToken, PartHttpContextEnum.ResponseBody, "data")]
        [HttpPost]
        public async Task<IActionResult> CreateRefreshToken()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RefreshTokenController)}.{nameof(CreateRefreshToken)}()");

            return Ok(await _refreshTokenService.GenerateRefreshTokenAsync());
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.RefreshTokenController, nameof(DeactivedRefreshToken), EntitiesEnum.RefreshToken, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpPut("{uid}")]
        public async Task<IActionResult> DeactivedRefreshToken(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RefreshTokenController)}.{nameof(DeactivedRefreshToken)}()");

            if (!GetBool(await _refreshTokenService.DeactivateRefreshTokenAsync(uid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.RefreshTokenController, nameof(DeleteRefreshToken), EntitiesEnum.RefreshToken, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteRefreshToken(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RefreshTokenController)}.{nameof(DeleteRefreshToken)}()");

            if (!GetBool(await _refreshTokenService.DeleteRefreshTokenAsync(uid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }
    }
}
