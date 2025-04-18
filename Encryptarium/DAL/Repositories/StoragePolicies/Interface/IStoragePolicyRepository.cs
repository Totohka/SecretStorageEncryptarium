using DAL.Repositories.Base.Interface;
using Microsoft.EntityFrameworkCore.Internal;
using Model.Entities;

namespace DAL.Repositories.StoragePolicies.Interface
{
    public interface IStoragePolicyRepository : IBaseRepository<StoragePolicy>
    {
        public new Task<Guid> CreateAsync(StoragePolicy item);
    }
}
