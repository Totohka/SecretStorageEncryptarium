using DAL.Repositories.Base.Implementation;
using DAL.Repositories.Audits.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.Audits.Implementation
{
    public class AuditRepository : BaseRepository<Audit>, IAuditRepository
    {
        public AuditRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
    }
}
