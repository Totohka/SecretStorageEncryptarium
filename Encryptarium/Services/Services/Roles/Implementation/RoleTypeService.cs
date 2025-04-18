using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;

namespace BusinessLogic.Services.Roles.Implementation
{
    public class RoleTypeService : BaseService, IRoleTypeService
    {
        private readonly IRoleTypeRepository _roleTypeRepository;
        public RoleTypeService(IRoleTypeRepository roleTypeRepository,
                               ILogger<RoleTypeService> logger) : base(logger)
        {
            _roleTypeRepository = roleTypeRepository;
        }
        public async Task<ServiceResponse<bool>> CreateRoleTypeAsync(string name, int code)
        {
            await _roleTypeRepository.CreateAsync(new RoleType() { Name = name, Code = code });
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeactivateRoleTypeAsync(Guid uid)
        {
            RoleType roleType = await _roleTypeRepository.GetAsync(uid);
            roleType.IsActive = false;
            await _roleTypeRepository.UpdateAsync(roleType);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteRoleTypeAsync(Guid uid)
        {
            await _roleTypeRepository.DeleteAsync(uid);
            return Ok(true);
        }

        public async Task<ServiceResponse<List<RoleType>>> GetAllRoleTypeAsync()
        {
            return Ok(await _roleTypeRepository.GetAllAsync());
        }

        public async Task<ServiceResponse<RoleType>> GetRoleTypeAsync(Guid uid)
        {
            return Ok(await _roleTypeRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<RoleType>> GetRoleTypeAsync(int code)
        {
            return Ok(await _roleTypeRepository.GetAsync(rt => rt.Code == code));
        }

        public async Task<ServiceResponse<bool>> UpdateRoleTypeAsync(Guid uid, string name, int code)
        {
            RoleType roleType = await _roleTypeRepository.GetAsync(uid);
            roleType.Name = name;
            roleType.Code = code;
            await _roleTypeRepository.UpdateAsync(roleType);
            return Ok(true);
        }
    }
}
