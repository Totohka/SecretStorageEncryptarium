using DAL.Repositories.Base.Implementation;
using DAL.Repositories.WhiteListIps.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.WhiteListIps.Implementation
{
    public class WhiteListIpRepository : BaseRepository<WhiteListIp>, IWhiteListIpRepository
    {
        public WhiteListIpRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
    }
}
