using DAL.Repositories.Base.Implementation;
using DAL.Repositories.Users.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.Users.Implementation
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public new async Task<Guid> CreateAsync(User user)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return user.Uid;
        }
    }
}
