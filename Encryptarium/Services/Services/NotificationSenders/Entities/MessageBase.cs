namespace BusinessLogic.Services.NotificationSenders.Entities;

public abstract class MessageBase
{
    public MessageBase(string msg) 
    { 
        Message = msg;
    }
    public virtual string Message { get; protected set; }
}
