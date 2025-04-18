using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.Storages.Interface
{
    public interface IStorageRepository : IBaseRepository<Storage>
    {
        public new Task<Guid> CreateAsync(Storage storage);
    }
}
