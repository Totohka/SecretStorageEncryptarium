using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.Users.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public new Task<Guid> CreateAsync(User user);
    }
}
