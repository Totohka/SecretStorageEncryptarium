using BusinessLogic.Entities;
using System.Security.Claims;

namespace BusinessLogic.Services.Tokens.Interface
{
    public interface IAccessTokenService
    {
        public Task<ServiceResponse<string>> GetAccessTokenAsync(Guid? refreshTokenUid, Guid userUid);
        public ServiceResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, Guid refreshToken);
    }
}
