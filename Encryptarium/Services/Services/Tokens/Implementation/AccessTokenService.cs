using Microsoft.IdentityModel.Tokens;
using BusinessLogic.Services.Tokens.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DAL.Repositories.Users.Interface;
using Model.Entities;
using Model;
using BusinessLogic.Services.Base;
using Microsoft.Extensions.Logging;
using BusinessLogic.Entities;

namespace BusinessLogic.Services.Tokens.Implementation
{
    public class AccessTokenService : BaseService, IAccessTokenService
    {
        private readonly IUserRepository _userRepository;
        public AccessTokenService(IUserRepository userRepository,
                                  ILogger<AccessTokenService> logger) : base(logger)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<string>> GetAccessTokenAsync(Guid? refreshTokenUid, Guid userUid)
        {
            _logger.LogInformation("Вызван метод AccessTokenService.GetAccessTokenAsync()");

            if (refreshTokenUid is not null)
            {
                User user = await _userRepository.GetAsync(userUid);
                
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshTokenUid.ToString()));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "Encryptarium",
                    audience: "Encryptarium",
                    claims: new[] { 
                        new Claim(Constants.ClaimUserUid, userUid.ToString()), 
                        new Claim(Constants.ClaimUserRole, user.IsAdmin ? "Admin" : "User" ) 
                    },
                    expires: DateTime.UtcNow.AddMinutes(1),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(tokenString);
            }
            _logger.LogError("Метод AccessTokenService.GetAccessTokenAsync(). Некорректный refresh токен");
            return Error<string>("Некорректный refresh токен");
        }

        public ServiceResponse<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token, Guid refreshToken)
        {
            _logger.LogInformation("Вызван метод AccessTokenService.GetPrincipalFromExpiredToken()");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshToken.ToString())),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogError("Метод AccessTokenService.GetPrincipalFromExpiredToken(). Некорректный токен");
                return Error<ClaimsPrincipal>("Некорректный токен");
            }
            return Ok(principal);
        }
    }
}
