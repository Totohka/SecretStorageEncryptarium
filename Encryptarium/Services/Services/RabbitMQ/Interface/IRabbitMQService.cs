using BusinessLogic.Entities;

namespace BusinessLogic.Services.RabbitMQ.Interface
{
    public interface IRabbitMQService
    {
        public Task SendMessage(MonitorMessage obj);
        public Task SendMessage(string message);
    }
}
