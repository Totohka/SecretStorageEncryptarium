namespace Encryptarium.Access.Entities.DTOs
{
    public class CreateLinkStorageDTO
    {
        public Guid RoleUid { get; set; }
        public Guid StoragePolicyUid { get; set; }
        public Guid StorageUid { get; set; }
    }
}
