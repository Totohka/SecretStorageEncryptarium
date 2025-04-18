using BusinessLogic.Entities;
using BusinessLogic.Services.RabbitMQ.Interface;
using MassTransit;

namespace BusinessLogic.Services.RabbitMQ.Implementation
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IBus _bus; 
        public RabbitMQService(IBus bus)
        {
            _bus = bus;
        }
        public async Task SendMessage(MonitorMessage monitorMessage)
        {
            Uri uri = new Uri("rabbitmq://localhost/ENCQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            await endPoint.Send(monitorMessage);
        }

        public async Task SendMessage(string message)
        {
            Uri uri = new Uri("rabbitmq://localhost/ENCQueue");
            var endPoint = await _bus.GetSendEndpoint(uri);
            await endPoint.Send(message);
        }
    }
}
