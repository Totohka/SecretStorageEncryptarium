using BusinessLogic.Entities;
using BusinessLogic.Services.Policies.Interface;
using BusinessLogic.Services.Roles.Interface;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Users.Interface;
using Encryptarium.Access.Attributes;
using Encryptarium.Access.Controllers.Base;
using Encryptarium.Access.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Access.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserRightController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IStoragePolicyService _storagePolicyService;
        private readonly ISecretPolicyService _secretPolicyService;
        private readonly ILogger<UserRightController> _logger;
        public UserRightController(IUserService userService, 
                                   IStoragePolicyService storagePolicyService, 
                                   ISecretPolicyService secretPolicyService, 
                                   IAccessTokenService accessTokenService,
                                   IRoleService roleService,
                                   ILogger<UserRightController> logger) : base(accessTokenService, logger)
        {
            _userService = userService;
            _storagePolicyService = storagePolicyService;
            _secretPolicyService = secretPolicyService;
            _roleService = roleService;
            _logger = logger;
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(UpdateUserRightAsync), EntitiesEnum.User, PartHttpContextEnum.RequestBody, "uid")]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPut(nameof(UpdateUserRightAsync))]
        public async Task<IActionResult> UpdateUserRightAsync(UpdateRightUserDTO updateRightUserDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(UpdateUserRightAsync)}()");

            await _userService.SetIsAdminAsync(updateRightUserDTO.Uid, updateRightUserDTO.IsAdmin);
            await _userService.SetIsCreateAsync(updateRightUserDTO.Uid, updateRightUserDTO.IsCreateStorage);
            var changeAuthMethod = new ChangeMethodAuthDTO() 
            { 
                IsApiKey = updateRightUserDTO.IsApiKey,
                IsOAuth = updateRightUserDTO.IsOAuth,
                IsUserPass = updateRightUserDTO.IsUserPass
            };
            await _userService.ChangeAuthMethodAsync(updateRightUserDTO.Uid, changeAuthMethod);
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPut(nameof(SharingStorageForUser))]
        public async Task<IActionResult> SharingStorageForUser(UpdateRightUserDTO updateRightUserDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(SharingStorageForUser)}()");
            //TODO
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPut(nameof(SharingSecretForUser))]
        public async Task<IActionResult> SharingSecretForUser(UpdateRightUserDTO updateRightUserDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(SharingSecretForUser)}()");
            //TODO
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(ChangeAccessForGroupStorage), EntitiesEnum.Storage, PartHttpContextEnum.RequestParameter, nameof(storageUid))]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPut(nameof(ChangeAccessForGroupStorage))]
        public async Task<IActionResult> ChangeAccessForGroupStorage(Guid storageUid, ChangeAccessForGroupStorageDTO dto)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(ChangeAccessForGroupStorage)}()");

            return Ok(await _roleService.ChangeAccessInGroupStorage(storageUid, dto));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(GetRightForSecret), EntitiesEnum.Secret, PartHttpContextEnum.RequestParameter, nameof(secretUid))]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetRightForSecret))]
        public async Task<IActionResult> GetRightForSecret(Guid secretUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(GetRightForSecret)}()");
            return Ok(await _secretPolicyService.GetRightUserForSecret((Guid)GetClaimUserUid(), secretUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(GetRightForStorage), EntitiesEnum.Storage, PartHttpContextEnum.RequestParameter, nameof(storageUid))]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetRightForStorage))]
        public async Task<IActionResult> GetRightForStorage(Guid storageUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(GetRightForStorage)}()");
            return Ok(await _storagePolicyService.GetRightUserForStorage((Guid)GetClaimUserUid(), storageUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(GetRightUserForStorage), EntitiesEnum.Storage, PartHttpContextEnum.RequestParameter, nameof(storageUid))]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetRightUserForStorage))]
        public async Task<IActionResult> GetRightUserForStorage(Guid userUid, Guid storageUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(GetRightUserForStorage)}()");
            return Ok(await _storagePolicyService.GetRightUserForStorage(userUid, storageUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(GetRightStorage), EntitiesEnum.Storage, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetRightStorage))]
        public async Task<IActionResult> GetRightStorage()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(GetRightForStorage)}()");
            Guid userUid = (Guid)GetClaimUserUid();
            return Ok(await _storagePolicyService.GetRightStorageAsync(userUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.UserRightController, nameof(GetRightSecret), EntitiesEnum.Secret, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet(nameof(GetRightSecret))]
        public async Task<IActionResult> GetRightSecret()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(UserRightController)}.{nameof(GetRightForStorage)}()");
            Guid userUid = (Guid)GetClaimUserUid();
            return Ok(await _secretPolicyService.GetRightSecretAsync(userUid));
        }
    }
}
