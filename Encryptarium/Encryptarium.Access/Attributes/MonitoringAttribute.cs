using Model.Enums;

namespace Encryptarium.Access.Attributes
{
    public class MonitoringAttribute : Attribute
    {
        public MonitoringAttribute(MicroservicesEnum microservice,
                                   ControllersEnum controller,
                                   string method,
                                   EntitiesEnum entity,
                                   PartHttpContextEnum partHttpContext,
                                   string? nameParameter = null)
        {
            Microservice = microservice;
            Controller = controller;
            Method = method;
            Entity = entity;
            PartHttpContext = partHttpContext;
            NameParameter = nameParameter;
        }
        public MicroservicesEnum Microservice { get; set; }
        public ControllersEnum Controller { get; set; }
        public EntitiesEnum Entity { get; set; }
        public PartHttpContextEnum PartHttpContext { get; set; }
        public string Method { get; set; }
        public string? NameParameter { get; set; }
    }
}
