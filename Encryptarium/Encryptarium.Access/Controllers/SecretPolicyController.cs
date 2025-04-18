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
    public class SecretPolicyController : ControllerBase
    {
        private readonly ISecretPolicyService _secretPolicyService;
        private readonly ILogger<SecretPolicyController> _logger;
        public SecretPolicyController(ISecretPolicyService secretPolicyService, ILogger<SecretPolicyController> logger)
        {
            _secretPolicyService = secretPolicyService;
            _logger = logger;
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(GetSecretPolicy), EntitiesEnum.SecretPolicy, PartHttpContextEnum.RequestParameter, nameof(secretPolicyUid))]
        [HttpGet("{secretPolicyUid}")]
        public async Task<IActionResult> GetSecretPolicy(Guid secretPolicyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(GetSecretPolicy)}()");

            return Ok(await _secretPolicyService.GetSecretPolicyAsync(secretPolicyUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(GetAllSecretPolicy), EntitiesEnum.SecretPolicy, PartHttpContextEnum.None)]
        [HttpGet]
        public async Task<IActionResult> GetAllSecretPolicy()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(GetAllSecretPolicy)}()");

            return Ok(await _secretPolicyService.GetAllSecretPolicyAsync());
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(GetSecretLinkPolicy), EntitiesEnum.SecretLinkPolicy, PartHttpContextEnum.None)]
        [HttpGet("{secretUid}/{roleUid}")]
        public async Task<IActionResult> GetSecretLinkPolicy(Guid secretUid, Guid roleUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(GetSecretLinkPolicy)}()");

            return Ok(await _secretPolicyService.GetLinkAsync(roleUid, secretUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(CreateLink), EntitiesEnum.SecretLinkPolicy, PartHttpContextEnum.None)]
        [HttpPost(nameof(CreateLink))]
        public async Task<IActionResult> CreateLink(CreateLinkSecretDTO createLinkSecretDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(CreateLink)}()");

            return Ok(await _secretPolicyService.CreateLinkAsync(createLinkSecretDTO.RoleUid, createLinkSecretDTO.SecretPolicyUid, createLinkSecretDTO.SecretUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(CreateSecretPolicy), EntitiesEnum.SecretPolicy, PartHttpContextEnum.ResponseBody, "data")]
        [HttpPost(nameof(CreateSecretPolicy))]
        public async Task<IActionResult> CreateSecretPolicy(CreateSecretPolicyDTO createSecretPolicyDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(CreateSecretPolicy)}()");

            DiscretionaryAccessModelSecret accessModelSecret = new DiscretionaryAccessModelSecret() 
            { 
                IsDelete = createSecretPolicyDTO.IsDelete,
                IsRead = createSecretPolicyDTO.IsRead,
                IsUpdate = createSecretPolicyDTO.IsUpdate,
            };
            return Ok(await _secretPolicyService.CreateSecretPolicyAsync(createSecretPolicyDTO.Title, createSecretPolicyDTO.Description, accessModelSecret));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(UpdateSecretPolicy), EntitiesEnum.SecretPolicy, PartHttpContextEnum.RequestBody, "uid")]
        [HttpPut]
        public async Task<IActionResult> UpdateSecretPolicy(UpdateSecretPolicyDTO updateSecretPolicyDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(UpdateSecretPolicy)}()");

            return Ok(await _secretPolicyService.UpdateSecretPolicyAsync(updateSecretPolicyDTO.Uid, updateSecretPolicyDTO.Title, updateSecretPolicyDTO.Description));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(DeactivateSecretPolicy), EntitiesEnum.SecretPolicy, PartHttpContextEnum.RequestParameter, nameof(secretPolicyUid))]
        [HttpPut("{secretPolicyUid}")]
        public async Task<IActionResult> DeactivateSecretPolicy(Guid secretPolicyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(DeactivateSecretPolicy)}()");

            return Ok(await _secretPolicyService.DeactivateSecretPolicyAsync(secretPolicyUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.SecretPolicyController, nameof(DeleteSecretPolicy), EntitiesEnum.SecretPolicy, PartHttpContextEnum.RequestParameter, nameof(secretPolicyUid))]
        [HttpDelete("{secretPolicyUid}")]
        public async Task<IActionResult> DeleteSecretPolicy(Guid secretPolicyUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(SecretPolicyController)}.{nameof(DeleteSecretPolicy)}()");

            return Ok(await _secretPolicyService.DeleteSecretPolicyAsync(secretPolicyUid));
        }
    }
}
