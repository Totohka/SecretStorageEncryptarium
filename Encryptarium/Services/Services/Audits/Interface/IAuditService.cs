using BusinessLogic.Entities;
using Model.Entities;

namespace BusinessLogic.Services.Audits.Interface
{
    public interface IAuditService
    {
        public Task CreateAsync(Audit audit);
        public Task<Audit> GetAsync(Guid uid);
        public Task<ServiceResponse<ResponseAuditDTOs>> GetAll(FilterAttributeAudit filterAttributeAudit);
    }
}
