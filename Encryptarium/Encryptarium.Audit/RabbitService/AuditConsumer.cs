using BusinessLogic.Entities;
using BusinessLogic.Services.Audits.Interface;
using MassTransit;

namespace Encryptarium.Audit.RabbitService
{
    public class AuditConsumer : IConsumer<MonitorMessage>
    {
        private readonly IAuditService _auditService;
        public AuditConsumer(IAuditService auditService)
        {
            _auditService = auditService;
        }
        public async Task Consume(ConsumeContext<MonitorMessage> context)
        {
            var data = context.Message;
            var audit = new Model.Entities.Audit()
            {
                Action = data.Action,
                SourceController = data.SourceController,
                SourceMethod = data.SourceMethod,
                AuthorizePolice = data.AuthorizePolice,
                SourceMicroservice = data.SourceMicroservice,
                StatusCode = data.StatusCode,
                IsSourceRightAdmin = data.IsSourceRightAdmin,
                DateAct = data.DateAct,
                UserUid = Guid.Empty == data.UserUid ? null : data.UserUid,
                Entity = data.Entity,
                EntityUid = data.EntityUid
            };
            await _auditService.CreateAsync(audit);
        }
    }
}
