using Model.Enums;

namespace BusinessLogic.Entities
{
    public class AuditDTO
    {
        public string Login { get; set; }
        public Guid? UserUid { get; set; }
        public StatusCodeEnum StatusCode { get; set; }
        public string Action { get; set; }
        public bool IsSourceRightAdmin { get; set; }
        public DateTime DateAct { get; set; }
    }
}
