using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Roles.Interface
{
    public interface IRoleService
    {
        public Task<ServiceResponse<Role>> GetRoleAsync(Guid uid);
        public Task<ServiceResponse<List<Role>>> GetAllRoleAsync(string? name);
        public Task<ServiceResponse<bool>> CreateRoleAsync(string name, string description, Guid roleTypeUid);
        public Task<ServiceResponse<Guid>> CreateGroupRoleAndStorage(GroupRoleDTO groupRoleDTO, Guid userUid);
        public Task<ServiceResponse<bool>> RemoveUserInGroupRole(Guid roleUid, Guid ownerUid, Guid userUid);
        public Task<ServiceResponse<bool>> AddUserInGroupRole(Guid roleUid, Guid ownerUid, Guid userUid, bool isMain);
        public Task<ServiceResponse<bool>> ChangeAccessInGroupStorage(Guid storageUid, ChangeAccessForGroupStorageDTO dto);
        public Task<ServiceResponse<bool>> UpdateRoleAsync(Guid uid, string name, string description);
        public Task<ServiceResponse<bool>> DeactivateRoleAsync(Guid uid);
        public Task<ServiceResponse<bool>> DeleteRoleAsync(Guid uid);
    }
}
