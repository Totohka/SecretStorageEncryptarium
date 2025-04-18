namespace BusinessLogic.Entities
{
    public class GroupRoleDTO
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string StorageTitle { get; set; }
        public string StorageDescription { get; set; }
        public Dictionary<Guid, RightForGroupDTO> RightUsers { get; set; }
    }
}
