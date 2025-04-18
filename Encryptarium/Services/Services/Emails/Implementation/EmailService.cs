using BusinessLogic.Entities;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.Emails.Interface;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BusinessLogic.Services.Emails.Implementation
{
    public class EmailService : BaseService, IEmailService
    {
        public EmailService(ILogger<EmailService> logger) : base(logger) 
        {
        
        }

        public async Task<ServiceResponse<bool>> SendEmailAsync(string email, string subject, string message)
        {
            _logger.LogInformation("Вызван метод EmailService.SendEmailAsync()");

            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Encryptarium", "Encryptarium@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                string error = null;
                try
                {
                    await client.ConnectAsync("smtp.yandex.ru", 465, true);
                    await client.AuthenticateAsync("Encryptarium@yandex.ru", "cnviitklvhqwxgob");
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    error = "Произошла ошибка в работе с почтой";// ex.Message;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }

                if (error is null)
                {
                    return Ok(true);
                }
                _logger.LogError("Метод EmailService.SendEmailAsync(). Произошла ошибка в работе с почтой");
                return Error(error);
            }
        }
    }
}
