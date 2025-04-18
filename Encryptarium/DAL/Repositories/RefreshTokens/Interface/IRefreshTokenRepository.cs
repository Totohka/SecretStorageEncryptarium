using DAL.Repositories.Base.Interface;
using Model.Entities;

namespace DAL.Repositories.RefreshTokens.Interface
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        public new Task<Guid> CreateAsync(RefreshToken refreshToken);
    }
}
