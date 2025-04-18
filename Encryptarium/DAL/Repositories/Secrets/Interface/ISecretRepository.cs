using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.Secrets.Interface
{
    public interface ISecretRepository : IBaseRepository<Secret>
    {
        public new Task<Guid> CreateAsync(Secret secret);
    }
}
