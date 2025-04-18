namespace Model.Entities
{
    public class Role : BaseEntity
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid RoleTypeUid { get; set; }
        public RoleType RoleType { get; set; }
        public bool IsActive { get; set; }
        public List<SecretLinkPolicy> SecretLinkPolicies { get; set; }
        public List<StorageLinkPolicy> StorageLinkPolicies { get; set; }
    }
}
