using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Ips.Interface
{
    public interface IIpService
    {
        /// <summary>
        /// Получение всех сущностей по IP адресу
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Task<ServiceResponse<List<WhiteListIp>>> GetWhiteIpsByIpAsync(string ip);

        /// <summary>
        /// Получение всех сущностей
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<List<WhiteListIp>>> GetAllIpListAsync();

        public Task<ServiceResponse<List<WhiteListIp>>> GetIpByApiKeyUidAsync(Guid apiKeyUid);

        /// <summary>
        /// Получение сущности IP
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<WhiteListIp>> GetIpListAsync(Guid uid);

        /// <summary>
        /// Валидирует IP-адрес
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="apiKeyUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> VerifyIp(string ip, Guid apiKeyUid);

        /// <summary>
        /// Создание сущности
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="apiKeyUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> CreateIpAsync(string ip, Guid apiKeyUid);

        /// <summary>
        /// Редактирование IP адреса
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> UpdateIpAsync(Guid uid, string ip);

        /// <summary>
        /// Изменение ApiKeyUid
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="apiKeyUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> UpdateApiKeyUidAsync(Guid uid, Guid apiKeyUid);

        /// <summary>
        /// Деактивация сущности
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeactivateIpByUidAsync(Guid uid);

        /// <summary>
        /// Удаление всех сущностей связанных с IP адресом
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteIpAsync(string ip);

        /// <summary>
        /// Удаление сущности по uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteIpByUidAsync(Guid uid);
    }
}
