using BusinessLogic.Entities;
using BusinessLogic.Services.Secrets.Interface;
using BusinessLogic.Services.Tokens.Interface;
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
    public class SecretController : BaseController
    {
        private readonly ISecretService _secretService;
        public SecretController(ISecretService secretService,
                                IAccessTokenService accessTokenService,
                                ILogger<SecretController> logger) : base(accessTokenService, logger)
        {
            _secretService = secretService;
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(Get), EntitiesEnum.Secret, PartHttpContextEnum.RequestParameter, "secretUid")]
        [HttpGet("{secretUid}")]
        [Authorize(Policy = Constants.SecretPolicy)]
        public async Task<IActionResult> Get(Guid secretUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(Get)}()");

            return Ok(await _secretService.GetSecretAsync(secretUid));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(GetAll), EntitiesEnum.Secret, PartHttpContextEnum.None)]
        [HttpGet("all/{storageUid}")]
        [Authorize(Policy = Constants.StoragePolicy)]
        public async Task<IActionResult> GetAll(Guid storageUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(GetAll)}()");

            return Ok(await _secretService.GetAllSecretAsync(storageUid));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(Create), EntitiesEnum.Secret, PartHttpContextEnum.ResponseBody, "data")]
        [HttpPost]
        [Authorize(Policy = Constants.StoragePolicy)]
        public async Task<IActionResult> Create(SecretDTO secretDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(Create)}()");

            Guid? userUidClaim = GetClaimUserUid();
            if (userUidClaim is null)
                return Unauthorized(new ServiceResponse<string>(null, false));
            return Ok(await _secretService.CreateSecretAsync((Guid)userUidClaim, secretDTO));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(Delete), EntitiesEnum.Secret, PartHttpContextEnum.RequestParameter, "secretUid")]
        [HttpDelete("{secretUid}")]
        [Authorize(Policy = Constants.SecretPolicy)]
        public async Task<IActionResult> Delete(Guid secretUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(Delete)}()");

            return Ok(await _secretService.DeleteSecretAsync(secretUid));
        }

        [Monitoring(MicroservicesEnum.Storage, ControllersEnum.SecretController, nameof(Update), EntitiesEnum.Secret, PartHttpContextEnum.RequestParameter, "secretUid")]
        [HttpPut("{secretUid}")]
        [Authorize(Policy = Constants.SecretPolicy)]
        public async Task<IActionResult> Update(Guid secretUid, SecretDTO secretDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AdminKeyController)}.{nameof(Update)}()");

            return Ok(await _secretService.UpdateSecretAsync(secretUid, secretDTO));
        }
    }
}
