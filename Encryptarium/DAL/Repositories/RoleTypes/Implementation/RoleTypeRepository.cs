using DAL.Repositories.Base.Implementation;
using DAL.Repositories.RoleTypes.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.RoleTypes.Implementation
{
    public class RoleTypeRepository : BaseRepository<RoleType>, IRoleTypeRepository
    {
        public RoleTypeRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
    }
}
