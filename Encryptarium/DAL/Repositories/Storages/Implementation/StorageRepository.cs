using DAL.Repositories.Base.Implementation;
using DAL.Repositories.Storages.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.Storages.Implementation
{
    public class StorageRepository : BaseRepository<Storage>, IStorageRepository
    {
        public StorageRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public new async Task<Guid> CreateAsync(Storage storage)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.Storages.AddAsync(storage);
            await db.SaveChangesAsync();
            return storage.Uid;
        }
    }
}
