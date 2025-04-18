using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Interface;
using Encryptarium.Auth.Controllers.Base;
using Encryptarium.Auth.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Model;
using BusinessLogic.Services.TwoFactorsAuth.Interface;
using Model.Enums;
using Encryptarium.Auth.Attributes;
using BusinessLogic.Entities;

namespace Encryptarium.Auth.Controllers
{
    /// <summary>
    /// Контроллер для авторизации через UserPass
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserPassController : BaseController
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        public UserPassController(IAccessTokenService accessTokenService, 
                                  IRefreshTokenService refreshTokenService, 
                                  IUserService userService,
                                  ILogger<UserPassController> logger,
                                  ITwoFactorAuthService twoFactorAuthService) : base(accessTokenService, logger)
        { 
            _refreshTokenService = refreshTokenService;
            _userService = userService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        /// <summary>
        /// Авторизация логин и пароль, отправка кода на email
        /// </summary>
        /// <param name="userpassDTO">DTO для входа через логин и пароль</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.UserPassController, nameof(Login), EntitiesEnum.User, PartHttpContextEnum.None)]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] UserpassDTO userpassDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserPassController)}.{nameof(Login)}()");

            User user = GetEntity(await _userService.GetUserPassAsync(userpassDTO.Login, userpassDTO.Password));
            if (user is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            string accessToken = GetEntity(await _accessTokenService.GetAccessTokenAsync(user.RefreshTokenUid, user.Uid));
            if (accessToken is null)
                return BadRequest(new ServiceResponse<string>(null, false));

            await _twoFactorAuthService.SendCodeOnEmail(user);

            return Ok(new ServiceResponse<string>(null, true));
        }

        /// <summary>
        /// Верификация кода из email, получение токенов
        /// </summary>
        /// <param name="verifyCodeDTO">DTO для верификации кода</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.UserPassController, nameof(VerifyCode), EntitiesEnum.User, PartHttpContextEnum.None)]
        [HttpPost(nameof(VerifyCode))]
        public async Task<IActionResult> VerifyCode(VerifyCodeDTO verifyCodeDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserPassController)}.{nameof(VerifyCode)}()");

            User user = GetEntity(await _userService.GetUserPassAsync(verifyCodeDTO.Login, verifyCodeDTO.Password));

            if (user is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            if (!user.IsActive)
                return Unauthorized(new ServiceResponse<string>(null, false));

            if (!user.IsActiveCode)
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
            if (accessToken is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            if (!GetEntity(await _twoFactorAuthService.VerifyCode(user, verifyCodeDTO.Code)))
                return Unauthorized(new ServiceResponse<string>(null, false));
            HttpContext.Response.Cookies.Append(Constants.RefreshToken, user.RefreshTokenUid.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                //IsEssential = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
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

        /// <summary>
        /// Регистрация пользователя через логин и пароль
        /// </summary>
        /// <param name="registrationDTO">DTO для регистрации пользователя через логин/пароль</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.UserPassController, nameof(Registration), EntitiesEnum.User, PartHttpContextEnum.None)]
        [HttpPost(nameof(Registration))]
        public async Task<IActionResult> Registration([FromBody] RegistrationDTO registrationDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserPassController)}.{nameof(Registration)}()");

            Guid refreshToken = GetEntity(await _refreshTokenService.GenerateRefreshTokenAsync());
            if (!GetBool(await _userService.CreateUserPassAsync(registrationDTO.Login, registrationDTO.Email, registrationDTO.Password, refreshToken)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        /// <summary>
        /// Тестовый метод для проверки
        /// </summary>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.UserPassController, nameof(Test), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserPassController)}.{nameof(Test)}()");

            return Ok(GetEntity(await _userService.GetAllUsersAsync()));
        }
    }
}
