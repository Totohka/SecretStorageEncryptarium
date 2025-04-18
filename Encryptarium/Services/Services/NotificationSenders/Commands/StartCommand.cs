using Telegram.Bot.Types;
using Telegram.Bot;

namespace BusinessLogic.Services.NotificationSenders.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(Name);
        }

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            await botClient.SendMessage(chatId, "Я бот для уведомлений Encryptarium.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}
