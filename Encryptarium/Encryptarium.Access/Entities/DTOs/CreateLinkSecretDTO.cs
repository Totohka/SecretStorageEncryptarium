namespace Encryptarium.Access.Entities.DTOs
{
    public class CreateLinkSecretDTO
    {
        public Guid RoleUid { get; set; }
        public Guid SecretPolicyUid { get; set; }
        public Guid SecretUid { get; set; }
    }
}
