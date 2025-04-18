using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Roles.Interface;
using DAL.Repositories.Roles.Interface;
using DAL.Repositories.RoleTypes.Interface;
using DAL.Repositories.UserRoles.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;

namespace BusinessLogic.Services.Roles.Implementation
{
    public class UserRoleService : BaseService, IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleTypeRepository _roleTypeRepository;
        public UserRoleService(IUserRoleRepository userRoleRepository,
                               IRoleRepository roleRepository,
                               IRoleTypeRepository roleTypeRepository,
                               ILogger<UserRoleService> logger) : base(logger)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _roleTypeRepository = roleTypeRepository;
        }

        public async Task<ServiceResponse<bool>> AddUserInGroupRoleAsync(Guid userUid, Guid roleUid)
        {
            var role = await _roleRepository.GetAsync(roleUid);
            var roleType = await _roleTypeRepository.GetAsync(role.RoleTypeUid);
            if (roleType.Code == 1) //Если роль является личной
                return Ok(false);
            await _userRoleRepository.CreateAsync(new UserRole { UserUid = userUid, RoleUid = roleUid });
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> RemoveUserInGroupRoleAsync(Guid userUid, Guid roleUid)
        {
            var role = await _roleRepository.GetAsync(roleUid);
            var roleType = await _roleTypeRepository.GetAsync(role.RoleTypeUid);
            if (roleType.Code == 1) //Если роль является личной
                return Ok(false);
            await _userRoleRepository.DeleteAsync(ur => ur.UserUid == userUid && ur.RoleUid == roleUid);
            return Ok(true);
        }
    }
}
