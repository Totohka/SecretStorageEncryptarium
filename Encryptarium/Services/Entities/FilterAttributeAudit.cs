using Model.Enums;

namespace BusinessLogic.Entities
{
    public class FilterAttributeAudit
    {
        public Guid? UserUid { get; set; }
        public MicroservicesEnum? Microservice { get; set; } 
        public ControllersEnum? Controller { get; set; }
        public string? Method { get; set; }
        public AuthorizePoliciesEnum? AuthorizePolicies { get; set; }
        public StatusCodeEnum? StatusCode { get; set; }
        public DateTime DateStart { get; set; } = DateTime.UtcNow.AddDays(-1);
        public DateTime DateEnd { get; set; } = DateTime.UtcNow;
        public EntitiesEnum? Entity { get; set; }
        public Guid? EntityUid { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
