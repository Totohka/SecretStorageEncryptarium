using DAL.Repositories.Base.Implementation;
using DAL.Repositories.StorageLinkPolicies.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using System.Linq.Expressions;

namespace DAL.Repositories.StorageLinkPolicies.Implementation
{
    public class StorageLinkPolicyRepository : BaseRepository<StorageLinkPolicy>, IStorageLinkPolicyRepository
    {
        public StorageLinkPolicyRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        /// <summary>
        /// Для модели StorageLinkPolicy не используется
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task<StorageLinkPolicy> GetAsync(Guid uid)
        {
            throw new Exception("Выбран некорректный метод GetAsync для модели SecretLinkPolicy");
        }

        /// <summary>
        /// Метод удаления сущности StorageLinkPolicy
        /// </summary>
        /// <param name="storageUid">Идентификатор хранилища</param>
        /// <param name="roleUid">Идентификатор роли</param>
        public async Task DeleteAsync(Guid storageUid, Guid roleUid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            StorageLinkPolicy storageLinkPolicy = await db.StorageLinkPolicies.FindAsync(roleUid, storageUid );
            db.StorageLinkPolicies.Remove(storageLinkPolicy);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Метод получения сущности StorageLinkPolicy
        /// </summary>
        /// <param name="storageUid">Идентификатор хранилища</param>
        /// <param name="roleUid">Идентификатор роли</param>
        /// <returns>Сущность SecretLinkPolicy</returns>
        public async Task<StorageLinkPolicy> GetAsync(Guid storageUid, Guid roleUid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.StorageLinkPolicies.FindAsync(roleUid, storageUid);
        }

        public new async Task<List<StorageLinkPolicy>> GetAsync(Expression<Func<StorageLinkPolicy, bool>> predicate)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.StorageLinkPolicies.Where(predicate).ToListAsync();
        }
    }
}
