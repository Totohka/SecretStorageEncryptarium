namespace BusinessLogic.Services.NotificationSenders.Entities;

public class EmailMessage : MessageDecorator
{
    public EmailMessage(string email, string subject, MessageBase messageBase) : base(messageBase.Message, messageBase)
    { 
        Email = email;
        Subject = subject;
    }
    public string Email { get; set; }
    public string Subject { get; set; }
}
