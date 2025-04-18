using Model.Entities;

namespace BusinessLogic.Entities
{
    public class ResponseAuditDTOs
    {
        public List<Audit> Audits { get; set; }
        public int Count { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}
