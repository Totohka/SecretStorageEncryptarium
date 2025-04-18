namespace BusinessLogic.Services.NotificationSenders.Entities;

public class TelegramMessage : MessageDecorator
{
    public TelegramMessage(Guid userUid, MessageBase messageBase) : base(messageBase.Message, messageBase)
    {
        UserUid = userUid;
    }
    public Guid UserUid { get; set; }
}
