using Model.Enums;

namespace BusinessLogic.Entities
{
    public class MonitorMessage
    {
        public Guid UserUid { get; set; }
        public ControllersEnum SourceController { get; set; }
        public MicroservicesEnum SourceMicroservice { get; set; }
        public AuthorizePoliciesEnum AuthorizePolice { get; set; }
        public Guid? EntityUid { get; set; }
        public EntitiesEnum Entity { get; set; }
        public string SourceMethod { get; set; }
        public StatusCodeEnum StatusCode { get; set; }
        public DateTime DateAct { get; set; }
        public bool IsSourceRightAdmin { get; set; }
        public string Action { get; set; }
    }
}
