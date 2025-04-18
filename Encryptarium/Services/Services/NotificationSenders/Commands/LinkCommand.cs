using Telegram.Bot.Types;
using Telegram.Bot;
using BusinessLogic.Services.Users.Interface;

namespace BusinessLogic.Services.NotificationSenders.Commands
{
    public class LinkCommand : Command
    {
        private readonly IUserService _userService;
        public override string Name => @"/link";
        public LinkCommand(IUserService userService) 
        { 
            _userService = userService;
        }
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            await botClient.DeleteMessage(chatId, message.Id);
            var result = await _userService.LinkTelegramNotificationAsync(message.Text, chatId);
            if (result.IsSuccess)
            {
                await botClient.SendMessage(chatId, "Пользователь привязан к уведомлениям в телеграмме.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            else
            {
                await botClient.SendMessage(chatId, "Произошла ошибка.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }
}
