namespace BusinessLogic.Services.NotificationSenders.Entities;

public abstract class MessageDecorator : MessageBase
{
    protected MessageDecorator(string msg, MessageBase messageBase) : base(msg)
    {
        MessageBase = messageBase;
    }

    protected MessageBase MessageBase { get; set; }

    public virtual MessageBase GetMessageBase()
    {
        return MessageBase;
    }
}
