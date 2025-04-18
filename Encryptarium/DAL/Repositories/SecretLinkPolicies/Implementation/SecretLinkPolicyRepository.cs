using DAL.Repositories.Base.Implementation;
using DAL.Repositories.SecretLinkPolicies.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using System.Xml.Linq;

namespace DAL.Repositories.SecretLinkPolicies.Implementation
{
    public class SecretLinkPolicyRepository : BaseRepository<SecretLinkPolicy>, ISecretLinkPolicyRepository
    {
        public SecretLinkPolicyRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {

        }

        /// <summary>
        /// Для модели SecretLinkPolicy не используется
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override Task<SecretLinkPolicy> GetAsync(Guid uid)
        {
            throw new Exception("Выбран некорректный метод GetAsync для модели SecretLinkPolicy");
        }

        /// <summary>
        /// Метод удаления сущности SecretLinkPolicy
        /// </summary>
        /// <param name="secretUid">Идентификатор секрета</param>
        /// <param name="userUid">Идентификатор юзера</param>
        public async Task DeleteAsync(Guid secretUid, Guid userUid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            SecretLinkPolicy secretLinkPolicy = await db.SecretLinkPolicies.FindAsync(userUid, secretUid); ;
            db.SecretLinkPolicies.Remove(secretLinkPolicy);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Метод получения сущности SecretLinkPolicy
        /// </summary>
        /// <param name="secretUid">Идентификатор секрета</param>
        /// <param name="userUid">Идентификатор юзера</param>
        /// <returns>Сущность SecretLinkPolicy</returns>
        public async Task<SecretLinkPolicy> GetAsync(Guid secretUid, Guid userUid)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            return await db.SecretLinkPolicies.FindAsync(userUid, secretUid); ;
        }
    }
}
