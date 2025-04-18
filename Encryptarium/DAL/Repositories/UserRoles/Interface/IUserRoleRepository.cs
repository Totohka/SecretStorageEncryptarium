using DAL.Repositories.Base.Interface;
using Model.Entities;
using System.Linq.Expressions;

namespace DAL.Repositories.UserRoles.Interface
{
    public interface IUserRoleRepository : IBaseRepository<UserRole>
    {
        public new Task<UserRole> GetAsync(Guid userUid, Guid roleUid);
        public new Task<List<UserRole>> GetAsync(Expression<Func<UserRole, bool>> predicate);
    }
}
