using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.SecretPolicies.Interface
{
    public interface ISecretPolicyRepository : IBaseRepository<SecretPolicy>
    {
        public new Task<Guid> CreateAsync(SecretPolicy item);
    }
}
