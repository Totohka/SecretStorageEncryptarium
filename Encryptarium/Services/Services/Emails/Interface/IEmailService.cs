using BusinessLogic.Entities;

namespace BusinessLogic.Services.Emails.Interface
{
    public interface IEmailService
    {
        /// <summary>
        /// Отправка писем
        /// </summary>
        /// <param name="email">Email куда нужно отправить сообщение</param>
        /// <param name="subject">Заголовок письма</param>
        /// <param name="message">Письмо</param>
        /// <returns></returns>
        public Task<ServiceResponse<bool>> SendEmailAsync(string email, string subject, string message);
    }
}
