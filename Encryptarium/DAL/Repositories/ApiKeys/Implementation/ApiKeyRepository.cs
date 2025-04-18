using DAL.Repositories.ApiKeys.Interface;
using DAL.Repositories.Base.Implementation;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.ApiKeys.Implementation
{
    public class ApiKeyRepository : BaseRepository<ApiKey>, IApiKeyRepository
    {
        public ApiKeyRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }
    }
}
