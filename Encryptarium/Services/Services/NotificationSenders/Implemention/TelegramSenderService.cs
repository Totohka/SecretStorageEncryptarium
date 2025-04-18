using BusinessLogic.Services.NotificationSenders.Entities;
using BusinessLogic.Services.NotificationSenders.Interface;
using BusinessLogic.Services.Users.Interface;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace BusinessLogic.Services.NotificationSenders.Implemention
{
    public class TelegramSenderService : INotificationSenterService
    {
        private readonly string _notificationBot; 
        private readonly string _notificationToken;
        private readonly string _notificationURL; 
        private readonly IUserService _userService;
        private readonly BotWebNotification _botWebNotification;
        private readonly INotificationSenterService? _sender;
        public TelegramSenderService(INotificationSenterService? sender,
                                     IConfiguration configuration,
                                     IUserService userService,
                                     BotWebNotification botWebNotification)
        {
            _botWebNotification = botWebNotification;
            _notificationBot = configuration["Telegram:NotificationBot"];
            _notificationToken = configuration["Telegram:NotificationToken"];
            _notificationURL = configuration["Telegram:NotificationURL"];
            _userService = userService;
            _sender = sender;
        }
        public async Task SendNotification(MessageBase messageBase)
        {
            if (messageBase is TelegramMessage telegramMessage)
            {
                await SendTelegramAsync(telegramMessage.Message, telegramMessage.UserUid);
                if (_sender is not null)
                    await _sender.SendNotification(messageBase);
            }
        }

        private async Task SendTelegramAsync(string message, Guid userUid)
        {
            var user = await _userService.GetUserByUidAsync(userUid);
            if (user?.Data?.ChatId is not null)
            {
                var botClient = await _botWebNotification.GetBotClientAsync();
                await botClient.SendMessage(user.Data.ChatId, message, Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }
}
