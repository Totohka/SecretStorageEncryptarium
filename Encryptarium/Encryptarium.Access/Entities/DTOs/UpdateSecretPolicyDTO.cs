namespace Encryptarium.Access.Entities.DTOs
{
    public class UpdateSecretPolicyDTO
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}
