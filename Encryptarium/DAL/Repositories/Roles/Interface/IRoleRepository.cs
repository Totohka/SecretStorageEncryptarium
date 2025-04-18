using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.Roles.Interface
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        public new Task<Guid> CreateAsync(Role role);
    }
}
