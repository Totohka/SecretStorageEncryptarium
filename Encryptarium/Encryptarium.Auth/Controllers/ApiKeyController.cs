using BusinessLogic.Entities;
using BusinessLogic.Services.ApiKeys.Interface;
using BusinessLogic.Services.Ips.Interface;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Interface;
using Encryptarium.Auth.Attributes;
using Encryptarium.Auth.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Entities;
using Model.Enums;

namespace Encryptarium.Auth.Controllers
{
    /// <summary>
    /// Контроллер для авторизации через API-Key
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApiKeyController : BaseController
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly IUserService _userService;
        private readonly IIpService _ipService;
        private readonly IRefreshTokenService _refreshTokenService;
        public ApiKeyController(IApiKeyService apiKeyService,
                                IUserService userService,
                                IRefreshTokenService refreshTokenService,
                                IIpService ipService,
                                ILogger<ApiKeyController> logger,
                                IAccessTokenService accessTokenService) : base(accessTokenService, logger)
        {
            _apiKeyService = apiKeyService;
            _userService = userService;
            _ipService = ipService;
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// Создание ключа
        /// </summary>
        /// <param name="userUid">Для админа, который будет настраивать вход для других</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(CreateApiKey), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)] //нужна политика для действий, которые может делать или админ или создатель юзер
        [HttpPost(nameof(CreateApiKey))]
        public async Task<IActionResult> CreateApiKey(Guid? userUid, string name)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(CreateApiKey)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            var changeMethodAuthDTO = new ChangeMethodAuthDTO() { IsApiKey = true };

            //Если есть, значит админ генерит ключ другому пользователю
            if (userUid is not null)
                userUidClaim = userUid;

            // Здесь ключ не хеширован, пользователю нужно один раз показать его ключ, больше такой возможности не будет.
            // Следовательно при сохранении в БД, нужно его захешировать его 2 раза, т.к. потом ключ будет хешироваться на
            // клиенте и на сервере.
            string key = GetEntity(await _apiKeyService.CreateApiKey((Guid)userUidClaim, name));
            ApiKey apiKey = GetEntity(await _apiKeyService.GetApiKeyAsync(key));

            if (!GetBool(await _ipService.CreateIpAsync(ipAddress, apiKey.Uid)))
                return BadRequest(new ServiceResponse<string>(null, false));

            if (!GetBool(await _userService.ChangeAuthMethodAsync((Guid)userUidClaim, changeMethodAuthDTO)))
                return BadRequest(new ServiceResponse<string>(null, false));
            var response = new ServiceResponse<string>(key, true);
            return Ok(response);
        }

