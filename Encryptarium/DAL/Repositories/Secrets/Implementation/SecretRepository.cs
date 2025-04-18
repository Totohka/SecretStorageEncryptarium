using DAL.Repositories.Base.Implementation;
using DAL.Repositories.Secrets.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.Secrets.Implementation
{
    public class SecretRepository : BaseRepository<Secret>, ISecretRepository
    {
        public SecretRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
        public new async Task<Guid> CreateAsync(Secret secret)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.Secrets.AddAsync(secret);
            await db.SaveChangesAsync();
            return secret.Uid;
        }
    }
}
