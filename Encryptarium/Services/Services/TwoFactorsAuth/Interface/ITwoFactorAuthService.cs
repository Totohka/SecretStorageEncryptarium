using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.TwoFactorsAuth.Interface
{
    public interface ITwoFactorAuthService
    {
        /// <summary>
        /// Генерирует код для 2FA
        /// </summary>
        /// <returns></returns>
        public ServiceResponse<string> GenerateCode();

        /// <summary>
        /// Отправляет код на почту и активирует его у юзера
        /// </summary>
        /// <param name="user"> Юзер, кому отправить код</param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> SendCodeOnEmail(User user);

        /// <summary>
        /// Проверяет 2FA код и делает код неактивным
        /// </summary>
        /// <param name="user"> Юзер, которому нужно валидировать код</param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> VerifyCode(User user, string code);
    }
}
