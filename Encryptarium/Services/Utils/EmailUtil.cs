using MailKit.Net.Smtp;
using MimeKit;

namespace BusinessLogic.Utils
{
    public static class EmailUtil
    {
        public static async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
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
                    return true;
                }

                return false;
            }
        }
    }
}
