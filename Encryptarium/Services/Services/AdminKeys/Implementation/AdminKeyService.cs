using BusinessLogic.Services.AdminKeys.Interface;
using DAL.Repositories.AdminKeys.Interface;
using Microsoft.Extensions.Configuration;
using Model;
using mtanksl.ShamirSecretSharing;
using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Services.AdminKeys.Implementation
{
    public class AdminKeyService : IAdminKeyService
    {
        private readonly IAdminKeyRepository _adminKeyRepository;
        private readonly IConfiguration _configuration;
        private readonly string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()-_=+[]{}|;:,.<>?/~";
        public AdminKeyService(IAdminKeyRepository adminKeyRepository,
                               IConfiguration configuration) 
        {
            _adminKeyRepository = adminKeyRepository;
            _configuration = configuration;
        }

        public async Task RemoveAdminKeyAsync()
        {
            await _adminKeyRepository.RemoveAdminKeyAsync();
        }

        public async Task<bool> SetAdminKeyAsync(string value)
        {
            using var rng = RandomNumberGenerator.Create();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 100; i++)
            {
                sb.Append(RandomNumberGenerator.GetString(chars, 10));
            }
            string keyIndex = sb.ToString();
            for (int i = 1; i < 6; i++)
            {
                string id = _configuration.GetSection($"AdminKeyId{i}").Value;
                if (id == "")
                {
                    _configuration.GetSection($"AdminKeyId{i}").Value = keyIndex;
                    await _adminKeyRepository.SetAdminKeyAsync(keyIndex, value);
                    break;
                }
            }

            if (await _adminKeyRepository.CheckAdminKeysForMasterKeyAsync())
            {
                await _adminKeyRepository.SetMasterKeyAsync(); //восстанавливаем ключ, если это возможно
                return true;
            }
            return false;
        } 

        public async Task<List<string>> CreateAdminKeysAsync()
        {
            using var sss = new ShamirSecretSharing();
            using var rng = RandomNumberGenerator.Create();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; i++) 
            {
                sb.Append(RandomNumberGenerator.GetString(chars, 10));
            }
            string masterKey = sb.ToString();
            sb.Remove(0, sb.Length);
            for (int i = 0; i < 32; i++) //256 бит
            {
                sb.Append(Encoding.UTF8.GetString(RandomNumberGenerator.GetBytes(1)));
            }
            _configuration.GetSection(Constants.MasterKeyId).Value = sb.ToString();
            var adminKeys = sss.Split(3, 5, masterKey);
            await _adminKeyRepository.SetMasterKeyAsync(masterKey);
            return adminKeys.Select(x => x.ToString()).ToList();
        }
    }
}
