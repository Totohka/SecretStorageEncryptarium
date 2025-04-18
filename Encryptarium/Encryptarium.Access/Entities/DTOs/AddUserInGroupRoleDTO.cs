namespace Encryptarium.Access.Entities.DTOs
{
    public class AddUserInGroupRoleDTO
    {
        public Guid UserUid { get; set; }
        public Guid RoleUid { get; set; }
        public bool IsMain { get; set; }
    }
}
