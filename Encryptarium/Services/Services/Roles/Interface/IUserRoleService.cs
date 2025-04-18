using BusinessLogic.Entities;

namespace BusinessLogic.Services.Roles.Interface
{
    public interface IUserRoleService
    {
        public Task<ServiceResponse<bool>> AddUserInGroupRoleAsync(Guid userUid, Guid roleUid);
        public Task<ServiceResponse<bool>> RemoveUserInGroupRoleAsync(Guid userUid, Guid roleUid);
    }
}
