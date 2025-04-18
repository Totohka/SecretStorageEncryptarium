using BusinessLogic.Services.Audits.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Encryptarium.Audit.RabbitService
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public RabbitMqListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            using var scope = _scopeFactory.CreateScope();

            var factory = new ConnectionFactory { HostName = "localhost", AutomaticRecoveryEnabled = true };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(
                queue: "MyQueue", 
                durable: false, 
                exclusive: false, 
                autoDelete: false, 
                arguments: null
            );
            
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Model.Entities.Audit audit = JsonSerializer.Deserialize<Model.Entities.Audit>(message);
                var auditService = scope.ServiceProvider.GetService<IAuditService>();
                await auditService.CreateAsync(audit);
                await channel.BasicAckAsync(ea.DeliveryTag, true);
            };

            await channel.BasicConsumeAsync("MyQueue", autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
