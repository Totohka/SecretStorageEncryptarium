using BusinessLogic.Entities;
using BusinessLogic.Services.ApiKeys.Interface;
using BusinessLogic.Services.Base;
using DAL.Repositories.ApiKeys.Interface;
using DAL.Repositories.Users.Interface;
using Microsoft.Extensions.Logging;
using Model;
using Model.Entities;
using System.Security.Cryptography;

namespace BusinessLogic.Services.ApiKeys.Implemention
{
    public class ApiKeyService : BaseService, IApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IUserRepository _userRepository;
        public ApiKeyService(IApiKeyRepository apiKeyRepository,
                             ILogger<ApiKeyService> logger,
                             IUserRepository userRepository) : base(logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<bool>> DeactivedApiKey(List<Guid>? uidUsers, bool? isAll)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.DeactivedApiKey()");

            if (isAll is not null && (bool)isAll)
            {
                List<ApiKey> apiKeys = await _apiKeyRepository.GetAllAsync(ak => ak.IsActive);
                foreach (var apikey in apiKeys)
                {
                    apikey.IsActive = false;
                    await _apiKeyRepository.UpdateAsync(apikey);
                }
                return Ok(true);
            }
            else
            {
                if (uidUsers is null)
                {
                    _logger.LogError("Метод ApiKeyService.DeactivedApiKey(). Были переданы неверные параметры");
                    return Error("Были переданы неверные параметры");
                }

                foreach (var uidUser in uidUsers)
                {
                    ApiKey apiKey = await _apiKeyRepository.GetAsync(ak => ak.UserUid == uidUser && ak.IsActive);
                    apiKey.IsActive = false;
                    await _apiKeyRepository.UpdateAsync(apiKey);
                }
                return Ok(true);
            }
        }

        public async Task<ServiceResponse<List<ApiKey>>> GetApiKeysByUserUidAsync(Guid userUid)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.GetApiKeysByUserUidAsync()");

            var apikeys = await _apiKeyRepository.GetAllAsync(a => a.UserUid == userUid);
            return Ok(apikeys);
        }

        private string GenerateApiKey()
        {
            _logger.LogInformation("Вызван метод ApiKeyService.GenerateApiKey()");

            var bytes = RandomNumberGenerator.GetBytes(Constants.NumberOfSecureBytesToGenerate);

            string base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");

            var keyLength = Constants.LengthOfKey - Constants.PrefixApiKey.Length;

            return Constants.PrefixApiKey + base64String[..keyLength];
        }

        public async Task<ServiceResponse<string>> CreateApiKey(Guid userUid, string name)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.CreateApiKey()");

            ApiKey? apiKey = await _apiKeyRepository.GetAsync(ak => ak.UserUid == userUid && ak.IsActive);
            if (apiKey is null)
            {
                string key = GenerateApiKey();
                apiKey = new ApiKey() {
                    UserUid = userUid,
                    KeyHash = key,
                    Name = name,
                    IsActive = true
                };
                await _apiKeyRepository.CreateAsync(apiKey);
                return Ok(key);
            }
            _logger.LogWarning("Метод ApiKeyService.CreateApiKey(). Ключ уже существует");
            return Error<string>("Ключ уже существует");
        }

        public async Task<ServiceResponse<User>> VerifyApiKey(string key)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.VerifyApiKey()");

            //Хешируем apiKey с помощью хелпера(пока нет)

            ApiKey apiKey = await _apiKeyRepository.GetAsync(ak => ak.KeyHash == key);
            if (!apiKey.IsActive)
            {
                _logger.LogWarning("Метод ApiKeyService.VerifyApiKey(). Ключ неактивен");
                return Error<User>("Ключ неактивен");
            }

            User user = await _userRepository.GetAsync(apiKey.UserUid);
            if (!user.IsActive)
            {
                _logger.LogWarning("Метод ApiKeyService.VerifyApiKey(). Пользователь неактивен");
                return Error<User>("Пользователь неактивен");
            }
            if (!user.IsApiKey)
            {
                _logger.LogWarning("Метод ApiKeyService.VerifyApiKey(). Пользователю не разрешена авторизация по API-ключу");
                return Error<User>("Пользователю не разрешена авторизация по API-ключу");
            }

            return Ok(user);
        }


        public async Task<ServiceResponse<ApiKey>> GetApiKeyByUserUidAsync(Guid userUid)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.GetApiKeyByUserUidAsync()");

            return Ok(await _apiKeyRepository.GetAsync(ak => ak.UserUid == userUid && ak.IsActive));
        }

        public async Task<ServiceResponse<ApiKey>> GetApiKeyAsync(string apiKey)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.GetApiKeyAsync()");

            return Ok(await _apiKeyRepository.GetAsync(ak => ak.KeyHash == apiKey));
        }

        public async Task<ServiceResponse<ApiKey>> GetApiKeyAsync(Guid uid)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.GetApiKeyAsync()");

            return Ok(await _apiKeyRepository.GetAsync(uid));
        }

        public async Task<ServiceResponse<List<ApiKey>>> GetAllApiKeyAsync()
        {
            _logger.LogInformation("Вызван метод ApiKeyService.GetAllApiKeyAsync()");

            return Ok(await _apiKeyRepository.GetAllAsync());
        }

        public async Task<ServiceResponse<bool>> DeactivedApiKey(Guid apiKeyUid, bool? isAll)
        {
            _logger.LogInformation("Вызван метод ApiKeyService.DeactivedApiKey()");

            if (isAll is not null && (bool)isAll)
            {
                List<ApiKey> apiKeys = await _apiKeyRepository.GetAllAsync(ak => ak.IsActive);
                foreach (var apikey in apiKeys)
                {
                    apikey.IsActive = false;
                    await _apiKeyRepository.UpdateAsync(apikey);
                }
                return Ok(true);
            }
            else
            {
                var apiKey = await _apiKeyRepository.GetAsync(ak => ak.Uid == apiKeyUid);
                apiKey.IsActive = !apiKey.IsActive;
                await _apiKeyRepository.UpdateAsync(apiKey);
                return Ok(true);
            }
        }
    }
}
