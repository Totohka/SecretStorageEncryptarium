using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Tokens.Interface
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Проверяет, истёк ли токен
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> CheckExpireTokenAsync(Guid uid);

        /// <summary>
        /// Получение refresh токена по uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<RefreshToken>> GetAsync(Guid uid);

        /// <summary>
        /// Получение всех refresh токенов
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<List<RefreshToken>>> GetAllAsync();

        /// <summary>
        /// Создание нового refresh токена
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<Guid>> GenerateRefreshTokenAsync();

        /// <summary>
        /// Деактивация токена по uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeactivateRefreshTokenAsync(Guid uid);

        /// <summary>
        /// Удаление refresh токена по uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteRefreshTokenAsync(Guid uid);
    }
}
