using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Emails.Interface;
using BusinessLogic.Services.TwoFactorsAuth.Interface;
using BusinessLogic.Utils;
using DAL.Repositories.Users.Interface;
using Microsoft.Extensions.Logging;
using Model.Entities;

namespace BusinessLogic.Services.TwoFactorsAuth.Implementation
{
    public class TwoFactorAuthService : BaseService, ITwoFactorAuthService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        public TwoFactorAuthService(IEmailService emailService,
                                    IUserRepository userRepository,
                                    ILogger<TwoFactorAuthService> logger) : base(logger)
        {
            _emailService = emailService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Генерирует 2FA код
        /// </summary>
        /// <returns></returns>
        public ServiceResponse<string> GenerateCode()
        {
            _logger.LogInformation("Вызван метод TwoFactorAuthService.GenerateCode()");

            var random = new Random();
            return Ok(random.Next(100000, 999999).ToString());
        }

        /// <summary>
        /// Отправляет код на почту и активирует его у юзера
        /// </summary>
        /// <param name="user"> Юзер, кому отправить код </param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> SendCodeOnEmail(User user)
        {
            _logger.LogInformation("Вызван метод TwoFactorAuthService.SendCodeOnEmail()");

            ServiceResponse<string> responseCode = GenerateCode();
            string code = responseCode.Data;

            string subject = "Хранилище Encryptarium";
            string body = $"Код для 2FA: {code}";
            await EmailUtil.SendEmailAsync(user.Email, subject, body);

            user.Code2FA = code;
            user.IsActiveCode = true;
            await _userRepository.UpdateAsync(user);

            return Ok(true);
        }

        /// <summary>
        /// Проверяет 2FA код и делает код неактивным
        /// </summary>
        /// <param name="user"> Юзер, которому нужно валидировать код </param>
        /// <param name="code"> Код </param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> VerifyCode(User user, string code)
        {
            _logger.LogInformation("Вызван метод TwoFactorAuthService.VerifyCode()");

            user.IsActiveCode = false;
            await _userRepository.UpdateAsync(user);

            if (user.Code2FA == code)
                return Ok(true);
            return Ok(false);
        }
    }
}