        /// <summary>
        /// Авторизация через ключ
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(LoginWithApiKey), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [HttpPost(nameof(LoginWithApiKey))]
        public async Task<IActionResult> LoginWithApiKey([FromBody] string key)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(LoginWithApiKey)}()");

            User user = GetEntity(await _apiKeyService.VerifyApiKey(key));

            ApiKey apiKey = GetEntity(await _apiKeyService.GetApiKeyAsync(key));

            if (!GetBool(await _ipService.VerifyIp(ipAddress, apiKey.Uid)))
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
            var response = new ServiceResponse<string>(null, true);
            return Ok(response);
        }

        /// <summary>
        /// Получение всех апи ключей пользователя
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(GetApiKeysByUserUid), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpGet(nameof(GetApiKeysByUserUid))]
        public async Task<IActionResult> GetApiKeysByUserUid(Guid userUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(GetApiKeysByUserUid)}()");
            return Ok(await _apiKeyService.GetApiKeysByUserUidAsync(userUid));
        }

        /// <summary>
        /// Обновление своего ключа
        /// </summary>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(RefreshApiMyKey), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPut(nameof(RefreshApiMyKey))]
        public async Task<IActionResult> RefreshApiMyKey()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(RefreshApiMyKey)}()");

            Guid? userUid = GetClaimUserUid();
            if (userUid is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            User user = GetEntity(await _userService.GetUserByUidAsync((Guid)userUid));
            ApiKey apiKeyOld = GetEntity(await _apiKeyService.GetApiKeyByUserUidAsync(user.Uid));

            if (!GetBool(await _apiKeyService.DeactivedApiKey(new List<Guid>() { (Guid)userUid }, false)))
                return BadRequest(new ServiceResponse<string>(null, false));

            string key = GetEntity(await _apiKeyService.CreateApiKey(user.Uid, user.Login));
            ApiKey apiKey = GetEntity(await _apiKeyService.GetApiKeyAsync(key));

            List<WhiteListIp> whiteListIps = GetEntity(await _ipService.GetWhiteIpsByIpAsync(ipAddress));
            WhiteListIp whiteListIp = whiteListIps.FirstOrDefault(wli => wli.ApiKeyUid == apiKeyOld.Uid);

            if (!GetBool(await _ipService.UpdateApiKeyUidAsync(whiteListIp.Uid, apiKey.Uid)))
                return BadRequest(new ServiceResponse<string>(null, false));
            var response = new ServiceResponse<string>(key, true);
            return Ok(response);
        }

        /// <summary>
        /// Обновление ключа некоторым пользователям
        /// </summary>
        /// <param name="uidUsers">Для админа, который будет настраивать вход для других</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(RefreshApiKeyForUsers), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Roles = Constants.Admin, Policy = Constants.TokenPolicy)]
        [HttpPut(nameof(RefreshApiKeyForUsers))]
        public async Task<IActionResult> RefreshApiKeyForUsers(List<Guid> uidUsers)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(RefreshApiKeyForUsers)}()");

            if (uidUsers is null || uidUsers.Count == 0)
                return NotFound(new ServiceResponse<Dictionary<Guid, string>>(null, false));

            Dictionary<Guid, string> keys = new Dictionary<Guid, string>();
            User user = null;
            foreach (var uid in uidUsers)
            {
                user = GetEntity(await _userService.GetUserByUidAsync(uid));
                if (!GetBool(await _apiKeyService.DeactivedApiKey(new List<Guid>() { user.Uid }, false)))
                    return BadRequest(new ServiceResponse<Dictionary<Guid, string>>(null, false));
                string key = GetEntity(await _apiKeyService.CreateApiKey(uid, user.Login));
                keys.Add(uid, key);
            }
            var response = new ServiceResponse<Dictionary<Guid, string>>(keys, true);
            return Ok(response);
        }

        /// <summary>
        /// Обновление ключа всем пользователям
        /// </summary>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(RefreshApiKeyAllUsers), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Roles = Constants.Admin, Policy = Constants.TokenPolicy)]
        [HttpPut(nameof(RefreshApiKeyAllUsers))]
        public async Task<IActionResult> RefreshApiKeyAllUsers()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(RefreshApiKeyAllUsers)}()");

            Dictionary<Guid, string> keys = new Dictionary<Guid, string>();
            List<User> users = GetEntity(await _userService.GetAllUsersAsync());

            if (!GetBool(await _apiKeyService.DeactivedApiKey(users.Select(u => u.Uid).ToList(), true)))
                return BadRequest(new ServiceResponse<Dictionary<Guid, string>>(null, false));

            foreach (var user in users)
            {
                string key = GetEntity(await _apiKeyService.CreateApiKey(user.Uid, user.Login));
                keys.Add(user.Uid, key);
            }
            var response = new ServiceResponse<Dictionary<Guid, string>>(keys, true);
            return Ok(response);
        }

        /// <summary>
        /// Деактивация ключа
        /// </summary>
        /// <param name="apiKeyUid">UID апи ключа</param>
        /// <param name="isAll">Деактивировать всем, при утечке данных</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(DeactivedApiKey), EntitiesEnum.ApiKey, PartHttpContextEnum.RequestParameter, nameof(apiKeyUid))]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)] 
        [HttpPut(nameof(DeactivedApiKey))]
        public async Task<IActionResult> DeactivedApiKey(Guid apiKeyUid, bool? isAll)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(DeactivedApiKey)}()");

            if (!GetBool(await _apiKeyService.DeactivedApiKey(apiKeyUid, isAll)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        /// <summary>
        /// Деактивация ключей(Устарел)
        /// </summary>
        /// <param name="uidUsers">Для админа, который будет настраивать вход для других</param>
        /// <param name="isAll">Деактивировать всем, при утечке данных</param>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(DeactivedApiKey), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)] //нужна политика для действий, которые может делать или админ или создатель юзер
        [HttpPut(nameof(DeactivedApiKeyByUsers))]
        public async Task<IActionResult> DeactivedApiKeyByUsers(List<Guid>? uidUsers, bool? isAll)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(DeactivedApiKey)}()");

            if (!GetBool(await _apiKeyService.DeactivedApiKey(uidUsers, isAll)))
                return BadRequest(new ServiceResponse<string>(null, false));
            return Ok(new ServiceResponse<string>(null, true));
        }

        /// <summary>
        /// Удаление ключ(не сделано)
        /// </summary>
        /// <returns></returns>
        [Monitoring(MicroservicesEnum.Auth, ControllersEnum.ApiKeyController, nameof(DeleteApiKey), EntitiesEnum.ApiKey, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)] //нужна политика для действий, которые может делать или админ или создатель юзер
        [HttpDelete(nameof(DeleteApiKey))]
        public async Task<IActionResult> DeleteApiKey()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(ApiKeyController)}.{nameof(DeleteApiKey)}()");
            //TODO
            return Ok(new ServiceResponse<string>(null, true));
        }
    }
}
