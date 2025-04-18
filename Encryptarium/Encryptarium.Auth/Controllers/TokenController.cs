using BusinessLogic.Entities;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Interface;
using Encryptarium.Auth.Attributes;
using Encryptarium.Auth.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Entities;
using Model.Enums;

namespace Encryptarium.Auth.Controllers
{
    /// <summary>
    /// Контроллер для обновления токенов JWT
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController : BaseController
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;

        public TokenController(ILogger<TokenController> logger,
                               IRefreshTokenService refreshTokenService,
                               IAccessTokenService accessTokenService,
                               IUserService userService) : base(accessTokenService, logger)
        {
            _refreshTokenService = refreshTokenService;
            _userService = userService;
        }

        /// <summary>
        /// Обновление access и refresh токена
        /// </summary>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.TokenController, nameof(RefreshToken), EntitiesEnum.None, PartHttpContextEnum.None)]
        [HttpPost(nameof(RefreshToken))]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(TokenController)}.{nameof(RefreshToken)}()");

            Guid? userUid = GetClaimUserUid();

            if (userUid is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            Guid refreshTokenUid = new Guid(HttpContext.Request.Cookies[Constants.RefreshToken]);

            if (GetBool(await _refreshTokenService.CheckExpireTokenAsync(refreshTokenUid)))
            {
                if (!GetBool(await _refreshTokenService.DeactivateRefreshTokenAsync(refreshTokenUid)))
                    return BadRequest(new ServiceResponse<string>(null, false));
                refreshTokenUid = GetEntity(await _refreshTokenService.GenerateRefreshTokenAsync());

                if (!GetBool(await _userService.UpdateRefreshTokenByUserAsync((Guid)userUid, refreshTokenUid)))
                    return BadRequest(new ServiceResponse<string>(null, false));
                HttpContext.Response.Cookies.Append(Constants.RefreshToken, refreshTokenUid.ToString(), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            }

            string accessToken = GetEntity(await _accessTokenService.GetAccessTokenAsync(refreshTokenUid, (Guid)userUid));

            HttpContext.Response.Cookies.Append(Constants.AccessToken, accessToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(10),
                HttpOnly = true,
                //IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });

            return Ok(new ServiceResponse<string>(null, true));
        }
    }
}
