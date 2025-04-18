using DAL.Repositories.Base.Implementation;
using DAL.Repositories.UserRoles.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using System.Linq.Expressions;

namespace DAL.Repositories.UserRoles.Implementation
{
    public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public new async Task<UserRole> GetAsync(Guid userUid, Guid roleUid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.UserRoles.FindAsync(userUid, roleUid);
        }

        public new async Task<List<UserRole>> GetAsync(Expression<Func<UserRole, bool>> predicate)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.UserRoles.Where(predicate).ToListAsync();
        }
    }
}
