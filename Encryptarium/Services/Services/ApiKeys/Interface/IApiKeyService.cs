using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.ApiKeys.Interface
{
    /// <summary>
    /// Сервис занимающийся API-ключами
    /// </summary>
    public interface IApiKeyService
    {
        /// <summary>
        /// Получение ключа по uid пользователя
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<ApiKey>> GetApiKeyByUserUidAsync(Guid userUid);

        /// <summary>
        /// Получение ключа по значению
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public Task<ServiceResponse<ApiKey>> GetApiKeyAsync(string apiKey);

        /// <summary>
        /// Получение API ключей по userUid 
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<List<ApiKey>>> GetApiKeysByUserUidAsync(Guid userUid);

        /// <summary>
        /// Получение ключа по uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<ApiKey>> GetApiKeyAsync(Guid uid);

        /// <summary>
        /// Получение ключей
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<List<ApiKey>>> GetAllApiKeyAsync();

        /// <summary>
        /// Генерация ключа и его сохранение в базе
        /// </summary>
        /// <param name="userUid">Uid пользователя которому создаём ключ</param>
        /// <param name="name">Название ключа</param>
        /// <returns></returns>
        public Task<ServiceResponse<string>> CreateApiKey(Guid userUid, string name);

        /// <summary>
        /// Валидация ключа и выдача пользователя
        /// </summary>
        /// <param name="key">Хешированный ключ на клиенте</param>
        /// <returns></returns>
        public Task<ServiceResponse<User>> VerifyApiKey(string key);

        /// <summary>
        /// Деактивация API-ключа
        /// </summary>
        /// <param name="uidUsers"></param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeactivedApiKey(List<Guid>? uidUsers, bool? isAll);

        /// <summary>
        /// Деактивация API-ключа
        /// </summary>
        /// <param name="uidUsers"></param>
        /// <param name="isAll"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeactivedApiKey(Guid apiKeyUid, bool? isAll);
    }
}
