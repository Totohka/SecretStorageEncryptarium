using BusinessLogic.Entities;
using BusinessLogic.Services.Storages.Interface;
using BusinessLogic.Services.Tokens.Interface;
using Encryptarium.Storage.Attributes;
using Encryptarium.Storage.Controllers.Base;
using Encryptarium.Storage.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Storage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StorageController : BaseController
    {
        private readonly IStorageService _storageService;
        public StorageController(IStorageService storageService,
                                 IAccessTokenService accessTokenService,
                                 ILogger<StorageController> logger) : base(accessTokenService, logger) 
        {
            _storageService = storageService;
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(GetStorageUser), EntitiesEnum.Storage, PartHttpContextEnum.ResponseBody, "data")]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet("my")]
        public async Task<IActionResult> GetStorageUser()
        {
            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
            {
                return Unauthorized(new ServiceResponse<string>(null, false));
            }

            return Ok(await _storageService.GetStorageUserAsync((Guid)userUidClaim));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(GetStorageCommon), EntitiesEnum.Storage, PartHttpContextEnum.ResponseBody, "data")]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpGet("common")]
        public async Task<IActionResult> GetStorageCommon()
        {
            return Ok(await _storageService.GetStorageCommonAsync());
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(Get), EntitiesEnum.Storage, PartHttpContextEnum.RequestParameter, "storageUid")]
        [HttpGet("{storageUid}")]
        [Authorize(Policy = Constants.StoragePolicy)]
        public async Task<IActionResult> Get(Guid storageUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StorageController)}.{nameof(Get)}()");

            return Ok(await _storageService.GetStorage(storageUid));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(GetAll), EntitiesEnum.Storage, PartHttpContextEnum.None)]
        [HttpGet]
        [Authorize(Policy = Constants.TokenPolicy)]
        public async Task<IActionResult> GetAll(int code = (int)TypeStorageEnum.None)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StorageController)}.{nameof(GetAll)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
            {
                return Unauthorized(new ServiceResponse<string>(null, false));
            }

            return Ok(await _storageService.GetAllStorage((Guid)userUidClaim, code));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(Delete), EntitiesEnum.Storage, PartHttpContextEnum.RequestParameter, "storageUid")]
        [HttpDelete("{storageUid}")]
        [Authorize(Policy = Constants.StoragePolicy)]
        public async Task<IActionResult> Delete(Guid storageUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StorageController)}.{nameof(Delete)}()");

            await _storageService.DeleteStorage(storageUid);
            return Ok(new ServiceResponse<string>(null, true));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(Create), EntitiesEnum.Storage, PartHttpContextEnum.ResponseBody, "data")]
        [HttpPost]
        [Authorize(Policy = Constants.UserPolicy)]
        public async Task<IActionResult> Create(StorageDTO storageDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StorageController)}.{nameof(Create)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));

            return Ok(await _storageService.CreateStorage((Guid)userUidClaim, storageDTO.Title, storageDTO.Description));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.StorageController, nameof(Update), EntitiesEnum.Storage, PartHttpContextEnum.RequestParameter, nameof(storageUid))]
        [HttpPut("{storageUid}")]
        [Authorize(Policy = Constants.StoragePolicy)]
        public async Task<IActionResult> Update(Guid storageUid, StorageDTO storageDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StorageController)}.{nameof(Update)}()");

            return Ok(await _storageService.UpdateStorage(storageUid, storageDTO.Title, storageDTO.Description));
        }
    }
}
