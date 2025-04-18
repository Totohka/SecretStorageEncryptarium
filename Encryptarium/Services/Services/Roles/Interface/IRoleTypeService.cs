using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Roles.Interface
{
    public interface IRoleTypeService
    {
        public Task<ServiceResponse<RoleType>> GetRoleTypeAsync(Guid uid);
        public Task<ServiceResponse<RoleType>> GetRoleTypeAsync(int code);
        public Task<ServiceResponse<List<RoleType>>> GetAllRoleTypeAsync();
        public Task<ServiceResponse<bool>> CreateRoleTypeAsync(string name, int code);
        public Task<ServiceResponse<bool>> UpdateRoleTypeAsync(Guid uid, string name, int code);
        public Task<ServiceResponse<bool>> DeactivateRoleTypeAsync(Guid uid);
        public Task<ServiceResponse<bool>> DeleteRoleTypeAsync(Guid uid);
    }
}
