namespace Encryptarium.Access.Entities.DTOs
{
    public class CreateSecretPolicyDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}
