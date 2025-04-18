using DAL.Repositories.Base.Implementation;
using DAL.Repositories.RefreshTokens.Interface;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace DAL.Repositories.RefreshTokens.Implementation
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IDbContextFactory<SecretContext> dbContextFactory) : base(dbContextFactory)
        {
        }

        public override async Task<Guid> CreateAsync(RefreshToken refreshToken)
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            await db.RefreshTokens.AddAsync(refreshToken);
            await db.SaveChangesAsync();
            return refreshToken.Uid;
        }
    }
}
