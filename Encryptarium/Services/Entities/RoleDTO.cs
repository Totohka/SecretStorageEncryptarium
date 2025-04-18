using Model.Entities;

namespace BusinessLogic.Entities
{
    public class RoleDTO
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CodeType { get; set; }
        public bool IsActive { get; set; }
    }
}
