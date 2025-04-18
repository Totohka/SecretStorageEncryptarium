using DAL.Repositories.Base.Implementation;
using DAL.Repositories.SecretPolicies.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.SecretPolicies.Implementation
{
    public class SecretPolicyRepository : BaseRepository<SecretPolicy>, ISecretPolicyRepository
    {
        public SecretPolicyRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
        public async override Task<Guid> CreateAsync(SecretPolicy secretPolicy)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.SecretPolicies.AddAsync(secretPolicy);
            await db.SaveChangesAsync();
            return secretPolicy.Uid;
        }
    }
}
