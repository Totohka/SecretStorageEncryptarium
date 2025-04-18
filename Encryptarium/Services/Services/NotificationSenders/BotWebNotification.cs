using BusinessLogic.Services.NotificationSenders.Commands;
using BusinessLogic.Services.Users.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Telegram.Bot;

namespace BusinessLogic.Services.NotificationSenders
{
    public class BotWebNotification
    {
        public static IReadOnlyList<Command> Commands;
        private static TelegramBotClient botClient;
        private List<Command> commandsList = new List<Command>();
        private readonly IUserService _userService;
        private readonly string _token;
        private readonly string _url;
        private readonly string _name;
        public BotWebNotification(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _name = configuration["Telegram:NotificationBot"];
            _url = configuration["Telegram:NotificationURL"];
            _token = configuration["Telegram:NotificationToken"];
            commandsList = [new StartCommand(), new LinkCommand(_userService)];
            Commands = commandsList.AsReadOnly();
            if (botClient is null)
            {
                botClient = new TelegramBotClient(_token);
                string hook = string.Format(_url, "api/v1/telegram");
                Task.Run(() => botClient.SetWebhook(hook)).Wait();
            }
        }

        public async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }

            botClient = new TelegramBotClient(_token);
            string hook = string.Format(_url, "api/v1/telegram");
            await botClient.SetWebhook(hook);
            return botClient;
        }
    }
}
