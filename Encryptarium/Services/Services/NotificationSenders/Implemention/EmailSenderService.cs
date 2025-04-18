using BusinessLogic.Services.NotificationSenders.Entities;
using BusinessLogic.Services.NotificationSenders.Interface;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace BusinessLogic.Services.NotificationSenders.Implemention
{
    public class EmailSenderService : INotificationSenterService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _useSsl;
        private readonly string _name;
        private readonly string _address;
        private readonly string _password;
        private readonly INotificationSenterService? _sender;
        public EmailSenderService(INotificationSenterService? sender, IConfiguration configuration)
        {
            _sender = sender;
            _name = configuration["Email:Name"];
            _host = configuration["Email:Host"];
            _port = int.Parse(configuration["Email:Port"]);
            _useSsl = bool.Parse(configuration["Email:UseSsl"]);
            _address = configuration["Email:Address"];
            _password = configuration["Email:Password"];
        }
        public async Task SendNotification(MessageBase messageBase)
        {
            //var emailMessage = messageBase as EmailMessage;
            if (messageBase is EmailMessage emailMessage)
            {
                await SendEmailAsync(emailMessage.Email, emailMessage.Subject, emailMessage.Message);
                if (_sender is not null)
                    await _sender.SendNotification(emailMessage.GetMessageBase());
            }
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_name, _address));
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
                    await client.ConnectAsync(_host, _port, _useSsl);
                    await client.AuthenticateAsync(_address, _password);
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
                    return true;
                }

                return false;
            }
        }
    }
}
