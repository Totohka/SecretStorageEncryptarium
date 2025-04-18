using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Users.Interface
{
    public interface IUserService
    {
        /// <summary>
        /// Получение пользователя по логину паролю
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<ServiceResponse<User>> GetUserPassAsync(string login, string password);

        /// <summary>
        /// Получение пользователя по email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<ServiceResponse<User>> GetUserByEmailAsync(string email);

        /// <summary>
        /// Получение пользователя по uid
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<User>> GetUserByUidAsync(Guid uid);
        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<List<User>>> GetAllUsersAsync();
        /// <summary>
        /// Получение всех пользователей + фильтрация + пагинация
        /// </summary>
        /// <returns></returns>
        public Task<ServiceResponse<ResponseUserDTOs>> GetAllUsersAsync(Guid myUid, FilterUser filterUser, bool isRemoveMe = true);

        /// <summary>
        /// Создание пользователя по логину паролю
        /// </summary>
        /// <param name="login"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="refreshTokenUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> CreateUserPassAsync(string login, string email, string password, Guid refreshTokenUid);

        /// <summary>
        /// Создание пользователя из OAuth
        /// </summary>
        /// <param name="gitHubUser"></param>
        /// <param name="refreshTokenUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> CreateUserOAuthAsync(GitHubUser gitHubUser, Guid refreshTokenUid);

        public Task<ServiceResponse<bool>> ChangeEmailAsync(Guid uid, string email);

        public Task<ServiceResponse<bool>> ChangeLoginAsync(Guid uid, string login);

        /// <summary>
        /// Присвоение refresh токена к пользователю
        /// </summary>
        /// <param name="userUid"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> UpdateRefreshTokenByUserAsync(Guid userUid, Guid refreshToken);

        /// <summary>
        /// Изменение метода авторизации
        /// </summary>
        /// <param name="userUid"></param>
        /// <param name="changeMethodAuthDTO"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> ChangeAuthMethodAsync(Guid userUid, ChangeMethodAuthDTO changeMethodAuthDTO);

        /// <summary>
        /// Установка кода 2FA
        /// </summary>
        /// <param name="userUid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> SetCode2FAAsync(Guid userUid, string code);

        /// <summary>
        /// Деактивация пользователя
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeactivateUserAsync(Guid userUid);
        
        /// <summary>
        /// Удаление пользователя по uid
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> DeleteUserAsync(Guid userUid);

        /// <summary>
        /// Изменения права на создание хранилища пользователю
        /// </summary>
        /// <param name="userUid"></param>
        /// <param name="isCreate"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> SetIsCreateAsync(Guid userUid, bool isCreate);

        /// <summary>
        /// Дать права администратора системы
        /// </summary>
        /// <param name="userUid"></param>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> SetIsAdminAsync(Guid userUid, bool isAdmin);

        public Task<ServiceResponse<UsersByStorageResponseDTO>> GetUsersByStorageAsync(Guid storageUid);
        public Task<ServiceResponse<bool>> LinkTelegramNotificationAsync(string message, long chatId);
    }
}
