using BusinessLogic.Services.NotificationSenders.Entities;

namespace BusinessLogic.Services.NotificationSenders.Interface
{
    public interface INotificationSenterService
    {
        //private readonly INotificationSenterService? _sender { get; set; }
        public Task SendNotification(MessageBase messageBase);
    }
}
