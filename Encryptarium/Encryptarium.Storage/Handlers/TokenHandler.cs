using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DAL;
using Encryptarium.Storage.Requirements;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Model;
using Microsoft.EntityFrameworkCore;
using BusinessLogic.Entities;
using System.Text.Json;

namespace Encryptarium.Storage.Handlers
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
                    case SecretRequirement:
                        if (await ValidateSecretPolicy(accessToken, new Guid(refreshToken), httpContext))
                        {
                            
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return;
                        }
                        break;
                    case StorageRequirement:
                        if (await ValidateStoragePolicy(accessToken, new Guid(refreshToken), httpContext))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();
                            return;
                        }
                        break;
                    case UserRequirement:
                        if (await ValidateUserPolicy(accessToken, new Guid(refreshToken)))
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

        private async Task<bool> ValidateUserPolicy(string accessToken, Guid refreshToken)
        {
            User? user = await GetUserByToken(accessToken, refreshToken);
            if (user is null)
                return false;

            if (user.IsCreateStorage)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> ValidateSecretPolicy(string accessToken, Guid refreshToken, HttpContext httpContext)
        {
            User? user = await GetUserByToken(accessToken, refreshToken);
            if (user is null)
                return false;

            Guid secretUid = Guid.Parse(httpContext.Request.RouteValues["secretUid"].ToString());

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SecretContext>();
                var roles = dbContext.UserRoles.Where(ur => ur.UserUid == user.Uid).ToList()
                                        .Select(ur => dbContext.Roles.Find(ur.RoleUid));
                var secretLinkPolicies = dbContext.SecretLinkPolicies.Where(slp => slp.SecretUid == secretUid).ToList();
                var secretLinkRolePolicies = new List<SecretLinkPolicy>();
                foreach (var secretLinkPolicy in secretLinkPolicies) {
                    if (roles.Select(r => r.Uid).Contains(secretLinkPolicy.RoleUid))
                    {
                        secretLinkRolePolicies.Add(secretLinkPolicy);
                    }
                }
                if (secretLinkRolePolicies.Count == 0) //Если политики на секрет нет, то проверяем политику на хранилище
                {
                    Guid storageUid = dbContext.Secrets.First(s => s.Uid == secretUid).StorageUid;
                    
                    var storagePolicies = dbContext.UserRoles.Where(ur => ur.UserUid == user.Uid).ToList()
                                            .Select(ur => dbContext.Roles.Find(ur.RoleUid))
                                            .Select(r => dbContext.StorageLinkPolicies.First(slp => slp.RoleUid == r.Uid && slp.StorageUid == storageUid))
                                            .Select(slp => dbContext.StoragePolicies.Find(slp.StoragePolicyUid));
                    bool isTrue = false;
                    switch (httpContext.Request.Method)
                    {
                        case "PUT":
                            foreach (var policy in storagePolicies)
                            {
                                if (policy.IsUpdate)
                                {
                                    isTrue = true;
                                    break;
                                }
                            }
                            break;
                        case "DELETE":
                            foreach (var policy in storagePolicies)
                            {
                                if (policy.IsDelete)
                                {
                                    isTrue = true;
                                    break;
                                }
                            }
                            break;
                        case "GET":
                            foreach (var policy in storagePolicies)
                            {
                                if (policy.IsRead)
                                {
                                    isTrue = true;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    return isTrue;
                }
                else //Если политики на секрет есть, то проверяем их
                {
                    var secretPolicies = secretLinkPolicies.Select(slp => dbContext.SecretPolicies.Find(slp.SecretPolicyUid));

                    bool isTrue = false;
                    switch (httpContext.Request.Method)
                    {
                        case "PUT":
                            foreach (var policy in secretPolicies)
                            {
                                if (policy.IsUpdate)
                                {
                                    isTrue = true;
                                    break;
                                }
                            }
                            break;
                        case "DELETE":
                            foreach (var policy in secretPolicies)
                            {
                                if (policy.IsDelete)
                                {
                                    isTrue = true;
                                    break;
                                }
                            }
                            break;
                        case "GET":
                            foreach (var policy in secretPolicies)
                            {
                                if (policy.IsRead)
                                {
                                    isTrue = true;
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    return isTrue;
                }
                
            }
        }

        private async Task<bool> ValidateStoragePolicy(string accessToken, Guid refreshToken, HttpContext httpContext)
        {
            User? user = await GetUserByToken(accessToken, refreshToken);
            if (user is null)
                return false;

            
            Guid storageUid = Guid.Empty;
            if (httpContext.Request.Method == "POST")
            {
                using (StreamReader stream = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    httpContext.Request.EnableBuffering();
                    string body = await stream.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    SecretDTO secretDTO = JsonSerializer.Deserialize<SecretDTO>(body, options);
                    storageUid = (Guid)secretDTO.StorageUid;
                }
            }
            else if (httpContext.Request.Method == "GET" ||
                     httpContext.Request.Method == "PUT" ||
                     httpContext.Request.Method == "DELETE")
            {
                storageUid = Guid.Parse(httpContext.Request.RouteValues["storageUid"].ToString());
            }
            

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SecretContext>();

                var storagePolicies = dbContext.UserRoles.Where(ur => ur.UserUid == user.Uid).ToList()
                                        .Select(ur => dbContext.Roles.Find(ur.RoleUid))
                                        .Select(r => dbContext.StorageLinkPolicies.First(slp => slp.RoleUid == r.Uid && slp.StorageUid == storageUid))
                                        .Select(slp => dbContext.StoragePolicies.Find(slp.StoragePolicyUid));
                bool isTrue = false;
                switch (httpContext.Request.Method)
                {
                    case "PUT":
                        foreach (var policy in storagePolicies)
                        {
                            if (policy.IsUpdate)
                            {
                                isTrue = true;
                                break;
                            }
                        }
                        break;
                    case "DELETE":
                        foreach (var policy in storagePolicies)
                        {
                            if (policy.IsDelete)
                            {
                                isTrue = true;
                                break;
                            }
                        }
                        break;
                    case "GET":
                        foreach (var policy in storagePolicies)
                        {
                            if (policy.IsRead)
                            {
                                isTrue = true;
                                break;
                            }
                        }
                        break;
                    case "POST":
                        foreach (var policy in storagePolicies)
                        {
                            if (policy.IsCreate)
                            {
                                isTrue = true;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
                return isTrue;
            }
        }

        private async Task<User?> GetUserByToken(string accessToken, Guid refreshToken)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
            
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
