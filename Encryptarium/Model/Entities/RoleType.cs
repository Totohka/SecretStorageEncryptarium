namespace Model.Entities
{
    public class RoleType : BaseEntity
    {
        public Guid Uid { get; set; }		
        public string Name { get; set; }		
        public int Code { get; set; }		
        public bool IsActive { get; set; }
    }
}
