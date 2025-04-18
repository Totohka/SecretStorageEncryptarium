using BusinessLogic.Entities;
using BusinessLogic.Services.OAuths.Interface;
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
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : BaseController
    {
        private readonly IGitHubService _gitHubService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;
        public OAuthController(IGitHubService gitHubService,
                               IUserService userService,
                               IRefreshTokenService refreshTokenService,
                               ILogger<OAuthController> logger,
                               IAccessTokenService accessTokenService) : base(accessTokenService, logger)
        {
            _refreshTokenService = refreshTokenService;
            _userService = userService;
            _gitHubService = gitHubService;
        }

        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.OAuthController, nameof(GitHubGetCode), EntitiesEnum.User, PartHttpContextEnum.None)]
        [HttpPost(nameof(GitHubGetCode))]
        public async Task<IActionResult> GitHubGetCode([FromBody] string code)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(OAuthController)}.{nameof(GitHubGetCode)}()");

            User user = GetEntity(await _gitHubService.GetUserAsync(code));

            if (user is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            if (!user.IsActive)
                return Unauthorized(new ServiceResponse<string>(null, false));

            Guid refreshTokenUid = (Guid)user.RefreshTokenUid;
            if (GetBool(await _refreshTokenService.CheckExpireTokenAsync(refreshTokenUid)))
            {
                if (!GetBool(await _refreshTokenService.DeactivateRefreshTokenAsync(refreshTokenUid)))
                    return BadRequest(new ServiceResponse<string>(null, false));
                refreshTokenUid = GetEntity(await _refreshTokenService.GenerateRefreshTokenAsync());
                if (!GetBool(await _userService.UpdateRefreshTokenByUserAsync(user.Uid, refreshTokenUid)))
                    return BadRequest(new ServiceResponse<string>(null, false));
                user.RefreshTokenUid = refreshTokenUid;
            }

            string accessToken = GetEntity(await _accessTokenService.GetAccessTokenAsync(user.RefreshTokenUid, user.Uid));

            HttpContext.Response.Cookies.Append(Constants.AccessToken, accessToken, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(5),
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Strict
            });
            HttpContext.Response.Cookies.Append(Constants.RefreshToken, user.RefreshTokenUid.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                HttpOnly = true,
                IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new ServiceResponse<string>(null, true));
        }
    }
}
