using Telegram.Bot.Types;
using Telegram.Bot;

namespace BusinessLogic.Services.NotificationSenders.Commands;

public abstract class Command
{
    public abstract string Name { get; }

    public abstract Task Execute(Message message, TelegramBotClient client);

    public abstract bool Contains(Message message);
}
