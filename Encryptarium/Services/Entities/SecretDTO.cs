namespace BusinessLogic.Entities
{
    public class SecretDTO
    {
        public string Name { get; set; }
        public string? Secret { get; set; } = null;
        public Guid? StorageUid { get; set; } = null;
    }
}
