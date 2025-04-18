using BusinessLogic.Entities;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Interface;
using Encryptarium.Storage.Attributes;
using Encryptarium.Storage.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Storage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService,
                              IAccessTokenService accessTokenService,
                              ILogger<UserController> logger) : base(accessTokenService, logger)
        {
            _userService = userService;
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(GetMe), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetMe))]
        public async Task<IActionResult> GetMe()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserController)}.{nameof(GetMe)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));
            var user = await _userService.GetUserByUidAsync(userUidClaim.Value);
            var userDTO = new UserDTO()
            {
                Uid = user.Data.Uid,
                Email = user.Data.Email,
                Login = user.Data.Login,
                DateCreate = user.Data.DateCreate,
                IsAdmin = user.Data.IsAdmin,
                IsCreateStorage = user.Data.IsCreateStorage,
                IsCreateGroupRole = user.Data.IsCreateGroupRole,
                IsActive = user.Data.IsActive
            };
            return Ok(new ServiceResponse<UserDTO>(userDTO));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(GetByUid), EntitiesEnum.User, PartHttpContextEnum.RequestParameter, "uid")]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet("{uid}")]
        public async Task<IActionResult> GetByUid(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserController)}.{nameof(GetByUid)}()");
            var user = await _userService.GetUserByUidAsync(uid);
            var userDTO = new UserDTO()
            {
                Uid = user.Data.Uid,
                Email = user.Data.Email,
                Login = user.Data.Login,
                DateCreate = user.Data.DateCreate,
                IsAdmin = user.Data.IsAdmin,
                IsCreateStorage = user.Data.IsCreateStorage,
                IsCreateGroupRole = user.Data.IsCreateGroupRole,
                IsActive = user.Data.IsActive
            };
            return Ok(new ServiceResponse<UserDTO>(userDTO));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(GetAll), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPost]
        public async Task<IActionResult> GetAll(FilterUser filterUser)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserController)}.{nameof(GetAll)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));
            return Ok(await _userService.GetAllUsersAsync((Guid)userUidClaim, filterUser, true));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(GetAll), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPost(nameof(GetAllForAdmin))]
        public async Task<IActionResult> GetAllForAdmin(FilterUser filterUser)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserController)}.{nameof(GetAll)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));
            return Ok(await _userService.GetAllUsersAsync((Guid)userUidClaim, filterUser, false));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(GetUsersByStorage), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetUsersByStorage))]
        public async Task<IActionResult> GetUsersByStorage(Guid storageUid)
        {
            return Ok(await _userService.GetUsersByStorageAsync(storageUid));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(ChangeEmail), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet("change/email/{email}")]
        public async Task<IActionResult> ChangeEmail(string email)
        {
            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));
            return Ok(await _userService.ChangeEmailAsync(userUidClaim.Value, email));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.UserController, nameof(ChangeEmail), EntitiesEnum.User, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet("change/login/{login}")]
        public async Task<IActionResult> ChangeLogin(string login)
        {
            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));
            return Ok(await _userService.ChangeLoginAsync(userUidClaim.Value, login));
        }
    }
}
