using BusinessLogic.Services.Roles.Interface;
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
    public class RoleTypeController : ControllerBase
    {
        private readonly ILogger<RoleTypeController> _logger;
        private readonly IRoleTypeService _roleTypeService;
        public RoleTypeController(IRoleTypeService roleTypeService, ILogger<RoleTypeController> logger)
        {
            _roleTypeService = roleTypeService;
            _logger = logger;
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleTypeController, nameof(GetRoleType), EntitiesEnum.RoleType, PartHttpContextEnum.RequestParameter, nameof(roleTypeUid))]
        [HttpGet("{roleTypeUid}")]
        public async Task<IActionResult> GetRoleType(Guid roleTypeUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleTypeController)}.{nameof(GetRoleType)}()");

            return Ok(await _roleTypeService.GetRoleTypeAsync(roleTypeUid));
        } 

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleTypeController, nameof(GetAllRoleType), EntitiesEnum.RoleType, PartHttpContextEnum.None)]
        [HttpGet]
        public async Task<IActionResult> GetAllRoleType()
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleTypeController)}.{nameof(GetAllRoleType)}()");

            return Ok(await _roleTypeService.GetAllRoleTypeAsync());
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleTypeController, nameof(CreateRoleType), EntitiesEnum.RoleType, PartHttpContextEnum.None)]
        [HttpPost]
        public async Task<IActionResult> CreateRoleType(CreateRoleTypeDTO createRoleTypeDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleTypeController)}.{nameof(CreateRoleType)}()");

            return Ok(await _roleTypeService.CreateRoleTypeAsync(createRoleTypeDTO.Name, createRoleTypeDTO.Code));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleTypeController, nameof(UpdateRoleType), EntitiesEnum.RoleType, PartHttpContextEnum.RequestBody, "uid")]
        [HttpPut]
        public async Task<IActionResult> UpdateRoleType(UpdateRoleTypeDTO updateRoleTypeDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleTypeController)}.{nameof(UpdateRoleType)}()");

            return Ok(await _roleTypeService.UpdateRoleTypeAsync(updateRoleTypeDTO.Uid, updateRoleTypeDTO.Name, updateRoleTypeDTO.Code));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleTypeController, nameof(DeactivateRoleType), EntitiesEnum.RoleType, PartHttpContextEnum.RequestParameter, nameof(roleTypeUid))]
        [HttpPut("{roleTypeUid}")]
        public async Task<IActionResult> DeactivateRoleType(Guid roleTypeUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleTypeController)}.{nameof(DeactivateRoleType)}()");

            return Ok(await _roleTypeService.DeactivateRoleTypeAsync(roleTypeUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleTypeController, nameof(DeleteRoleType), EntitiesEnum.RoleType, PartHttpContextEnum.RequestParameter, nameof(roleTypeUid))]
        [HttpDelete("{roleTypeUid}")]
        public async Task<IActionResult> DeleteRoleType(Guid roleTypeUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleTypeController)}.{nameof(DeleteRoleType)}()");

            return Ok(await _roleTypeService.DeleteRoleTypeAsync(roleTypeUid));
        }
    }
}
