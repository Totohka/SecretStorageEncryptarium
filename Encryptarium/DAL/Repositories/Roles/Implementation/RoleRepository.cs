using DAL.Repositories.Base.Implementation;
using DAL.Repositories.Roles.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.Roles.Implementation
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
        public new async Task<Guid> CreateAsync(Role role)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.Roles.AddAsync(role);
            await db.SaveChangesAsync();
            return role.Uid;
        }
    }
}
