namespace Encryptarium.Access.Entities.DTOs
{
    public class CreateRoleDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RoleTypeUid { get; set; }
    }
}
