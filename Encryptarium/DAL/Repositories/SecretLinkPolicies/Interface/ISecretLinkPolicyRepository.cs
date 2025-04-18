using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.SecretLinkPolicies.Interface
{
    public interface ISecretLinkPolicyRepository : IBaseRepository<SecretLinkPolicy>
    {
        /// <summary>
        /// Метод удаления сущности SecretLinkPolicy
        /// </summary>
        /// <param name="secretUid"></param>
        /// <param name="userUid"></param>
        public Task<SecretLinkPolicy> GetAsync(Guid secretUid, Guid userUid);

        /// <summary>
        /// Метод получения сущности SecretLinkPolicy
        /// </summary>
        /// <param name="secretUid"></param>
        /// <param name="userUid"></param>
        /// <returns>Сущность SecretLinkPolicy</returns>
        public Task DeleteAsync(Guid secretUid, Guid userUid);

        /// <summary>
        /// Для модели SecretLinkPolicy не используется
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public new Task<SecretLinkPolicy> GetAsync(Guid uid);
    }
}
