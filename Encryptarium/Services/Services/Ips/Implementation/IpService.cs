using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Ips.Interface;
using DAL.Repositories.WhiteListIps.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;

namespace BusinessLogic.Services.Ips.Implementation
{
    public class IpService : BaseService, IIpService
    {
        private readonly IWhiteListIpRepository _whiteListIpRepository;
        public IpService(IWhiteListIpRepository whiteListIpRepository,
                         ILogger<IpService> logger) : base(logger)
        {
            _whiteListIpRepository = whiteListIpRepository;
        }
        public async Task<ServiceResponse<bool>> CreateIpAsync(string ip, Guid apiKeyUid)
        {
            _logger.LogInformation("Вызван метод IpService.CreateIpAsync()");

            var ipListIp = new WhiteListIp()
            {
                Ip = ip,
                ApiKeyUid = apiKeyUid,
                IsActive = true
            };
            await _whiteListIpRepository.CreateAsync(ipListIp);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeactivateIpByUidAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод IpService.DeactivateIpByUidAsync()");

            WhiteListIp ipListIp = await _whiteListIpRepository.GetAsync(uid);
            ipListIp.IsActive = !ipListIp.IsActive;
            await _whiteListIpRepository.UpdateAsync(ipListIp);
            return Ok(true);
        }
        
        public async Task<ServiceResponse<bool>> DeleteIpAsync(string ip)
        {
            _logger.LogInformation("Вызван метод IpService.DeleteIpAsync()");

            await _whiteListIpRepository.DeleteAsync(wli => wli.Ip == ip);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> DeleteIpByUidAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод IpService.DeleteIpByUidAsync()");

            await _whiteListIpRepository.DeleteAsync(uid);
            return Ok(true);
        }

        public async Task<ServiceResponse<List<WhiteListIp>>> GetIpByApiKeyUidAsync(Guid apiKeyUid)
        {
            _logger.LogInformation("Вызван метод IpService.GetAllIpListAsync()");

            return Ok(await _whiteListIpRepository.GetAllAsync(i => i.ApiKeyUid == apiKeyUid));
        }

        public async Task<ServiceResponse<List<WhiteListIp>>> GetAllIpListAsync()
        {
            _logger.LogInformation("Вызван метод IpService.GetAllIpListAsync()");

            return Ok(await _whiteListIpRepository.GetAllAsync());
        }

        public async Task<ServiceResponse<WhiteListIp>> GetIpListAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод IpService.GetIpListAsync()");

            return Ok(await _whiteListIpRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<List<WhiteListIp>>> GetWhiteIpsByIpAsync(string ip)
        {
            _logger.LogInformation("Вызван метод IpService.GetWhiteIpsByIpAsync()");

            return Ok(await _whiteListIpRepository.GetAllAsync(wli => wli.Ip == ip));
        }

        public async Task<ServiceResponse<bool>> UpdateApiKeyUidAsync(Guid uid, Guid apiKeyUid)
        {
            _logger.LogInformation("Вызван метод IpService.UpdateApiKeyUidAsync()");

            WhiteListIp ipListIp = await _whiteListIpRepository.GetAsync(uid);
            ipListIp.ApiKeyUid = apiKeyUid;
            await _whiteListIpRepository.UpdateAsync(ipListIp);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> UpdateIpAsync(Guid uid, string ip)
        {
            _logger.LogInformation("Вызван метод IpService.UpdateIpAsync()");

            WhiteListIp ipListIp = await _whiteListIpRepository.GetAsync(uid);
            ipListIp.Ip = ip;
            await _whiteListIpRepository.UpdateAsync(ipListIp);
            return Ok(true);
        }

        public async Task<ServiceResponse<bool>> VerifyIp(string ip, Guid apiKeyUid)
        {
            _logger.LogInformation("Вызван метод IpService.VerifyIp()");

            List<WhiteListIp>? whiteListIps = await _whiteListIpRepository.GetAllAsync(wli => wli.Ip == ip);

            if (whiteListIps is null)
                return Ok(false);

            foreach (var whiteListIp in whiteListIps)
            {
                if (whiteListIp.ApiKeyUid == apiKeyUid)
                    return Ok(true);
            }
            return Ok(false);
        }
    }
}
