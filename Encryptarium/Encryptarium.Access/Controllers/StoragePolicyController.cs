using BusinessLogic.Entities;
using BusinessLogic.Services.Policies.Interface;
using Encryptarium.Access.Attributes;
using Encryptarium.Access.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Access.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
    public class StoragePolicyController : ControllerBase
    {
        private readonly ILogger<StoragePolicyController> _logger;
        private readonly IStoragePolicyService _storagePolicyService;
        public StoragePolicyController(IStoragePolicyService storagePolicyService, ILogger<StoragePolicyController> logger)
        {
            _storagePolicyService = storagePolicyService;
            _logger = logger;
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(GetStoragePolicy), EntitiesEnum.StoragePolicy, PartHttpContextEnum.RequestParameter, nameof(storagePolicyUid))]
        [HttpGet("{storagePolicyUid}")]
        public async Task<IActionResult> GetStoragePolicy(Guid storagePolicyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(GetStoragePolicy)}()");

            return Ok(await _storagePolicyService.GetStoragePolicyAsync(storagePolicyUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(GetAllStoragePolicy), EntitiesEnum.StoragePolicy, PartHttpContextEnum.None)]
        [HttpGet]
        public async Task<IActionResult> GetAllStoragePolicy()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(GetAllStoragePolicy)}()");

            return Ok(await _storagePolicyService.GetAllStoragePolicyAsync());
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(GetStoragePolicy), EntitiesEnum.StorageLinkPolicy, PartHttpContextEnum.None)]
        [HttpGet("{storageUid}/{roleUid}")]
        public async Task<IActionResult> GetStorageLinkPolicy(Guid storageUid, Guid roleUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(GetStorageLinkPolicy)}()");

            return Ok(await _storagePolicyService.GetLinkAsync(roleUid, storageUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(CreateLink), EntitiesEnum.StorageLinkPolicy, PartHttpContextEnum.None)]
        [HttpPost(nameof(CreateLink))]
        public async Task<IActionResult> CreateLink(CreateLinkStorageDTO createLinkStorageDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(CreateLink)}()");
           
            return Ok(await _storagePolicyService.CreateLinkAsync(createLinkStorageDTO.RoleUid, createLinkStorageDTO.StoragePolicyUid, createLinkStorageDTO.StorageUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(CreateStoragePolicy), EntitiesEnum.StoragePolicy, PartHttpContextEnum.ResponseBody, "data")]
        [HttpPost(nameof(CreateStoragePolicy))]
        public async Task<IActionResult> CreateStoragePolicy(CreateStoragePolicyDTO createStoragePolicyDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(CreateStoragePolicy)}()");

            DiscretionaryAccessModelStorage accessModelStorage = new DiscretionaryAccessModelStorage()
            {
                IsDelete = createStoragePolicyDTO.IsDelete,
                IsRead = createStoragePolicyDTO.IsRead,
                IsUpdate = createStoragePolicyDTO.IsUpdate,
            };
            return Ok(await _storagePolicyService.CreateStoragePolicyAsync(createStoragePolicyDTO.Title, createStoragePolicyDTO.Description, accessModelStorage));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(UpdateStoragePolicy), EntitiesEnum.StoragePolicy, PartHttpContextEnum.RequestBody, "uid")]
        [HttpPut]
        public async Task<IActionResult> UpdateStoragePolicy(UpdateStoragePolicyDTO updateStoragePolicyDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(UpdateStoragePolicy)}()");

            return Ok(await _storagePolicyService.UpdateStoragePolicyAsync(updateStoragePolicyDTO.Uid, updateStoragePolicyDTO.Title, updateStoragePolicyDTO.Description));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(DeactivateStoragePolicy), EntitiesEnum.StoragePolicy, PartHttpContextEnum.RequestParameter, nameof(storagePolicyUid))]
        [HttpPut("{storagePolicyUid}")]
        public async Task<IActionResult> DeactivateStoragePolicy(Guid storagePolicyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(DeactivateStoragePolicy)}()");

            return Ok(await _storagePolicyService.DeactivateStoragePolicyAsync(storagePolicyUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.StoragePolicyController, nameof(DeleteStoragePolicy), EntitiesEnum.StoragePolicy, PartHttpContextEnum.RequestParameter, nameof(storagePolicyUid))]
        [HttpDelete("{storagePolicyUid}")]
        public async Task<IActionResult> DeleteStoragePolicy(Guid storagePolicyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(StoragePolicyController)}.{nameof(DeleteStoragePolicy)}()");

            return Ok(await _storagePolicyService.DeleteStoragePolicyAsync(storagePolicyUid));
        }
    }
}
