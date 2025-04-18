using DAL;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Encryptarium.Audit.Requirements;
using Model;

namespace Encryptarium.Audit.Handlers
{
    public class TokenHandler : IAuthorizationHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenHandler(IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            List<IAuthorizationRequirement> pendingRequirements = context.PendingRequirements.ToList();
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            string? accessToken = httpContext.Request.Cookies[Constants.AccessToken];
            string? refreshToken = httpContext.Request.Cookies[Constants.RefreshToken];

            if (accessToken is null || refreshToken is null)
            {
                context.Fail();
                return;
            }

            foreach (var requirement in pendingRequirements)
            {
                switch (requirement)
                {
                    case TokenRequirement:
                        if (await ValidateToken(accessToken, new Guid(refreshToken)))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return;
                        }
                        break;
                    case RolesAuthorizationRequirement:
                        var requirementRole = requirement as RolesAuthorizationRequirement;
                        if (await ValidateRole(requirementRole.AllowedRoles, accessToken, new Guid(refreshToken)))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task<User> GetUserByToken(string accessToken, Guid refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(refreshToken.ToString());
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            string userUid = jwtToken.Claims.First(x => x.Type == "Uid").Value;

            User? user = null;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SecretContext>();
                user = await dbContext.Users.FindAsync(new Guid(userUid));
            }
            return user;
        }

        public async Task<bool> ValidateRole(IEnumerable<string> roles, string accessToken, Guid refreshToken)
        {
            string role = roles.ElementAt(0);
            try
            {
                User? user = await GetUserByToken(accessToken, refreshToken);

                if (user is null)
                    return false;

                if (user.IsAdmin)
                    return true;

                if (!user.IsAdmin && role == "User")
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> ValidateToken(string accessToken, Guid refreshToken)
        {
            try
            {
                User? user = await GetUserByToken(accessToken, refreshToken);
                RefreshToken? refreshTokenEntity = null;

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SecretContext>();
                    refreshTokenEntity = await dbContext.RefreshTokens.FindAsync(refreshToken);
                }

                if (user is null)
                    return false;

                if (user.RefreshTokenUid != refreshToken)
                    return false;

                if (refreshTokenEntity.DateExpireToken <= DateTime.UtcNow)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
