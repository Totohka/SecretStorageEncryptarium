using BusinessLogic.Entities;
using BusinessLogic.Services.Audits.Interface;
using Encryptarium.Audit.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enums;

namespace Encryptarium.Audit.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditController> _logger;
        public AuditController(IAuditService auditService, ILogger<AuditController> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        [Monitoring(MicroservicesEnum.Audit, ControllersEnum.AuditController, nameof(GetRecord), EntitiesEnum.Audit, PartHttpContextEnum.RequestParameter, "uid")]
        [HttpGet("{uid}")]
        [Authorize(Policy = Constants.TokenPolicy)]
        public async Task<IActionResult> GetRecord(Guid uid)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AuditController)}.{nameof(GetRecord)}()");

            return Ok(await _auditService.GetAsync(uid));
        }

        [Monitoring(MicroservicesEnum.Audit, ControllersEnum.AuditController, nameof(GetAllRecords), EntitiesEnum.Audit, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy)]
        [HttpPost]
        public async Task<IActionResult> GetAllRecords(FilterAttributeAudit filterAttributeAudit)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AuditController)}.{nameof(GetAllRecords)}()");
            var result = await _auditService.GetAll(filterAttributeAudit);
            return Ok(result);
        }

        [Monitoring(MicroservicesEnum.Audit, ControllersEnum.AuditController, nameof(GetAllRecordsForAdmin), EntitiesEnum.Audit, PartHttpContextEnum.None)]
        [Authorize(Policy = Constants.TokenPolicy, Roles = Constants.Admin)]
        [HttpPost(nameof(GetAllRecordsForAdmin))]
        public async Task<IActionResult> GetAllRecordsForAdmin(FilterAttributeAudit filterAttributeAudit)
        {
            _logger.LogInformation($"{Constants.PrefixLog} {nameof(AuditController)}.{nameof(GetAllRecordsForAdmin)}()");
            var result = await _auditService.GetAll(filterAttributeAudit);
            return Ok(result);
        }
    }
}
