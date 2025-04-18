using BusinessLogic.Entities;
using BusinessLogic.Services.Roles.Interface;
using BusinessLogic.Services.Tokens.Interface;
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
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService,
                              IAccessTokenService accessTokenService,
                              ILogger<RoleController> logger) 
            : base(accessTokenService, logger)
        {
            _roleService = roleService;
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(GetRole), EntitiesEnum.Role, PartHttpContextEnum.RequestParameter, nameof(roleUid))]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpGet("{roleUid}")]
        public async Task<IActionResult> GetRole(Guid roleUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(GetRole)}()");

            return Ok(await _roleService.GetRoleAsync(roleUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(GetAllRole), EntitiesEnum.Role, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllRole(string? name)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(GetAllRole)}()");

            return Ok(await _roleService.GetAllRoleAsync(name));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(CreateRole), EntitiesEnum.Role, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPost(nameof(CreateRole))]
        public async Task<IActionResult> CreateRole(CreateRoleDTO createRoleDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(CreateRole)}()");

            return Ok(await _roleService.CreateRoleAsync(createRoleDTO.Name, createRoleDTO.Description, createRoleDTO.RoleTypeUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(CreateGroupRoleAndStorage), EntitiesEnum.Role, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPost(nameof(CreateGroupRoleAndStorage))]
        public async Task<IActionResult> CreateGroupRoleAndStorage(GroupRoleDTO groupRoleDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(CreateGroupRoleAndStorage)}()");
            Guid userUid = GetClaimUserUid().Value;
            return Ok(await _roleService.CreateGroupRoleAndStorage(groupRoleDTO, userUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(DeactivateRole), EntitiesEnum.Role, PartHttpContextEnum.RequestParameter, nameof(roleUid))]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPut("{roleUid}")]
        public async Task<IActionResult> DeactivateRole(Guid roleUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(DeactivateRole)}()");

            return Ok(await _roleService.DeactivateRoleAsync(roleUid));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(UpdateRole), EntitiesEnum.Role, PartHttpContextEnum.RequestBody, "uid")]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPut(nameof(UpdateRole))]
        public async Task<IActionResult> UpdateRole(UpdateRoleDTO updateRoleDTO)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(UpdateRole)}()");

            return Ok(await _roleService.UpdateRoleAsync(updateRoleDTO.Uid, updateRoleDTO.Name, updateRoleDTO.Description));
        }

        [Monitoring(MicroservicesEnum.Access, ControllersEnum.RoleController, nameof(DeleteRole), EntitiesEnum.Role, PartHttpContextEnum.RequestParameter, nameof(roleUid))]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpDelete("{roleUid}")]
        public async Task<IActionResult> DeleteRole(Guid roleUid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(RoleController)}.{nameof(DeleteRole)}()");

            return Ok(await _roleService.DeleteRoleAsync(roleUid));
        }
    }
}
