using DAL.Repositories.RefreshTokens.Interface;
using Model.Entities;
using BusinessLogic.Services.Tokens.Interface;
using BusinessLogic.Services.Base;
using Microsoft.Extensions.Logging;
using BusinessLogic.Entities;

namespace BusinessLogic.Services.Tokens.Implementation
{
    public class RefreshTokenService : BaseService, IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository,
                                   ILogger<RefreshTokenService> logger) : base(logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ServiceResponse<bool>> CheckExpireTokenAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод RefreshTokenService.CheckExpireTokenAsync()");

            RefreshToken refreshToken = await _refreshTokenRepository.GetAsync(uid);
            if (refreshToken.DateExpireToken > DateTime.UtcNow)
            {
                refreshToken.DateDeadToken = refreshToken.DateExpireToken;
                refreshToken.IsActive = false;
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        public async Task<ServiceResponse<bool>> DeactivateRefreshTokenAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод RefreshTokenService.DeactivateRefreshTokenAsync()");

            RefreshToken refreshToken = await _refreshTokenRepository.GetAsync(uid);
            if (refreshToken.DateExpireToken > DateTime.UtcNow)
            {
                refreshToken.DateDeadToken = refreshToken.DateExpireToken;
            }
            else
            {
                refreshToken.DateDeadToken = DateTime.UtcNow;
            }
            refreshToken.IsActive = false;

            await _refreshTokenRepository.UpdateAsync(refreshToken);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteRefreshTokenAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод RefreshTokenService.DeleteRefreshTokenAsync()");

            await _refreshTokenRepository.DeleteAsync(uid);
            return Ok(true);
        }

        public async Task<ServiceResponse<Guid>> GenerateRefreshTokenAsync()
        {
            _logger.LogInformation("Вызван метод RefreshTokenService.GenerateRefreshTokenAsync()");

            var refreshToken = new RefreshToken()
            {
                DateCreate = DateTime.UtcNow,
                DateDeadToken = null,
                DateExpireToken = DateTime.UtcNow.AddDays(30),
                IsActive = true
            };
            return Ok(await _refreshTokenRepository.CreateAsync(refreshToken));
        }

        public async Task<ServiceResponse<List<RefreshToken>>> GetAllAsync()
        {
            _logger.LogInformation("Вызван метод RefreshTokenService.GetAllAsync()");

            return Ok(await _refreshTokenRepository.GetAllAsync());
        }

        public async Task<ServiceResponse<RefreshToken>> GetAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод RefreshTokenService.GetAsync()");

            return Ok(await _refreshTokenRepository.GetAsync(uid));
        }
    }
}
