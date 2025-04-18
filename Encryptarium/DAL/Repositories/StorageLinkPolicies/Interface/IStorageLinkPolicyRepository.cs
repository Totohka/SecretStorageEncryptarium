using DAL.Repositories.Base.Interface;
using Model.Entities;
using System.Linq.Expressions;

namespace DAL.Repositories.StorageLinkPolicies.Interface
{
    public interface IStorageLinkPolicyRepository : IBaseRepository<StorageLinkPolicy>
    {

        /// <summary>
        /// Метод получения сущности StorageLinkPolicy
        /// </summary>
        /// <param name="storageUid">Идентификатор хранилища</param>
        /// <param name="roleUid">Идентификатор роли</param>
        /// <returns>Сущность StorageLinkPolicy</returns>
        public Task<StorageLinkPolicy> GetAsync(Guid storageUid, Guid roleUid);

        /// <summary>
        /// Метод удаления сущности StorageLinkPolicy
        /// </summary>
        /// <param name="storageUid">Идентификатор хранилища</param>
        /// <param name="roleUid">Идентификатор роли</param>
        public Task DeleteAsync(Guid storageUid, Guid roleUid);

        /// <summary>
        /// Для модели StorageLinkPolicy не используется
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public new Task<StorageLinkPolicy> GetAsync(Guid uid);

        public new Task<List<StorageLinkPolicy>> GetAsync(Expression<Func<StorageLinkPolicy, bool>> predicate);
    }
}
