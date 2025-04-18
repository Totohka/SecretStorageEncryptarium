using DAL.Repositories.AdminKeys.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using mtanksl.ShamirSecretSharing;

namespace DAL.Repositories.AdminKeys.Implementation
{
    public class AdminKeyRepository : IAdminKeyRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public AdminKeyRepository(IMemoryCache memoryCache,
                                  IConfiguration configuration) 
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task SetAdminKeyAsync(string key, string value)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(600))
                .SetSlidingExpiration(TimeSpan.FromSeconds(600)) 
                .SetPriority(CacheItemPriority.High)
                );
        }

        public async Task RemoveAdminKeyAsync()
        {
            for (int i = 1; i < 6; i++)
            {
                string id = _configuration.GetSection($"AdminKeyId{i}").Value;
                if (id != "" && _memoryCache.TryGetValue(id, out var value1))
                {
                    _memoryCache.Remove(id);
                    _configuration.GetSection($"AdminKeyId{i}").Value = "";
                }
                else if (id != "" && !_memoryCache.TryGetValue(id, out var value2))
                {
                    _configuration.GetSection($"AdminKeyId{i}").Value = "";
                }
            }
        }

        public async Task<bool> CheckAdminKeysForMasterKeyAsync()
        {
            int totalFalse = 0;
            for (int i = 1; i < 6; i++)
            {
                if (!_memoryCache.TryGetValue<string>(_configuration.GetSection($"AdminKeyId{i}").Value, out string value))
                {
                    totalFalse += 1;
                }
            }
            return totalFalse > 2 ? false : true;  //Если нет больше двух ключей, то восстановление невозможно
        }

        public async Task SetMasterKeyAsync()
        {
            var masterKeyId = _configuration.GetSection("MasterKeyId").Value;
            using var sss = new ShamirSecretSharing();
            Share[] shares = Array.Empty<Share>();
            for (int i = 1; i < 6; i++)
            {
                if (_memoryCache.TryGetValue<string>(_configuration.GetSection($"AdminKeyId{i}").Value, out string value))
                {
                    shares = shares.Append(Share.Parse(value)).ToArray();
                }
            }
                                                                                             
            var msk = sss.Join(shares);
            _memoryCache.Set(masterKeyId, msk, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.MaxValue)
                .SetSlidingExpiration(TimeSpan.MaxValue)
                .SetPriority(CacheItemPriority.High)
                );
            await RemoveAdminKeyAsync();
        }

        public async Task SetMasterKeyAsync(string value)
        {
            var masterKeyId = _configuration.GetSection("MasterKeyId").Value;
            _memoryCache.Set(masterKeyId, value, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.MaxValue)
                .SetSlidingExpiration(TimeSpan.MaxValue) 
                .SetPriority(CacheItemPriority.High)
                );
        }
    }
}
