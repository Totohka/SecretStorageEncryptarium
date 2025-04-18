using DAL.Repositories.Base.Implementation;
using DAL.Repositories.StoragePolicies.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.StoragePolicies.Implementation
{
    public class StoragePolicyRepository : BaseRepository<StoragePolicy>, IStoragePolicyRepository
    {
        public StoragePolicyRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {

        }

        public async override Task<Guid> CreateAsync(StoragePolicy storagePolicy)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.StoragePolicies.AddAsync(storagePolicy);
            await db.SaveChangesAsync();
            return storagePolicy.Uid;
        }
    }
}
