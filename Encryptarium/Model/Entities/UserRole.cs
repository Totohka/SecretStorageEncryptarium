namespace Model.Entities
{
    public class UserRole : BaseEntity
    {
        public Guid UserUid { get; set; }
        public User User { get; set; }
        public Guid RoleUid { get; set; }
        public Role Role { get; set; }
        public bool IsMain { get; set; }
    }
}
