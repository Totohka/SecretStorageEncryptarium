using BusinessLogic.Services.NotificationSenders;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace Encryptarium.Storage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        //private readonly BotWebNotification _botWebNotification;
        //public TelegramController(BotWebNotification botWebNotification)
        //{
        //    _botWebNotification = botWebNotification;
        //}

        //[HttpPost]
        //public async Task<OkResult> Post([FromBody] Update update)
        //{
        //    if (update == null) return Ok();

        //    var commands = BotWebNotification.Commands;
        //    var message = update.Message;
        //    var botClient = await _botWebNotification.GetBotClientAsync();

        //    foreach (var command in commands)
        //    {
        //        if (command.Contains(message))
        //        {
        //            await command.Execute(message, botClient);
        //            break;
        //        }
        //    }
        //    return Ok();
        //}
    }
}
